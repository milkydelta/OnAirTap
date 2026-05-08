using System;

namespace OnAirTap;

public class BepLogger : AbLogger{

    public override void Info(string message)
    {
        if (enabled != true){return;}
        Console.WriteLine("OnAirTap Info: "+message);
    }

    public override void Warn(string message)
    {
        if (enabled != true){return;}
        Console.WriteLine("OnAirTap Warning: "+message);
    }

    public override void Error(string message)
    {
        if (enabled != true){return;}
        Console.WriteLine("OnAirTap Error: "+message);
    }  
}