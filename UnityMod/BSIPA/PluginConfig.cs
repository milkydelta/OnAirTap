namespace OnAirTap;

//Thank you, BSIPA docs.
public class PluginConfig
{
    public static PluginConfig Instance { get; set; }

    public int ResolutionX { get; set; } = 1920;

    public int ResolutionY { get; set; } = 1080;

    public bool ShouldRenderBG { get; set; } = true;

    public bool ShouldRenderFG { get; set; } = true;

    public bool ShouldRenderOptimised { get; set; } = true;

    public bool GroundClipPlaneEnabled { get; set; } = true;

    public float GroundClipPlaneElevation { get; set; } = 0.01f;

    public bool ClipPlaneShouldBeVertical { get; set; } = true;

    public bool ShouldReadResolutionFromMMF { get; set; } = false;

    public bool ShouldReadTrackerFromMMF { get; set; } = false;

    public bool ShouldSendBG { get; set; } = true;

    public bool ShouldSendFG { get; set; } = false;

    public bool ShouldSendOptimised { get; set; } = true;

    public bool DisposeSpoutOnRenderDispose { get; set; } = false;
}