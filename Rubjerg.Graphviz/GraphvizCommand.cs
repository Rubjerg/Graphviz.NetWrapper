using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace Rubjerg.Graphviz;

/// <summary>
/// See https://graphviz.org/doc/info/command.html
/// </summary>
public class GraphvizCommand
{
    public static RootGraph CreateLayout(Graph input, string engine = LayoutEngines.Dot, CoordinateSystem coordinateSystem = CoordinateSystem.BottomLeft)
    {
        var output = Exec(input, engine: engine);
        var resultGraph = RootGraph.FromDotString(output, coordinateSystem);
        return resultGraph;
    }

    // FIXNOW: expose stderr and return code too
    public static string Exec(Graph input, string format = "xdot", string outputPath = null, string engine = LayoutEngines.Dot)
    {
        string exeName = "dot.exe";
        string arguments = $"-T{format} -K{engine}";
        if (outputPath != null)
        {
            arguments = $"{arguments} -o{outputPath}";
        }
        string inputToStdin = input.ToDotString();

        // Get the location of the currently executing DLL
        // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly.codebase?view=net-5.0
        string exeDirectory = AppDomain.CurrentDomain.RelativeSearchPath
            ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        // Construct the path to the executable
        string exePath = Path.Combine(exeDirectory, exeName);

        Process process = new Process();

        process.StartInfo.FileName = exePath;
        process.StartInfo.Arguments = arguments;

        // Redirect the input/output streams
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardError = true;

        _ = process.Start();

        // Write to stdin
        using (StreamWriter sw = process.StandardInput)
            sw.WriteLine(inputToStdin);

        // Read from stdout
        string output;
        using (StreamReader sr = process.StandardOutput)
            output = sr.ReadToEnd()
                .Replace("\r\n", "\n"); // File operations do this automatically, but stream operations do not

        // Read from stderr
        string error;
        using (StreamReader sr = process.StandardError)
            error = sr.ReadToEnd()
                .Replace("\r\n", "\n"); // File operations do this automatically, but stream operations do not

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            // Something went wrong.
            throw new ApplicationException($"Process exited with code {process.ExitCode}. Error details: {error}");
        }
        else
        {
            // Process completed successfully.
            return output;
        }
    }
}
