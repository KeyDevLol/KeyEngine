using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace KeyEngine
{
    /// <summary>
    /// Logger
    /// </summary>
    public static class Log
    {
        public static bool ShowTime { get; set; } = true;
        public static string TimeFormat { get; set; } = "H:mm:ss";

        public static bool LogToFile { get; set; } = true;
        private static StreamWriter? streamWriter = null;

        [Conditional("DEBUG")]
        public static void Print(string? value, LogType type = LogType.Info)
        {
            GetLogTypeInfo(type, out string suffix, out ConsoleColor color);

            if (color != ConsoleColor.Black)
            {
                Console.ForegroundColor = color;
                WriteLine(value, suffix);
                Console.ResetColor();
            }
            else
            {
                WriteLine(value, suffix);
            }

            if (type == LogType.FatalError)
                throw new Exception("FATAL_ERROR");
        }

        [Conditional("DEBUG")]
        public static void Print(object? value, LogType type = LogType.Info)
        {
            Print(value?.ToString(), type);
        }

        [Conditional("DEBUG")]
        public static void Assert([DoesNotReturnIf(false)] bool condition)
        {
            if (condition)
                return;

            Print("Assertion failed.", LogType.Error);
            throw new Exception();
        }

        [Conditional("DEBUG")]
        public static void Assert([DoesNotReturnIf(false)] bool condition, string? message, LogType logType)
        {
            if (condition)
                return;

            Print(message, logType == LogType.FatalError ? LogType.Error : logType);

            if (logType == LogType.FatalError)
                throw new Exception();
        }

        [Conditional("DEBUG")]
        private static void WriteLine(string? value, string? suffix)
        {
            string formatted = ShowTime ?
            /* True  */ string.Format("[{0}] {1}: {2}", DateTime.Now.ToString(TimeFormat), suffix, value) :
            /* False */ string.Format("{0}: {1}", suffix, value);

            Console.WriteLine(formatted);
            streamWriter?.WriteLine(formatted);
        }

        /// <summary>
        /// Очищает консоль
        /// </summary>
        [Conditional("DEBUG")]
        public static void Clear()
        {
            Console.Clear();
        }

        [Conditional("DEBUG")]
        public static void StartLogToFile(string path, bool clearFile = false)
        {
            if (clearFile)
                File.WriteAllText(path, string.Empty);

            streamWriter ??= new StreamWriter(File.OpenWrite(path));
        }

        [Conditional("DEBUG")]
        public static void StopLogToFile()
        {
            if (streamWriter != null)
            {
                streamWriter.Close();
                streamWriter.Dispose();
                streamWriter = null;
            }
        }

        private static void GetLogTypeInfo(LogType type, out string suffix, out ConsoleColor color)
        {
            color = ConsoleColor.Black;

            switch (type)
            {
                case LogType.Info:
                    suffix = "INFO";
                    break;
                case LogType.Warning:
                    suffix = "WARNING";
                    color = ConsoleColor.DarkYellow;
                    break;
                case LogType.Error:
                    suffix = "ERROR";
                    color = ConsoleColor.DarkRed;
                    break;
                case LogType.FatalError:
                    suffix = "FATAL_ERROR";
                    color = ConsoleColor.DarkRed;
                    break;
                default:
                    suffix = "PRINT";
                    break;
            }
        }
    }

    public enum LogType : byte
    {
        Info,
        Warning,
        Error,
        FatalError
    }
}
