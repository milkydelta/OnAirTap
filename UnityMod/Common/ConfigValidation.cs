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
        if (mask.Length != 32){return false;}
        try
        {
            Convert.ToInt32(mask, 2);
        }
        catch (Exception)
        {
            return false;
        }
        return true;
    }
}