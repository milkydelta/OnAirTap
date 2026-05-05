using System;
using System.Runtime.InteropServices;

namespace OnAirTap;

public static class NativeMethods{

    [DllImport("lincomm", EntryPoint="open")]
    public static extern int LOpen(ref dataBlock dst);

    [DllImport("lincomm", EntryPoint="close")]
    public static extern int LClose(ref dataBlock dst);

    [DllImport("ntdll",CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr wine_get_version();

    public static string GetPlatform(){
        try{
            wine_get_version();
            return "Wine";
        }
        catch (EntryPointNotFoundException){
            return "Windows";
        }
        catch (DllNotFoundException){
            return "Unix";
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