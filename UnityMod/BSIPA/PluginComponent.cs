using UnityEngine;
using IPA.Config;
using IPA.Config.Stores;
using System;


namespace OnAirTap.BSIPA;

internal class BSIPA_OatComponent : MonoBehaviour
{
    internal static BSIPA_OatComponent instance;

    private Plugin plug;

    bool hasAwoken = false;

    private void BindConfigs()
    {
        PluginConfig.Instance = BSIPAPlugin.cfg.Generated<PluginConfig>();
    }

    private void ValidateConfigs()
    {
        PluginConfig defaultValues = new PluginConfig();

        if (!ConfigValidation.Resolution(PluginConfig.Instance.ResolutionX, PluginConfig.Instance.ResolutionY))
        {
            PluginConfig.Instance.ResolutionX = defaultValues.ResolutionX;
            PluginConfig.Instance.ResolutionY = defaultValues.ResolutionY;
        }
        if (!ConfigValidation.LayerMaskString(PluginConfig.Instance.LayerMaskString))
        {
            PluginConfig.Instance.LayerMaskString = defaultValues.LayerMaskString;
        }
    }

    private void SendConfigs()
    {
        Plugin.cfg.ResX = PluginConfig.Instance.ResolutionX;
        Plugin.cfg.ResY = PluginConfig.Instance.ResolutionY;

        Plugin.cfg.RenderBG = PluginConfig.Instance.ShouldRenderBG;
        Plugin.cfg.RenderFG = PluginConfig.Instance.ShouldRenderFG;
        Plugin.cfg.RenderOP = PluginConfig.Instance.ShouldRenderOptimised;

        Plugin.cfg.GroundPlaneOn = PluginConfig.Instance.GroundClipPlaneEnabled;
        Plugin.cfg.GroundPlaneHeight = PluginConfig.Instance.GroundClipPlaneElevation;
        Plugin.cfg.VerticalClipPlane = PluginConfig.Instance.ClipPlaneShouldBeVertical;

        Plugin.cfg.ReadResFromShm = PluginConfig.Instance.ShouldReadResolutionFromMMF;
        Plugin.cfg.ReadClipFromShm = PluginConfig.Instance.ShouldReadTrackerFromMMF;

        Plugin.cfg.SpoutSendBG = PluginConfig.Instance.ShouldSendBG;
        Plugin.cfg.SpoutSendFG = PluginConfig.Instance.ShouldSendFG;
        Plugin.cfg.SpoutSendOP = PluginConfig.Instance.ShouldSendOptimised;
        Plugin.cfg.BlankSpoutSenders = PluginConfig.Instance.BlankSpoutOnRenderDispose;

        Plugin.cfg.FarClip = PluginConfig.Instance.CameraFarClip;
        Plugin.cfg.ProtoMinorVer = PluginConfig.Instance.MMFProtocolMinorVersion;

        if (PluginConfig.Instance.LayerMaskString == "")
        {
            Plugin.cfg.LayerMask = 0;
        }
        else
        {
            Plugin.cfg.LayerMask = Convert.ToInt32(PluginConfig.Instance.LayerMaskString, 2);
        }

        if (PluginConfig.Instance.LayerMaskFG == "")
        {
            Plugin.cfg.LayerMaskFG = 0;
        }
        else
        {
            Plugin.cfg.LayerMaskFG = Convert.ToInt32(PluginConfig.Instance.LayerMaskFG, 2);
        }

        if (PluginConfig.Instance.LayerMaskOP == "")
        {
            Plugin.cfg.LayerMaskOP = 0;
        }
        else
        {
            Plugin.cfg.LayerMaskOP = Convert.ToInt32(PluginConfig.Instance.LayerMaskOP, 2);
        }

        Plugin.cfg.ClipBehaviour = PluginConfig.Instance.ClipPlaneBehaviour;

    }

    internal void ReloadConfig()
    {
        if (hasAwoken)
        {
            ValidateConfigs();
            SendConfigs();
            plug.ReloadConfig(true);
        }

    }

    private void Awake()
    {
        if (instance != null)
        {
            BSIPAPlugin.Log.Warn("The OatComponent already exists.");
            DestroyImmediate(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
        instance = this;

        Plugin.logger = new BSIPALogger(BSIPAPlugin.Log);

        plug = new Plugin();

        BindConfigs();

        ValidateConfigs();

        SendConfigs();

        plug.Awake();

        hasAwoken = true;
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