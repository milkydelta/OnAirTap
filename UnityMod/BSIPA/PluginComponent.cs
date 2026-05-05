using UnityEngine;
using IPA.Config;
using IPA.Config.Stores;


namespace OnAirTap;

internal class BSIPA_OatComponent: MonoBehaviour
{
    private static BSIPA_OatComponent instance;

    private Plugin plug;

    private void BindConfigs()
    {
        PluginConfig.Instance = BSIPAPlugin.cfg.Generated<PluginConfig>();
    }

    private void SendConfigs()
    {
        Plugin.configResX = PluginConfig.Instance.ResolutionX;
        Plugin.configResY = PluginConfig.Instance.ResolutionY;

        Plugin.configRenderBG = PluginConfig.Instance.ShouldRenderBG;
        Plugin.configRenderFG = PluginConfig.Instance.ShouldRenderFG;
        Plugin.configRenderOP = PluginConfig.Instance.ShouldRenderOptimised;

        Plugin.configGroundPlaneOn = PluginConfig.Instance.GroundClipPlaneEnabled;
        Plugin.configGroundPlaneHeight = PluginConfig.Instance.GroundClipPlaneElevation;
        Plugin.configVerticalClipPlane = PluginConfig.Instance.ClipPlaneShouldBeVertical;

        Plugin.configReadResFromShm = PluginConfig.Instance.ShouldReadResolutionFromMMF;
        Plugin.configReadClipFromShm = PluginConfig.Instance.ShouldReadTrackerFromMMF;
    }

    private void Awake()
    {
        if (instance != null){
            DestroyImmediate(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        instance = this;

        plug = new Plugin();

        BindConfigs();
        SendConfigs();

        plug.Awake();
    }

    void Update()
    {
        plug.Update();
    }

    void LateUpdate()
    {
        plug.LateUpdate();
    }


}