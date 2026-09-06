using System;
using System.Runtime.InteropServices;

namespace OnAirTap;

public class LComms : AbComms {

    dataBlock shm;

    private ushort pMV;

    public LComms(){
        shm = new dataBlock();
    }

    public override ICameraData Read(){
        if (!isOpen) {
            var s = new ShmStruct1_0();
            s.cfg = CamDatCfg.LOG_ON;
            return s;
        }

        switch (pMV) {
            case 0:
                return (ShmStruct1_0)Marshal.PtrToStructure(shm.data, typeof(ShmStruct1_0));
            case 1:
            default:
                return (ShmStruct1_1)Marshal.PtrToStructure(shm.data, typeof(ShmStruct1_1));
        }
    }

    public override bool Open(string targetName, ushort protocolMinorVersion) {
        Plugin.logger.Info("Unmanaged Wine Comms!");
        if (isOpen){return false;}
        if (shm.fd != 0 || shm.data != IntPtr.Zero) { return false;}

        shm.name = "/" + targetName + ".v1." + protocolMinorVersion.ToString();
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

    [DllImport("lincomm", EntryPoint="open")]
    private static extern int LOpen(ref dataBlock dst);

    [DllImport("lincomm", EntryPoint="close")]
    private static extern int LClose(ref dataBlock dst);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    private struct dataBlock{
        public string name;
        public int length;
        public IntPtr data;
        public int fd;
    };

}
