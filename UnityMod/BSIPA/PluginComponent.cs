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

        Plugin.configSpoutSendBG = PluginConfig.Instance.ShouldSendBG;
        Plugin.configSpoutSendFG = PluginConfig.Instance.ShouldSendFG;
        Plugin.configSpoutSendOP = PluginConfig.Instance.ShouldSendOptimised;
        Plugin.configBlankSpoutSenders = PluginConfig.Instance.BlankSpoutOnRenderDispose;

        Plugin.configFarClip = PluginConfig.Instance.CameraFarClip;
        Plugin.configProtoMinorVer = PluginConfig.Instance.MMFProtocolMinorVersion;
        Plugin.configLayerMask = PluginConfig.Instance.LayerMaskOverride;
    }

    private void Awake()
    {
        if (instance != null){
            BSIPAPlugin.Log.Warn("The OatComponent already exists.");
            DestroyImmediate(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        instance = this;

        Plugin.logger = new BSIPALogger(BSIPAPlugin.Log);

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