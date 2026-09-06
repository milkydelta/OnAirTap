using System;

namespace OnAirTap;



public abstract class AbComms{
    internal bool isOpen=false;
    abstract public bool Open(string targetName, ushort protocolMinorVersion);
    abstract public ICameraData Read();
    abstract public void Close();
}