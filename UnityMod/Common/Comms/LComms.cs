using System;
using System.Runtime.InteropServices;

namespace OnAirTap;

public class LComms : AbComms {

    dataBlock shm;

    private float[] cameraData;

    private float[] clipVec;

    public LComms(){
        cameraData = new float[9];
        clipVec = new float[3];
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

        //New data - Should probably have an if 
        //somewhere to set this to zero if the protocolversion is lower than needed.

        dat.resX = Marshal.ReadInt32(shm.data, sizeof(float)*8 + sizeof(int));
        dat.resY = Marshal.ReadInt32(shm.data, sizeof(float)*8 + sizeof(int)*2);

        Marshal.Copy(shm.data, clipVec, (8 + 3), 3);

        dat.clipX = clipVec[0];
        dat.clipY = clipVec[1];
        dat.clipZ = clipVec[2];

        return dat;
    }

    public override bool Open(string targetName) {
        if (shm.fd != 0 || shm.data != IntPtr.Zero) { return false;}

        shm.name = "/" + targetName;
        shm.length = (sizeof(float) * 8)+sizeof(int);

        shm.length += sizeof(int) * 2;
        shm.length += sizeof(float) * 3;

        return NativeMethods.LOpen(ref shm) == 0;
    }

}