using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz;

/// <summary>
/// See https://graphviz.org/doc/info/command.html
/// </summary>
public class GraphvizCommand
{
    internal static string Rid
    {
        get
        {
            var os = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win"
                   : RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "osx"
                   : "linux";   // default

            var arch = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.Arm64 => "arm64",
                Architecture.X86 => "x86",
                Architecture.Arm => "arm",
                _ => "unknown"
            };

            return $"{os}-{arch}";
        }
    }

    public static RootGraph CreateLayout(Graph input, string engine = LayoutEngines.Dot, CoordinateSystem coordinateSystem = CoordinateSystem.BottomLeft)
    {
        var (stdout, stderr) = Exec(input, engine: engine);
        var stdoutStr = ConvertBytesOutputToString(stdout);
        var resultGraph = RootGraph.FromDotString(stdoutStr, coordinateSystem);
        resultGraph.Warnings = stderr;
        return resultGraph;
    }

    public static string ConvertBytesOutputToString(byte[] data)
    {
        // Just to be safe, make sure the input has unix line endings. Graphviz does not properly support
        // windows line endings passed to stdin when it comes to attribute line continuations.
        return Encoding.UTF8.GetString(data).Replace("\r\n", "\n");
    }

    /// <summary>
    /// Start dot.exe to compute a layout.
    /// </summary>
    /// <exception cref="ApplicationException">When the Graphviz process did not return successfully</exception>
    /// <returns>stderr may contain warnings, stdout is in utf8 encoding</returns>
    public static (byte[] stdout, string stderr) Exec(Graph input, string format = "xdot", string? outputPath = null, string engine = LayoutEngines.Dot)
    {
        var exeName = Path.Combine(AppContext.BaseDirectory, "runtimes", Rid, "native", "dot");
        string arguments = $"-T{format} -K{engine}";
        if (outputPath != null)
        {
            arguments = $"{arguments} -o\"{outputPath}\"";
        }
        string? inputToStdin = input.ToDotString();

        // Get the location of the currently executing DLL
        // https://learn.microsoft.com/en-us/dotnet/api/system.reflection.assembly.codebase?view=net-5.0
        string exeDirectory = AppDomain.CurrentDomain.RelativeSearchPath
            ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            ?? Path.GetDirectoryName(System.AppContext.BaseDirectory);

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
        process.StartInfo.StandardOutputEncoding = Encoding.UTF8;
        process.StartInfo.StandardErrorEncoding = Encoding.UTF8;
        // In some situations starting a new process also starts a new console window, which is distracting and causes slowdown.
        // This flag prevents this from happening.
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

        StringBuilder stderr = new StringBuilder();
        process.ErrorDataReceived += (_, e) => stderr.AppendLine(e.Data);

        _ = process.Start();
        process.BeginErrorReadLine();

        // Write to stdin
        var inputBytes = Encoding.UTF8.GetBytes(inputToStdin);
        using (var sw = process.StandardInput.BaseStream)
            sw.Write(inputBytes, 0, inputBytes.Length);

        // Read from stdout, can be binary output such as pdf
        byte[] stdout;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            process.StandardOutput.BaseStream.CopyTo(memoryStream);
            stdout = memoryStream.ToArray();
        }

        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            // Something went wrong.
            throw new ApplicationException($"Process exited with code {process.ExitCode}. Error details: {stderr}");
        }
        else
        {
            // Process completed successfully.
            // Let's use unix line endings for consistency with stdout
            return (stdout, stderr.ToString().Replace("\r\n", "\n"));
        }
    }
}
