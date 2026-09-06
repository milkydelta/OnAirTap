using System.IO.MemoryMappedFiles;

namespace OnAirTap;

public class WComms : AbComms {
    protected MemoryMappedFile mmf;
    protected MemoryMappedViewAccessor mmfView;

    protected ushort pMV;

    public override bool Open(string targetName, ushort protocolMinorVersion){
        Plugin.logger.Info("Managed Windows Comms!");
        if (isOpen){return false;}
        string name = targetName + ".v1." + protocolMinorVersion.ToString();
        pMV = protocolMinorVersion;

        int size = (sizeof(float) * 8)+sizeof(int);
        if (protocolMinorVersion >= 1) {
            size += sizeof(int) * 2;
            size += sizeof(float) * 3;
        }

        mmf = MemoryMappedFile.CreateOrOpen(name, size);
        mmfView = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

        isOpen = true;
        return true;
    }

    public override ICameraData Read() {
        if (!isOpen) {
            var s = new ShmStruct1_0();
            s.cfg = CamDatCfg.LOG_ON;
            return s;
        }

        switch (pMV) {
            case 0:
                ShmStruct1_0 s;
                mmfView.Read<ShmStruct1_0>(0, out s);
                return s;
            case 1:
            default:
                ShmStruct1_1 s1;
                mmfView.Read<ShmStruct1_1>(0, out s1);
                return s1;
        }
    }

    public override void Close()
    {
        if (!isOpen){return;}

        isOpen = false;
        mmfView.Dispose();
        mmf.Dispose();
    }
}