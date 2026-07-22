using MelonLoader;

namespace OnAirTap.Melon;

public class Mod : MelonMod
{
    MelonPreferences_Category categoryRes;
    MelonPreferences_Entry<int> configResX;
    MelonPreferences_Entry<int> configResY;

    MelonPreferences_Category categoryPasses;
    MelonPreferences_Entry<bool> configRenderBG;
    MelonPreferences_Entry<bool> configRenderFG;
    MelonPreferences_Entry<bool> configRenderOP;
    MelonPreferences_Entry<bool> configSendBG;
    MelonPreferences_Entry<bool> configSendFG;
    MelonPreferences_Entry<bool> configSendOP;
    MelonPreferences_Entry<bool> configBlankSpout;
    MelonPreferences_Entry<string> configLMS;

    MelonPreferences_Category categoryClip;
    MelonPreferences_Entry<bool> configGroundPlaneOn;
    MelonPreferences_Entry<float> configGroundPlaneHeight;
    MelonPreferences_Entry<bool> configVerticalClipPlane;
    MelonPreferences_Entry<float> configFarClip;
    MelonPreferences_Entry<int> configClipBehaviour;

    MelonPreferences_Category categoryMMF;
    MelonPreferences_Entry<bool> configReadResFromShm;
    MelonPreferences_Entry<bool> configReadClipFromShm;
    MelonPreferences_Entry<ushort> configProtoMinorVer;

    OnAirTap.Plugin plug;

    const string configPath = "UserData/OnAirTap.cfg";

    private void BindConfigs()
    {
        categoryRes = MelonPreferences.CreateCategory("Resolution");
        categoryRes.SetFilePath(configPath, autoload: false);
        configResX = categoryRes.CreateEntry<int>("X", 1920);
        configResY = categoryRes.CreateEntry<int>("Y", 1080);

        categoryPasses = MelonPreferences.CreateCategory("RenderPasses");
        categoryPasses.SetFilePath(configPath, autoload: false);
        configRenderBG = categoryPasses.CreateEntry<bool>("RenderBG", true);
        configRenderFG = categoryPasses.CreateEntry<bool>("RenderFG", true);
        configRenderOP = categoryPasses.CreateEntry<bool>("RenderOP", true);
        configSendBG = categoryPasses.CreateEntry<bool>("SendBG", true);
        configSendFG = categoryPasses.CreateEntry<bool>("SendFG", true);
        configSendOP = categoryPasses.CreateEntry<bool>("SendOP", true);
        configBlankSpout = categoryPasses.CreateEntry<bool>("BlankSendersOnDispose", true);
        configLMS = categoryPasses.CreateEntry<string>("LayerMask", "00000000000000000000000000000000");

        categoryClip = MelonPreferences.CreateCategory("ClipPlanes");
        categoryClip.SetFilePath(configPath, autoload: false);
        configGroundPlaneOn = categoryClip.CreateEntry<bool>("GroundPlaneOn", true);
        configGroundPlaneHeight = categoryClip.CreateEntry<float>("GroundPlaneHeight", 0.01f);
        configVerticalClipPlane = categoryClip.CreateEntry<bool>("VerticalClipPlane", true);
        configFarClip = categoryClip.CreateEntry<float>("FarClip", 1000);
        configClipBehaviour = categoryClip.CreateEntry<int>("ClipBehaviour", 1);

        categoryMMF = MelonPreferences.CreateCategory("OAT_MMF_Data");
        categoryMMF.SetFilePath(configPath, autoload: false);
        configReadResFromShm = categoryMMF.CreateEntry<bool>("ReadRes", false);
        configReadClipFromShm = categoryMMF.CreateEntry<bool>("ReadClip", true);
        configProtoMinorVer = categoryMMF.CreateEntry<ushort>("ProtocolMinorVersion", 1);

        categoryRes.LoadFromFile();
        categoryPasses.LoadFromFile(false);
        categoryClip.LoadFromFile(false);
        categoryMMF.LoadFromFile(false);
    }

    private void ValidateConfigs()
    {
        if (!ConfigValidation.Resolution(configResX.Value, configResY.Value))
        {
            configResX.ResetToDefault();
            configResY.ResetToDefault();
        }

        if (!ConfigValidation.LayerMaskString(configLMS.Value))
        {
            configLMS.ResetToDefault();
        }
    }

    private void SendConfigs()
    {
        OnAirTap.Config cfg = Plugin.cfg;

        cfg.ResX = configResX.Value;
        cfg.ResY = configResY.Value;

        cfg.RenderBG = configRenderBG.Value;
        cfg.RenderFG = configRenderFG.Value;
        cfg.RenderOP = configRenderOP.Value;

        cfg.GroundPlaneOn = configGroundPlaneOn.Value;
        cfg.GroundPlaneHeight = configGroundPlaneHeight.Value;
        cfg.VerticalClipPlane = configVerticalClipPlane.Value;

        cfg.ReadResFromShm = configReadResFromShm.Value;
        cfg.ReadClipFromShm = configReadClipFromShm.Value;

        cfg.SpoutSendBG = configSendBG.Value;
        cfg.SpoutSendFG = configSendFG.Value;
        cfg.SpoutSendOP = configSendOP.Value;
        cfg.BlankSpoutSenders = configBlankSpout.Value;
        cfg.FarClip = configFarClip.Value;

        cfg.ProtoMinorVer = configProtoMinorVer.Value;

        if (configLMS.Value == "")
        {
            cfg.LayerMask = 0;
        }
        else
        {
            cfg.LayerMask = Convert.ToInt32(configLMS.Value, 2);
        }

        cfg.ClipBehaviour = configClipBehaviour.Value;
    }

    public void ReloadEvent(string path)
    {
        if (path == configPath)
        {
            ReloadConfig();
        }
    }

    void ReloadConfig()
    {
        ValidateConfigs();
        SendConfigs();
        plug.ReloadConfig(true);
    }


    public override void OnInitializeMelon()
    {
        if (MelonLoader.Utils.MelonEnvironment.IsDotnetRuntime)
        {
            //We're not on mono. This must be an il2cpp game.
        }

        Plugin.logger = new MelonLogger(LoggerInstance);

        plug = new Plugin();

        BindConfigs();

        ValidateConfigs();

        SendConfigs();

        plug.Awake();

        MelonPreferences.OnPreferencesLoaded.Subscribe(ReloadEvent, 100);
    }

    public override void OnUpdate()
    {
        plug.Update();
    }

    public override void OnLateUpdate()
    {
        plug.LateUpdate();
    }
}
