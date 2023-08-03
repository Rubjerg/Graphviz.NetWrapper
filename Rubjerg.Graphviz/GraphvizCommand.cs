using System;
using System.Diagnostics;
using System.IO;

namespace Rubjerg.Graphviz
{
    /// <summary>
    /// See https://graphviz.org/doc/info/command.html
    /// </summary>
    public class GraphvizCommand
    {
        public static XDotGraph Layout(Graph input, string outputPath = null, string engine = LayoutEngines.Dot)
        {
            string exeName = "dot.exe";
            string arguments = $"-Txdot -K{engine}";
            if (outputPath != null)
            {
                arguments = $"{arguments} -o{outputPath}";
            }
            string inputToStdin = input.ToDotString();

            // Get the location of the currently executing DLL
            //string dllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string exeDirectory = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

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
                var resultGraph = XDotGraph.FromDotString(output);
                return resultGraph;
            }
        }
    }
}
