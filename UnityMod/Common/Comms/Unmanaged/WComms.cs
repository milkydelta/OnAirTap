using System;
using System.Runtime.InteropServices;

namespace OnAirTap;

public class WComms : AbComms {

    dataBlock shm;

    private float[] cameraData;

    private float[] clipVec;

    private ushort pMV;

    public WComms(){
        cameraData = new float[9];
        clipVec = new float[3];
        shm = new dataBlock();
    }

    public override LIVnyan_dat Read() {
        LIVnyan_dat dat = new LIVnyan_dat();
        if (!isOpen){dat.cfg = LIVnyan_cfg.LOG_ON; return dat;}

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

        if (pMV >= 1) {
            dat.resX = Marshal.ReadInt32(shm.data, sizeof(float)*8 + sizeof(int));
            dat.resY = Marshal.ReadInt32(shm.data, sizeof(float)*8 + sizeof(int)*2);

            Marshal.Copy(new IntPtr(shm.data.ToInt64() + (sizeof(float)*8 + sizeof(int)*3)), clipVec, 0, 3);

            dat.clipX = clipVec[0];
            dat.clipY = clipVec[1];
            dat.clipZ = clipVec[2];
        }

        return dat;
    }

    public override bool Open(string targetName, ushort protocolMinorVersion) {
        Plugin.logger.Info("Unmanaged Windows Comms!");
        if (isOpen){return false;}
        if (shm.fd != IntPtr.Zero || shm.data != IntPtr.Zero) { return false;}

        shm.name = "Local\\" + targetName + ".v1." + protocolMinorVersion.ToString();
        shm.length = (sizeof(float) * 8)+sizeof(int);

        if (protocolMinorVersion >= 1) {
            shm.length += sizeof(int) * 2;
            shm.length += sizeof(float) * 3;
        }
        pMV = protocolMinorVersion;

        
        if (LOpen(ref shm) == 0) {
            isOpen = true;
            return true;
        }
        return false;
        
    }

    public override void Close()
    {
        if (!isOpen){return;}

        isOpen = false;

        LClose(ref shm);
        shm = new dataBlock();

        return;
    }

    [DllImport("wincomm", EntryPoint="wopen")]
    private static extern int LOpen(ref dataBlock dst);

    [DllImport("wincomm", EntryPoint="wclose")]
    private static extern int LClose(ref dataBlock dst);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private struct dataBlock{
        public string name;
        public int length;
        public IntPtr data;
        public IntPtr fd; //actually a HANDLE
    };

}
