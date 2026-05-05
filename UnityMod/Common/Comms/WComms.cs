using System.IO.MemoryMappedFiles;

namespace OnAirTap;

public class WComms : AbComms {
    private MemoryMappedFile mmf;
    private MemoryMappedViewAccessor mmfView;

    private float[] cameraData = new float[9];

    public override bool Open(string targetName){
        mmf = MemoryMappedFile.CreateOrOpen(targetName, (sizeof(float) * 8)+sizeof(int));
        mmfView = mmf.CreateViewAccessor(0, (sizeof(float) * 8)+sizeof(int), MemoryMappedFileAccess.Read);

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

        return dat;
    }
}