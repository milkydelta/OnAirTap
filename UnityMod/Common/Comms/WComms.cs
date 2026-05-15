using System.IO.MemoryMappedFiles;

namespace OnAirTap;

public class WComms : AbComms {
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor mmfView;

    private float[] cameraData = new float[9];

    public override bool Open(string targetName){
        int size = (sizeof(float) * 8)+sizeof(int);
        size += sizeof(int) * 2;

        mmf = MemoryMappedFile.CreateOrOpen(targetName, size);
        mmfView = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

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

        dat.cfg = (LIVnyan_cfg)mmfView.ReadInt32(sizeof(float)*8);

        //New data - Should probably have an if 
        //somewhere to set this to zero if the protocolversion is lower than needed.
        
        dat.resX = mmfView.ReadInt32(sizeof(float)*8 + sizeof(int));
        dat.resY = mmfView.ReadInt32(sizeof(float)*8 + sizeof(int)*2);

        dat.clipX = mmfView.ReadSingle(sizeof(float)*8 + sizeof(int)*3);
        dat.clipY = mmfView.ReadSingle(sizeof(float)*9 + sizeof(int)*3);
        dat.clipZ = mmfView.ReadSingle(sizeof(float)*10 + sizeof(int)*3);        
        
        return dat;
    }
}