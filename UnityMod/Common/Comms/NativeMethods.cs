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