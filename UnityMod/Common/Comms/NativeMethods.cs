using System;
using System.Runtime.InteropServices;

namespace OnAirTap;

public static class NativeMethods{

    public enum Platform
    {
        Wine,
        Windows,
        Unix
    }

    [DllImport("lincomm", EntryPoint="open")]
    public static extern int LOpen(ref dataBlock dst);

    [DllImport("lincomm", EntryPoint="close")]
    public static extern int LClose(ref dataBlock dst);

    [DllImport("ntdll",CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr wine_get_version();

    public static Platform GetPlatform(){
        try{
            wine_get_version();
            return Platform.Wine;
        }
        catch (EntryPointNotFoundException){
            return Platform.Windows;
        }
        catch (DllNotFoundException){
            return Platform.Unix;
        }
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct dataBlock{
    public string name;
    public int length;
    public IntPtr data;
    public int fd;
};