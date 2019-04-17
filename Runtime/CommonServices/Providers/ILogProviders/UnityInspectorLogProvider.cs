using Atto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityInspectorLogProvider : MonoBehaviour, ILogService
{
    public string lastLog;
    public string lastInfo;
    public string lastNotice;
    public string lastWarning;
    public string lastError;

    //Usage: Core.Logger.Log("Some value", "#SomeLabel");
    public SerializableDictionary<string, string> valueDictionary;

    public void Log(string message, params object[] args)
    {
        if (args[0] is string)
        {
            var code = (string)args[0];
            if (code.StartsWith("#"))
            {
                if (valueDictionary.ContainsKey(code))
                {
                    valueDictionary[code] = message;
                }
            }
        }
        lastLog = message;
    }

    public void Error(string message, params object[] args)
    {
        lastError = message;
    }

    public void Info(string message, params object[] args)
    {
        lastInfo = message;
    }

    public void Notice(string message, params object[] args)
    {
        lastNotice = message;
    }

    public void Warning(string message, params object[] args)
    {
        lastWarning = message;
    }

    public void SetMinLogLevel(LogLevel minLogLevel)
    {
        //Not used
    }
}
