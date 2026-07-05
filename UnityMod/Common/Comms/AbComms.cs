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

    public bool HasSetting(LIVnyan_cfg flag)
    {
        return (cfg & flag) != LIVnyan_cfg.None;
    }
};

[Flags]
public enum LIVnyan_cfg {
    None         = 0b0000_0000,
    CAM_ON       = 0b0000_0001,
    LOG_ON       = 0b0000_0010,
    LOGSPM       = 0b0000_0100,
    OAT_READCLIP = 0b0000_1000
}

public abstract class AbComms{
    internal bool isOpen=false;
    abstract public bool Open(string targetName, ushort protocolMinorVersion);
    abstract public LIVnyan_dat Read();

    abstract public void Close();
}