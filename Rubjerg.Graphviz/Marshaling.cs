using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Rubjerg.Graphviz;

using static Constants;

internal static class Marshaling
{
    /// <summary>
    /// Marshal a c-string in utf8 encoding to a .NET string
    /// </summary>
    /// <param name="ptr">Pointer to the native c-string</param>
    /// <param name="free">Whether to free the native c-string after marshaling</param>
    /// <returns>.NET unicode string</returns>
    public static string? MarshalFromUtf8(IntPtr ptr, bool free)
    {
        var bytes = CopyCharPtrToByteArray(ptr, free);
        if (bytes is null)
            return null;
        return Encoding.UTF8.GetString(bytes);
    }

    public static void MarshalToUtf8(string? s, Action<IntPtr> continuation, bool free = true)
    {
        _ = MarshalToUtf8(s, ptr => { continuation(ptr); return 0; }, free);
    }

    /// <summary>
    /// Marshal a .NET string to a native c-string in utf8 encoding.
    /// </summary>
    /// <param name="s">the .NET string</param>
    /// <param name="free">Whether to free the allocated native c-string after the action returned</param>
    /// <param name="continuation">the continuation consuming the native c-string</param>
    /// <returns></returns>
    public static T MarshalToUtf8<T>(string? s, Func<IntPtr, T> continuation, bool free = true)
    {
        IntPtr ptr = IntPtr.Zero;
        if (s is null)
        {
            return continuation(ptr);
        }
        else
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            try
            {
                // allocate native c-string with an extra byte for the null terminator
                ptr = Marshal.AllocHGlobal(bytes.Length + 1);

                // copy the UTF8 bytes to the allocated memory
                Marshal.Copy(bytes, 0, ptr, bytes.Length);

                // set the null terminator
                Marshal.WriteByte(ptr + bytes.Length, 0);

                // call the continuation function with the native pointer
                return continuation(ptr);
            }
            finally
            {
                if (free && ptr != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(ptr);
                }
            }
        }
    }

    public static byte[]? CopyCharPtrToByteArray(IntPtr ptr, bool free)
    {
        if (ptr == IntPtr.Zero) return null;

        // Find the length of the string by looking for the null terminator
        int len = 0;
        while (Marshal.ReadByte(ptr, len) != 0)
        {
            len++;
        }

        byte[] byteArray = new byte[len];
        Marshal.Copy(ptr, byteArray, 0, len);
        if (free)
        {
            free_str(ptr);
        }
        return byteArray;
    }

    [DllImport(GraphvizWrapperLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern void free_str(IntPtr ptr);
    [DllImport(CGraphLib, SetLastError = true, CallingConvention = CallingConvention.Cdecl)]
    private static extern void agstrfree(IntPtr root, IntPtr str);
}
