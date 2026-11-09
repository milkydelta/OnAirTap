using System;

namespace OnAirTap;

internal static class ConfigValidation
{
    public static bool Resolution(int x, int y)
    {
        if (x <= 0 || y <= 0) {return false;}
        return true;
    }

    public static bool LayerMaskString(string mask)
    {
        if (mask == ""){return true;}

        if (mask.Length != 32){
            Plugin.logger.Warn("Layer Mask String - Length bad!");
            return false;
        }
        try
        {
            Convert.ToInt32(mask, 2);
        }
        catch (Exception)
        {
            Plugin.logger.Warn("Layer Mask String - Could not convert!");
            return false;
        }
        return true;
    }

    public static bool MemoryPath(string path)
    {
        if (path == null || path == ""){Plugin.logger.Warn("MemoryPath - empty");return false;}

        // This is probably unnecesssary, but it's good to be safe.
        // .NET documentation does not specify a length limit, and
        // Linux manpages for shm_open say it's "up to NAME_MAX
        // (i.e., 255) characters". I'm not sure where I got 32 from.
        if (path.Length > (32-".v1.1".Length)){Plugin.logger.Warn("MemoryPath - long");return false;}

        if (path.Contains("/") || path.Contains("\\")){Plugin.logger.Warn("MemoryPath - shenanigans");return false;}

        return true;
    }
}