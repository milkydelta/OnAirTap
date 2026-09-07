using System;
using System.Runtime.InteropServices;

namespace OnAirTap;

public class WComms : AbComms {

    dataBlock shm;

    private ushort pMV;

    public WComms(){
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
        Plugin.logger.Info("Unmanaged Windows Comms!");
        if (isOpen){return false;}
        if (shm.fd != IntPtr.Zero || shm.data != IntPtr.Zero) { return false;}

        shm.name = "Local\\" + targetName + ".v1." + protocolMinorVersion.ToString();
        switch (protocolMinorVersion)
        {
            case 0:
                shm.length = Marshal.SizeOf(typeof(ShmStruct1_0));
                break;
            case 1:
            default:
                shm.length = Marshal.SizeOf(typeof(ShmStruct1_1));
                break;
        }
        // shm.length = (sizeof(float) * 8)+sizeof(int);

        // if (protocolMinorVersion >= 1) {
        //     shm.length += sizeof(int) * 2;
        //     shm.length += sizeof(float) * 3;
        // }
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
