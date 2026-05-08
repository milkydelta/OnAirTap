namespace OnAirTap;

public abstract class AbLogger{
    public bool enabled=true;

    abstract public void Info(string message);

    abstract public void Warn(string message);

    abstract public void Error(string message);
}