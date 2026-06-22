using System.IO.MemoryMappedFiles;

namespace OnAirTap;

public class WComms : AbComms {
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor mmfView;

    private float[] cameraData = new float[9];

    private ushort pMV;

    public override bool Open(string targetName, ushort protocolMinorVersion){
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

    public override LIVnyan_dat Read() {
        LIVnyan_dat dat = new LIVnyan_dat();
        if (!isOpen){dat.cfg = LIVnyan_cfg.LOG_ON; return dat;}

        mmfView.ReadArray<float>(0,cameraData,0,8);

        dat.x = cameraData[0];
        dat.y = cameraData[1];
        dat.z = cameraData[2];
        dat.qw = cameraData[3];
        dat.qx = cameraData[4];
        dat.qy = cameraData[5];
        dat.qz = cameraData[6];
        dat.fov = cameraData[7];

        dat.cfg = (LIVnyan_cfg)mmfView.ReadInt32(sizeof(float)*8);

        
        if (pMV >= 1) {
            dat.resX = mmfView.ReadInt32(sizeof(float)*8 + sizeof(int));
            dat.resY = mmfView.ReadInt32(sizeof(float)*8 + sizeof(int)*2);

            dat.clipX = mmfView.ReadSingle(sizeof(float)*8 + sizeof(int)*3);
            dat.clipY = mmfView.ReadSingle(sizeof(float)*9 + sizeof(int)*3);
            dat.clipZ = mmfView.ReadSingle(sizeof(float)*10 + sizeof(int)*3);
        }       
        
        return dat;
    }

    public override void Close()
    {
        if (!isOpen){return;}

        isOpen = false;
        mmfView.Dispose();
        mmf.Dispose();
    }
}