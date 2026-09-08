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
    bool FakeSetting {get{return false;} set{BSIPA_OatComponent.instance.ShouldReloadSettings = true;}}
    #endregion



    #region LayerSettings

    string fgStorage {get =>"This should be hidden."; set{Plugin.logger.Info("SET AGAIN!!");}}

    private List<string> passOptions = new string[] { "Background", "Foreground", "Optimised", }.ToList();

    private string passChoice = "Background";

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

    [UIAction("SetCurrentMaskToDefault")]
    private void ButtonPress()
    {
        if (Plugin.defaultLayerMask != 0){
            SetConfigLayerMask(Plugin.defaultLayerMask, passChoice);
        }
    }

    [UIAction("SaveCurrentMask")]
    private void OtherButtonPress()
    {
        Plugin.logger.Info("BUTTON");
        int mask = GetConfigLayerMask(passChoice);
        string maskString = Convert.ToString(mask, 2).PadLeft(32,'0');
        switch (passChoice)
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