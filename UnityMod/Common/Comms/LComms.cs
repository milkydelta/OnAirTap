using System;
using System.Runtime.InteropServices;

namespace OnAirTap;

public class LComms : AbComms {

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

        dat.cfg = (LIVnyan_cfg)Marshal.ReadInt32(shm.data, sizeof(float)*8);

        return dat;
    }

    public override bool Open(string targetName) {
        if (shm.fd != 0 || shm.data != IntPtr.Zero) { return false;}

        shm.name = "/" + targetName;
        shm.length = (sizeof(float) * 8)+sizeof(int);

        return NativeMethods.LOpen(ref shm) == 0;
    }

}