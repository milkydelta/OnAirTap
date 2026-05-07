namespace OnAirTap;

public abstract class AbLogger{
    abstract public void Info(string message);

    abstract public void Warn(string message);

    abstract public void Error(string message);
}