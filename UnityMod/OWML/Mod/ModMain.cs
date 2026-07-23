using OWML.Common;
using OWML.ModHelper;

using OnAirTap;
using System;

namespace OnAirTap.OWML;

public class ModMain : ModBehaviour
{
	public static ModMain instance;

	OnAirTap.Plugin plug;

	public void Awake()
	{
		instance = this;
	}

	public void ValidateConfigs()
	{
		var cfg = ModHelper.Config;

		if (!ConfigValidation.Resolution(cfg.GetSettingsValue<int>("resolutionX"), cfg.GetSettingsValue<int>("resolutionY")))
		{
			cfg.SetSettingsValue("resolutionX", 1920);
			cfg.SetSettingsValue("resolutionY", 1080);
		}

		if (!ConfigValidation.LayerMaskString(cfg.GetSettingsValue<string>("layerMaskString"))) {
			cfg.SetSettingsValue("layerMaskString", "00000000000000000000000000000000");
		}
		if (!ConfigValidation.LayerMaskString(cfg.GetSettingsValue<string>("layerMaskStringFG"))) {
			cfg.SetSettingsValue("layerMaskStringFG", "00000000000000000000000000000000");
		}
		if (!ConfigValidation.LayerMaskString(cfg.GetSettingsValue<string>("layerMaskStringOP"))) {
			cfg.SetSettingsValue("layerMaskStringOP", "00000000000000000000000000000000");
		}
	}
	public void SendConfigs()
	{
		var cfg = ModHelper.Config;
		Config conf = Plugin.cfg;

		//Resolution

		conf.ResX = cfg.GetSettingsValue<int>("resolutionX");
		conf.ResY = cfg.GetSettingsValue<int>("resolutionY");

		//RenderPasses

		conf.RenderBG = cfg.GetSettingsValue<bool>("renderBG");
		conf.RenderFG = cfg.GetSettingsValue<bool>("renderFG");
		conf.RenderOP = cfg.GetSettingsValue<bool>("renderOP");

		conf.SpoutSendBG = cfg.GetSettingsValue<bool>("sendBG");
		conf.SpoutSendFG = cfg.GetSettingsValue<bool>("sendFG");
		conf.SpoutSendOP = cfg.GetSettingsValue<bool>("sendOP");

		conf.BlankSpoutSenders = cfg.GetSettingsValue<bool>("blankSenders");

		string mask = cfg.GetSettingsValue<string>("layerMaskString");
		if (mask == ""){conf.LayerMask = 0;}
		else {conf.LayerMask = Convert.ToInt32(mask, 2);}

		mask = cfg.GetSettingsValue<string>("layerMaskStringFG");
		if (mask == ""){conf.LayerMaskFG = 0;}
		else {conf.LayerMaskFG = Convert.ToInt32(mask, 2);}

		mask = cfg.GetSettingsValue<string>("layerMaskStringOP");
		if (mask == ""){conf.LayerMaskOP = 0;}
		else {conf.LayerMaskOP = Convert.ToInt32(mask, 2);}

		//ClipPlanes

		conf.GroundPlaneOn = cfg.GetSettingsValue<bool>("groundOn");
		conf.GroundPlaneHeight = cfg.GetSettingsValue<float>("groundUp");

		conf.FarClip = cfg.GetSettingsValue<float>("farClip");

		string behaviour = cfg.GetSettingsValue<string>("clipBehave");
		switch (behaviour)
		{
			case "Distance-based":
				conf.ClipBehaviour = 1;
				break;
			case "Simple":
			default:
				conf.ClipBehaviour = 0;
				break;
		}

		conf.VerticalClipPlane = cfg.GetSettingsValue<bool>("clipVertical");

		// OAT_MMF_ExtraData

		conf.ProtoMinorVer = Convert.ToUInt16(cfg.GetSettingsValue<string>("protoMinorVer"));

		conf.ReadResFromShm = cfg.GetSettingsValue<bool>("readRes");
		conf.ReadClipFromShm = cfg.GetSettingsValue<bool>("readClip");

	}

	public void ReloadConfig()
	{
		ValidateConfigs();
        SendConfigs();
        plug.ReloadConfig(true);
    }

    public override void Configure(IModConfig config)
    {
        ReloadConfig();
    }

	public void Start()
	{
		Plugin.logger = new OWLogger(ModHelper.Console);

		plug = new Plugin();

		ValidateConfigs();

		SendConfigs();

		plug.Awake();

		ModHelper.Console.WriteLine($"{"milkydelta.OnAirTap"} is loaded!", MessageType.Success);
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

