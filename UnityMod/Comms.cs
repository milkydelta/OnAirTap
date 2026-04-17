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

class WComms {
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor mmfView;

    private float[] cameraData = new float[9];

    public bool Open(string targetName){
        mmf = MemoryMappedFile.CreateOrOpen("uk.lum.livnyan.cameradata.v1.0", (sizeof(float) * 8)+sizeof(int));
        mmfView = HoldingArea.mmf.CreateViewAccessor(0, (sizeof(float) * 8)+sizeof(int), MemoryMappedFileAccess.Read);

        return true;
    }

    public LIVnyan_dat Read() {
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

public static class liblincomm{

    [DllImport("lincomm")]
    public static extern int open(ref dataBlock dst);

    [DllImport("lincomm")]
    public static extern int close(ref dataBlock dst);
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct dataBlock{
    public string name;
    public int length;
    public IntPtr data;
    public int fd;
};

class LComms {

    dataBlock shm;

    private float[] cameraData;

    public LComms(){
        cameraData = new float[9];
        shm = new dataBlock();
    }

    public LIVnyan_dat Read() {
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

    public bool Open() {
        if (shm.fd != 0 || shm.data != IntPtr.Zero) { return false;}

        shm.name = "/uk.lum.livnyan.cameradata.v1.0";
        shm.length = (sizeof(float) * 8)+sizeof(int);

        return liblincomm.open(ref shm) == 0;
    }

}

