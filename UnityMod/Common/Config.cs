
using System.Diagnostics.Eventing.Reader;

namespace OnAirTap;

internal class Config
{
    internal int ResX;
    internal int ResY;
    internal bool RenderBG;
    internal bool RenderFG;
    internal bool RenderOP;
    internal bool RenderEX;
    internal bool GroundPlaneOn;
    internal float GroundPlaneHeight;
    internal bool ReadResFromShm;
    internal bool ReadClipFromShm;
    internal bool VerticalClipPlane;
    //internal bool SpoutSendBG;
    //internal bool SpoutSendFG;
    //internal bool SpoutSendOP;
    internal bool BlankSpoutSenders;
    internal float FarClip;
    internal ushort ProtoMinorVer;
    internal int LayerMask;
    internal int LayerMaskFG;
    internal int LayerMaskOP;
    internal int LayerMaskEX;
    internal int ClipBehaviour;
    internal string MemoryPath="uk.lum.vrnyan.cameradata";
}