using System;
using System.Collections.Generic;
using System.Linq;
using BeatSaberMarkupLanguage.Attributes;

namespace OnAirTap.BSIPA;


public class SettingsUI
{
    /* 
    I'm documenting this for myself - in code rather than in git, so I actually know it later.

    For menus in the Mod Settings menu, the getter is called automatically, each time 
    you enter the Mod Settings menu. The setters are also called automatically, but only
    when the "OK" button is pressed. Each control will remember it's value between
    screens on the Mod Settings menu, but that value is only pushed to the backing
    variable/property on "OK".

    You can have the Setters called more often, by changing apply-on-change or set-event,
    but, if that setter updates a value in your plugin config, or causes any sort of
    persistent change, your mod will break from the convention/expectation established
    above. That would confuse users.
    */

    #region RegularSettings

    public int ResX {get => PluginConfig.Instance.ResolutionX; set {PluginConfig.Instance.ResolutionX=value;}}
    public int ResY {get => PluginConfig.Instance.ResolutionY; set {PluginConfig.Instance.ResolutionY=value;}}
    public bool RenderBG {get => PluginConfig.Instance.ShouldRenderBG; set {PluginConfig.Instance.ShouldRenderBG=value;}}
    public bool RenderFG {get => PluginConfig.Instance.ShouldRenderFG; set {PluginConfig.Instance.ShouldRenderFG=value;}}
    public bool RenderOP {get => PluginConfig.Instance.ShouldRenderOptimised; set {PluginConfig.Instance.ShouldRenderOptimised=value;}}
    public bool GCPEnabled {get => PluginConfig.Instance.GroundClipPlaneEnabled; set {PluginConfig.Instance.GroundClipPlaneEnabled=value;}}
    public float GCPElevation {get => PluginConfig.Instance.GroundClipPlaneElevation; set {PluginConfig.Instance.GroundClipPlaneElevation=value;}}
    public bool ClipPlaneVertical {get => PluginConfig.Instance.ClipPlaneShouldBeVertical; set {PluginConfig.Instance.ClipPlaneShouldBeVertical=value;}}
    public bool ReadResMMF {get => PluginConfig.Instance.ShouldReadResolutionFromMMF; set {PluginConfig.Instance.ShouldReadResolutionFromMMF=value;}}
    public bool ReadClipMMF {get => PluginConfig.Instance.ShouldReadTrackerFromMMF; set {PluginConfig.Instance.ShouldReadTrackerFromMMF=value;}}
    public bool BlankSpout {get => PluginConfig.Instance.BlankSpoutOnRenderDispose; set {PluginConfig.Instance.BlankSpoutOnRenderDispose=value;}}
    public float FarClip {get => PluginConfig.Instance.CameraFarClip; set {PluginConfig.Instance.CameraFarClip=value;}}
    public int ClipPlaneBehaviour {get => PluginConfig.Instance.ClipPlaneBehaviour; set {PluginConfig.Instance.ClipPlaneBehaviour=value;}}

    #endregion

    #region Shenanigans
    //I need a way to detect the "OK" press, so this is a hidden setting.
    bool FakeSetting { 
        get { return false; }
        set { 
            Plugin.logger.Info("SAVING!");
            BSIPA_OatComponent.instance.ShouldReloadSettings = true; 
            foreach (var pass in passOptions)
            {
                SaveMask(pass);
            }
            }
        }
    int bgStorage;
    int fgStorage;
    int opStorage;
    #endregion

    public SettingsUI()
    {
        bgStorage = Convert.ToInt32(PluginConfig.Instance.LayerMaskString, 2);
        fgStorage = Convert.ToInt32(PluginConfig.Instance.LayerMaskFG, 2);
        opStorage = Convert.ToInt32(PluginConfig.Instance.LayerMaskOP, 2);
    }


    #region LayerSettings


    private List<string> passOptions = new string[] { "Background", "Foreground", "Optimised", }.ToList();

    private string passChoice = "Background";

    public bool Layer00 { get { return GetLayer(0, passChoice); } set { SetLayer(value, 0, passChoice); } }
    public bool Layer01 { get { return GetLayer(1, passChoice); } set { SetLayer(value, 1, passChoice); } }
    public bool Layer02 { get { return GetLayer(2, passChoice); } set { SetLayer(value, 2, passChoice); } }
    public bool Layer03 { get { return GetLayer(3, passChoice); } set { SetLayer(value, 3, passChoice); } }
    public bool Layer04 { get { return GetLayer(4, passChoice); } set { SetLayer(value, 4, passChoice); } }
    public bool Layer05 { get { return GetLayer(5, passChoice); } set { SetLayer(value, 5, passChoice); } }
    public bool Layer06 { get { return GetLayer(6, passChoice); } set { SetLayer(value, 6, passChoice); } }
    public bool Layer07 { get { return GetLayer(7, passChoice); } set { SetLayer(value, 7, passChoice); } }
    public bool Layer08 { get { return GetLayer(8, passChoice); } set { SetLayer(value, 8, passChoice); } }
    public bool Layer09 { get { return GetLayer(9, passChoice); } set { SetLayer(value, 9, passChoice); } }
    public bool Layer10 { get { return GetLayer(10, passChoice); } set { SetLayer(value, 10, passChoice); } }
    public bool Layer11 { get { return GetLayer(11, passChoice); } set { SetLayer(value, 11, passChoice); } }
    public bool Layer12 { get { return GetLayer(12, passChoice); } set { SetLayer(value, 12, passChoice); } }
    public bool Layer13 { get { return GetLayer(13, passChoice); } set { SetLayer(value, 13, passChoice); } }
    public bool Layer14 { get { return GetLayer(14, passChoice); } set { SetLayer(value, 14, passChoice); } }
    public bool Layer15 { get { return GetLayer(15, passChoice); } set { SetLayer(value, 15, passChoice); } }
    public bool Layer16 { get { return GetLayer(16, passChoice); } set { SetLayer(value, 16, passChoice); } }
    public bool Layer17 { get { return GetLayer(17, passChoice); } set { SetLayer(value, 17, passChoice); } }
    public bool Layer18 { get { return GetLayer(18, passChoice); } set { SetLayer(value, 18, passChoice); } }
    public bool Layer19 { get { return GetLayer(19, passChoice); } set { SetLayer(value, 19, passChoice); } }
    public bool Layer20 { get { return GetLayer(20, passChoice); } set { SetLayer(value, 20, passChoice); } }
    public bool Layer21 { get { return GetLayer(21, passChoice); } set { SetLayer(value, 21, passChoice); } }
    public bool Layer22 { get { return GetLayer(22, passChoice); } set { SetLayer(value, 22, passChoice); } }
    public bool Layer23 { get { return GetLayer(23, passChoice); } set { SetLayer(value, 23, passChoice); } }
    public bool Layer24 { get { return GetLayer(24, passChoice); } set { SetLayer(value, 24, passChoice); } }
    public bool Layer25 { get { return GetLayer(25, passChoice); } set { SetLayer(value, 25, passChoice); } }
    public bool Layer26 { get { return GetLayer(26, passChoice); } set { SetLayer(value, 26, passChoice); } }
    public bool Layer27 { get { return GetLayer(27, passChoice); } set { SetLayer(value, 27, passChoice); } }
    public bool Layer28 { get { return GetLayer(28, passChoice); } set { SetLayer(value, 28, passChoice); } }
    public bool Layer29 { get { return GetLayer(29, passChoice); } set { SetLayer(value, 29, passChoice); } }
    public bool Layer30 { get { return GetLayer(30, passChoice); } set { SetLayer(value, 30, passChoice); } }
    public bool Layer31 { get { return GetLayer(31, passChoice); } set { SetLayer(value, 31, passChoice); } }
    #endregion

    [UIAction("SetCurrentMaskToDefault")]
    private void ButtonPress()
    {
        if (Plugin.defaultLayerMask != 0){
            SetLayerMask(Plugin.defaultLayerMask, passChoice);
        }
    }


    private void SaveMask( string choice)
    {
        Plugin.logger.Info("BUTTON");
        int mask = GetLayerMask(choice);
        string maskString = Convert.ToString(mask, 2).PadLeft(32,'0');
        switch (choice)
        {
            case "Foreground":
                PluginConfig.Instance.LayerMaskFG = maskString;
                break;
            case "Background":
                PluginConfig.Instance.LayerMaskString = maskString;
                break;
            case "Optimised":
                PluginConfig.Instance.LayerMaskOP = maskString;
                break;
            default:
                break;
        }

    }

    private int GetLayerMask(string pass)
    {
        int layerMask = 0;
        switch (pass)
        {
            case "Foreground":
                layerMask = fgStorage;
                break;
            case "Background":
                layerMask = bgStorage;
                break;
            case "Optimised":
                layerMask = opStorage;
                break;
            default:
                break;
        }
        return layerMask;
    }
    private void SetLayerMask(int mask, string pass)
    {
        switch (pass)
        {
            case "Foreground":
                fgStorage = mask;
                break;
            case "Background":
                bgStorage = mask;
                break;
            case "Optimised":
                opStorage = mask;
                break;
            default:
                break;
        }
    }
    private bool GetLayer(int layer, string pass)
    {
        int layerMask = GetLayerMask(pass);

        return ((layerMask >> layer) & 1) > 0;
    }
    private void SetLayer(bool to, int layer, string pass)
    {
        int layerMask = GetLayerMask(pass);

        if (to == true)
        {
            layerMask |= 1 << layer;
        }
        else
        {
            layerMask &= ~(1 << layer);
        }
        SetLayerMask(layerMask, pass);
    }
}