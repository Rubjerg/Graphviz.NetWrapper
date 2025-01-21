using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz;

public static class NativeMethods
{
#if _WINDOWS
    public static void CreateConsole()
    {
        _ = AllocConsole();

        // stdout's handle seems to always be equal to 7
        IntPtr defaultStdout = new IntPtr(7);
        IntPtr currentStdout = GetStdHandle(StdOutputHandle);

        if (currentStdout != defaultStdout)
            // reset stdout
            SetStdHandle(StdOutputHandle, defaultStdout);

        // reopen stdout
        TextWriter writer = new StreamWriter(Console.OpenStandardOutput())
        { AutoFlush = true };
        Console.SetOut(writer);
    }

    // P/Invoke required:
    private const uint StdOutputHandle = 0xFFFFFFF5;
    [DllImport("kernel32.dll")]
    private static extern IntPtr GetStdHandle(uint nStdHandle);
    [DllImport("kernel32.dll")]
    private static extern void SetStdHandle(uint nStdHandle, IntPtr handle);
    [DllImport("kernel32.dll")]
    static extern bool AllocConsole();
#endif
}
