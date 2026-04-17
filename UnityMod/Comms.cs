using System.Runtime.InteropServices;
using System.IO.MemoryMappedFiles;
using System;


namespace OnAirTap;

public struct LIVnyan_dat {
    public float x;
    public float y;
    public float z;
    public float qw;
    public float qx;
    public float qy;
    public float qz;
    public float fov;
    public int cfg;
};

abstract class AbComms{
    abstract public bool Open(string targetName);
    abstract public LIVnyan_dat Read();
}

class WComms : AbComms {
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor mmfView;

    private float[] cameraData = new float[9];

    public override bool Open(string targetName){
        mmf = MemoryMappedFile.CreateOrOpen(targetName, (sizeof(float) * 8)+sizeof(int));
        mmfView = HoldingArea.mmf.CreateViewAccessor(0, (sizeof(float) * 8)+sizeof(int), MemoryMappedFileAccess.Read);

        return true;
    }

    public override LIVnyan_dat Read() {
        LIVnyan_dat dat = new LIVnyan_dat();

        mmfView.ReadArray<float>(0,cameraData,0,8);

        dat.x = cameraData[0];
        dat.y = cameraData[1];
        dat.z = cameraData[2];
        dat.qw = cameraData[3];
        dat.qx = cameraData[4];
        dat.qy = cameraData[5];
        dat.qz = cameraData[6];
        dat.fov = cameraData[7];

        dat.cfg = mmfView.ReadInt32(sizeof(float)*8);

        return dat;
    }
}


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

class LComms : AbComms {

    dataBlock shm;

    private float[] cameraData;

    public LComms(){
        cameraData = new float[9];
        shm = new dataBlock();
    }

    public override LIVnyan_dat Read() {
        LIVnyan_dat dat = new LIVnyan_dat();

        Marshal.Copy(shm.data, cameraData, 0, 8);

        dat.x = cameraData[0];
        dat.y = cameraData[1];
        dat.z = cameraData[2];
        dat.qw = cameraData[3];
        dat.qx = cameraData[4];
        dat.qy = cameraData[5];
        dat.qz = cameraData[6];
        dat.fov = cameraData[7];

        dat.cfg = Marshal.ReadInt32(shm.data, sizeof(float)*8);

        return dat;
    }

    public override bool Open(string targetName) {
        if (shm.fd != 0 || shm.data != IntPtr.Zero) { return false;}

        shm.name = "/" + targetName;
        shm.length = (sizeof(float) * 8)+sizeof(int);

        return NativeMethods.LOpen(ref shm) == 0;
    }

}

