using System;
using System.Runtime.InteropServices;

namespace Rubjerg.Graphviz.FFI;

using static Marshaling;
using static Constants;

internal static class TestLib
{
    public enum TestEnum
    {
        Val1, Val2, Val3, Val4, Val5
    }

    // .NET uses UnmanagedType.Bool by default for P/Invoke, but our C++ code uses UnmanagedType.U1
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool echobool([MarshalAs(UnmanagedType.U1)] bool arg);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int echoint(int arg);

    public static string? EchoString(string? str)
    {
        return MarshalToUtf8(str, ptr =>
        {
            var returnPtr = TestLib.echo_string(ptr);
            // echo_string gives us ownership over the string, which means that we have to free it.
            return MarshalFromUtf8(returnPtr, true);
        });
    }
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern TestEnum echo_enum(TestEnum e);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int return1();
    public static string? ReturnCopyRight() => MarshalFromUtf8(return_copyright(), false);
    public static string? ReturnEmptyString() => MarshalFromUtf8(return_empty_string(), false);
    public static string? ReturnHello() => MarshalFromUtf8(return_hello(), false);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern int return_1();
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern TestEnum return_enum1();
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern TestEnum return_enum2();
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    public static extern TestEnum return_enum5();
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool return_false();
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    [return: MarshalAs(UnmanagedType.U1)]
    public static extern bool return_true();

    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr echo_string(IntPtr str);
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr return_copyright();
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr return_empty_string();
    [DllImport(GraphvizWrapperLibName, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr return_hello();
}