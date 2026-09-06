using System;
using UnityEngine;

namespace OnAirTap;

public struct Vector2Int
{
    public Vector2Int(int X, int Y){x=X;y=Y;}
    public int x;
    public int y;
}

public interface ICameraData
{
    public UnityEngine.Vector3 camPos{get;}
    public UnityEngine.Quaternion camRot{get;}
    public float vFov{get;}
    public bool HasSetting(CamDatCfg flag);
    //Vector2Int doesn't exist on Unity 5 :(
    public OnAirTap.Vector2Int resolution{get;}
    public UnityEngine.Vector3 clipPos{get;}
    public string ToString();
}

// 36 bytes
public struct ShmStruct1_0: ICameraData
{
    //Struct Layout
    public float x;
    public float y;
    public float z;
    public float qw;
    public float qx;
    public float qy;
    public float qz;
    public float fov;
    public CamDatCfg cfg;

    //Interface impl
    public Vector3 camPos => new Vector3(x, y, z);

    public Quaternion camRot => new Quaternion(qx, qy, qz, qw);

    public float vFov => fov;

    public Vector2Int resolution => new Vector2Int(0,0);

    public Vector3 clipPos => Vector3.zero;

    public bool HasSetting(CamDatCfg flag)
    {
        if (flag >= CamDatCfg.OAT_READCLIP){return false;}
        return (cfg & flag) != CamDatCfg.None;
    }

    public override string ToString()
    {
        return String.Concat(
            "P: ", x, ",", y, ",",  z,
            " R: ", qw,",", qx,",", qy,",", qz,
            " F: ",fov,
            " C: ", (int)cfg
        );
    }
}

// 56 bytes
public struct ShmStruct1_1: ICameraData
{
    //Struct Layout
    public float x;
    public float y;
    public float z;
    public float qw;
    public float qx;
    public float qy;
    public float qz;
    public float fov;
    public CamDatCfg cfg;
    public int resX;
    public int resY;
    public float clipX;
    public float clipY;
    public float clipZ;
    
    //Interface impl
    public Vector3 camPos => new Vector3(x, y, z);

    public Quaternion camRot => new Quaternion(qx, qy, qz, qw);

    public float vFov => fov;

    public Vector2Int resolution => new Vector2Int(resX, resY);

    public Vector3 clipPos => new Vector3(clipX, clipY, clipZ);

    public bool HasSetting(CamDatCfg flag)
    {
        return (cfg & flag) != CamDatCfg.None;
    }

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
}


[Flags]
public enum CamDatCfg : Int32 {
    None         = 0b0000_0000,
    CAM_ON       = 0b0000_0001,
    LOG_ON       = 0b0000_0010,
    LOGSPM       = 0b0000_0100,
    OAT_READCLIP = 0b0000_1000
}