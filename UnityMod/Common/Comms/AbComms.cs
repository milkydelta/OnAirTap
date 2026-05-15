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
    public LIVnyan_cfg cfg;
    //New data
    public int resX;
    public int resY;
    public float clipX;
    public float clipY;
    public float clipZ;

    public override string ToString()
    {
        return String.Concat(
            "P: ", x, ",", y, ",",  z,
            " R: ", qw,",", qx,",", qy,",", qz,
            " F: ",fov,
            " C: ", (int)cfg,
            " Rs: ", resX, ",",resY,
            " Cl: ", clipX, ",", clipY, ",",  clipZ
        );
    }
};

[Flags]
public enum LIVnyan_cfg {
    CAM_ON = 0x01,
    LOG_ON = 0x02,
    LOGSPM = 0x04
}

public abstract class AbComms{
    abstract public bool Open(string targetName);
    abstract public LIVnyan_dat Read();
}