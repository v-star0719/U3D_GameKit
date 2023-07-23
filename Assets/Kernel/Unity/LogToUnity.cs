using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Kernel.Core;

public class LogToUnity : ILogTarget
{
    public void Init()
    {
    }

    public void Clear()
    {
    }

    public void OnLog(LogEvent ev)
    {
        try
        {
            //logging = true;
            if(Application.isEditor)
            {
                switch(ev.Level)
                {
                    case LogLevel.FATAL:
                    case LogLevel.ERROR:
                        Debug.LogError(ev.ToString());
                        break;
                    case LogLevel.WARN:
                        Debug.LogWarning(ev.ToString());
                        break;
                    case LogLevel.INFO:
                    case LogLevel.DEBUG:
                    case LogLevel.TRACE:
                        Debug.Log(ev.ToString());
                        break;
                }
            }
        }
        finally
        {
            //logging = false;
        }
    }
}