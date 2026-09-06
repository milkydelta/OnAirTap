using System;

namespace OnAirTap;



public abstract class AbComms{
    internal bool isOpen=false;
    abstract public bool Open(string targetName, ushort protocolMinorVersion);
    abstract public LIVnyan_dat Read();
    abstract public void Close();
}