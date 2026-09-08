using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;
using UnityEngine.UIElements;

namespace OnAirTap.BSIPA;


public class SettingsUI
{
    #region RegularSettings

    public int resX { get; set; } = 1920;
    public int resY { get; set; } = 1080;
    public bool RenderBG { get; set; } = true;
    public bool RenderFG { get; set; } = true;
    public bool RenderOptimised { get; set; } = true;
    public bool GroundClipPlaneEnabled { get; set; } = true;
    public float GroundClipPlaneElevation { get; set; } = 0.01f;
    public bool ClipPlaneShouldBeVertical { get; set; } = true;
    public bool ReadResolutionFromMMF { get; set; } = false;
    public bool ReadTrackerFromMMF { get; set; } = false;
    public bool BlankSpoutOnRenderDispose { get; set; } = false;
    public float CameraFarClip { get; set; } = 5000f;

    #endregion
    #region LayerSettings
    [UIValue("pass-options")]
    private List<string> passOptions = new string[] { "Foreground", "Background", "Optimised", }.ToList();

    [UIValue("pass-choice")]
    private string passChoice = "Foreground";

    public bool Layer00 { get { return GetConfigLayer(0, passChoice); } set { SetConfigLayer(value, 0, passChoice); } }
    public bool Layer01 { get { return GetConfigLayer(1, passChoice); } set { SetConfigLayer(value, 1, passChoice); } }
    public bool Layer02 { get { return GetConfigLayer(2, passChoice); } set { SetConfigLayer(value, 2, passChoice); } }
    public bool Layer03 { get { return GetConfigLayer(3, passChoice); } set { SetConfigLayer(value, 3, passChoice); } }
    public bool Layer04 { get { return GetConfigLayer(4, passChoice); } set { SetConfigLayer(value, 4, passChoice); } }
    public bool Layer05 { get { return GetConfigLayer(5, passChoice); } set { SetConfigLayer(value, 5, passChoice); } }
    public bool Layer06 { get { return GetConfigLayer(6, passChoice); } set { SetConfigLayer(value, 6, passChoice); } }
    public bool Layer07 { get { return GetConfigLayer(7, passChoice); } set { SetConfigLayer(value, 7, passChoice); } }
    public bool Layer08 { get { return GetConfigLayer(8, passChoice); } set { SetConfigLayer(value, 8, passChoice); } }
    public bool Layer09 { get { return GetConfigLayer(9, passChoice); } set { SetConfigLayer(value, 9, passChoice); } }
    public bool Layer10 { get { return GetConfigLayer(10, passChoice); } set { SetConfigLayer(value, 10, passChoice); } }
    public bool Layer11 { get { return GetConfigLayer(11, passChoice); } set { SetConfigLayer(value, 11, passChoice); } }
    public bool Layer12 { get { return GetConfigLayer(12, passChoice); } set { SetConfigLayer(value, 12, passChoice); } }
    public bool Layer13 { get { return GetConfigLayer(13, passChoice); } set { SetConfigLayer(value, 13, passChoice); } }
    public bool Layer14 { get { return GetConfigLayer(14, passChoice); } set { SetConfigLayer(value, 14, passChoice); } }
    public bool Layer15 { get { return GetConfigLayer(15, passChoice); } set { SetConfigLayer(value, 15, passChoice); } }
    public bool Layer16 { get { return GetConfigLayer(16, passChoice); } set { SetConfigLayer(value, 16, passChoice); } }
    public bool Layer17 { get { return GetConfigLayer(17, passChoice); } set { SetConfigLayer(value, 17, passChoice); } }
    public bool Layer18 { get { return GetConfigLayer(18, passChoice); } set { SetConfigLayer(value, 18, passChoice); } }
    public bool Layer19 { get { return GetConfigLayer(19, passChoice); } set { SetConfigLayer(value, 19, passChoice); } }
    public bool Layer20 { get { return GetConfigLayer(20, passChoice); } set { SetConfigLayer(value, 20, passChoice); } }
    public bool Layer21 { get { return GetConfigLayer(21, passChoice); } set { SetConfigLayer(value, 21, passChoice); } }
    public bool Layer22 { get { return GetConfigLayer(22, passChoice); } set { SetConfigLayer(value, 22, passChoice); } }
    public bool Layer23 { get { return GetConfigLayer(23, passChoice); } set { SetConfigLayer(value, 23, passChoice); } }
    public bool Layer24 { get { return GetConfigLayer(24, passChoice); } set { SetConfigLayer(value, 24, passChoice); } }
    public bool Layer25 { get { return GetConfigLayer(25, passChoice); } set { SetConfigLayer(value, 25, passChoice); } }
    public bool Layer26 { get { return GetConfigLayer(26, passChoice); } set { SetConfigLayer(value, 26, passChoice); } }
    public bool Layer27 { get { return GetConfigLayer(27, passChoice); } set { SetConfigLayer(value, 27, passChoice); } }
    public bool Layer28 { get { return GetConfigLayer(28, passChoice); } set { SetConfigLayer(value, 28, passChoice); } }
    public bool Layer29 { get { return GetConfigLayer(29, passChoice); } set { SetConfigLayer(value, 29, passChoice); } }
    public bool Layer30 { get { return GetConfigLayer(30, passChoice); } set { SetConfigLayer(value, 30, passChoice); } }
    public bool Layer31 { get { return GetConfigLayer(31, passChoice); } set { SetConfigLayer(value, 31, passChoice); } }
    #endregion

    [UIAction("pass-menu-pressed")]
    private void ButtonPress()
    {
        BSIPAPlugin.Log.Info(passChoice);
    }

    private int GetConfigLayerMask(string pass)
    {
        int layerMask = 0;
        switch (pass)
        {
            case "Foreground":
                layerMask = Plugin.cfg.LayerMaskFG;
                break;
            case "Background":
                layerMask = Plugin.cfg.LayerMask;
                break;
            case "Optimised":
                layerMask = Plugin.cfg.LayerMaskOP;
                break;
            default:
                break;
        }
        return layerMask;
    }
    private void SetConfigLayerMask(int mask, string pass)
    {
        switch (pass)
        {
            case "Foreground":
                Plugin.cfg.LayerMaskFG = mask;
                break;
            case "Background":
                Plugin.cfg.LayerMask = mask;
                break;
            case "Optimised":
                Plugin.cfg.LayerMaskOP = mask;
                break;
            default:
                break;
        }
    }
    private bool GetConfigLayer(int layer, string pass)
    {
        int layerMask = GetConfigLayerMask(pass);

        return ((layerMask >> layer) & 1) > 0;
    }
    private void SetConfigLayer(bool to, int layer, string pass)
    {
        int layerMask = GetConfigLayerMask(pass);

        if (to == true)
        {
            layerMask |= 1 << layer;
        }
        else
        {
            layerMask &= ~(1 << layer);
        }
        SetConfigLayerMask(layerMask, pass);
    }
}