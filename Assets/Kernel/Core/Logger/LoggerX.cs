using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kernel.Core
{
    public interface ILogTarget
    {
        void Init();
        void Clear();
        void OnLog(LogEvent e);
    }

    //游戏启动后注册需要的target。可以在任意时候添加一个新的，用完删除
    public static class LoggerX
    {
        public static List<ILogTarget> targets = new List<ILogTarget>();

        static LoggerX()
        {
            Enable = true;
        }

        public static void AddTarget(ILogTarget target)
        {
            target.Init();
            targets.Add(target);
        }

        public static void RemoveTarget(ILogTarget target)
        {
            target.Clear();
            targets.Remove(target);
        }

        /// <summary>
        ///     是否开启Log，Enable本身是bool，所以get和set是线程安全的。
        /// </summary>
        public static bool Enable { get; set; }

        public static void Fatal(string msg)
        {
            Log(LogLevel.FATAL, msg, null);
        }

        public static void Fatal(string msg, params object[] args)
        {
            Log(LogLevel.FATAL, string.Format(msg, args), null);
        }

        public static void Error(string msg)
        {
            Log(LogLevel.ERROR, msg, null);
        }

        public static void Error(string msg, params object[] args)
        {
            Log(LogLevel.ERROR, string.Format(msg, args), null);
        }

        public static void Warn(string msg)
        {
            Log(LogLevel.WARN, msg, null);
        }

        public static void Warn(string msg, params object[] args)
        {
            Log(LogLevel.WARN, string.Format(msg, args), null);
        }

        public static void Info(string msg)
        {
            Log(LogLevel.INFO, msg, null);
        }


        public static void Info(string msg, params object[] args)
        {
            Log(LogLevel.INFO, string.Format(msg, args), null);
        }

        public static void Debug(string msg)
        {
            Log(LogLevel.DEBUG, msg, null);
        }

        public static void Debug(string msg, params object[] args)
        {
            Log(LogLevel.DEBUG, string.Format(msg, args), null);
        }

        public static void Trace(string msg)
        {
            Log(LogLevel.TRACE, msg, null);
        }

        public static void Trace(string msg, params object[] args)
        {
            Log(LogLevel.TRACE, string.Format(msg, args), null);
        }
        
        private static void Log(LogLevel level, string msg, Exception e)
        {
            if (!Enable)
            {
                return;
            }

            try
            {
                var ev = new LogEvent
                {
                    DateTime = DateTime.Now,
                    Thread = Thread.CurrentThread.ManagedThreadId,
                    Level = level,
                    Message = msg,
                    Exception = e
                };
                foreach (var target in targets)
                {
                    target.OnLog(ev);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }

        public static void Clear()
        {
            foreach (var target in targets)
            {
                target.Clear();
            }
            targets.Clear();
        }
    }
}
