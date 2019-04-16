
public interface ILogService
{
    void Log(string message, params object[] args);
    void Info(string message, params object[] args);
    void Notice(string message, params object[] args);
    void Warning(string message, params object[] args);
    void Error(string message, params object[] args);
}

public enum LogLevel
{
    Debug,
    Info,
    Notice,
    Warning,
    Error
}