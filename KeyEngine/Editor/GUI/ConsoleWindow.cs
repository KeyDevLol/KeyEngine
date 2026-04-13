using ImGuiNET;
using System.Diagnostics;
using System.Numerics;

namespace KeyEngine.Editor.GUI
{
    public class ConsoleWindow : EditorWindow
    {
        private List<LogMessage> logMessages = [];
        private string output = "Lol";

        public ConsoleWindow()
        {
            this.Title = "Console";
            Log.OnMessageReceived += OnLogMessageReceived;
        }

        public override void Render()
        {
            if (ImGui.IsKeyDown(ImGuiKey.H))
            {
                Log.Print("HHHH");
            }

            if (ImGui.Button("Info"))
                Log.Print("Info", LogType.Info);
            if (ImGui.Button("Warning"))
                Log.Print("Warning", LogType.Warning);
            if (ImGui.Button("Error"))
                Log.Print("Error", LogType.Error);
            if (ImGui.Button("FatalError"))
                Log.Print("FatalError", LogType.FatalError);

            //ImGui.InputTextMultiline("sadd", ref output, 213, new Vector2(ImGui.GetWindowWidth() - 10, 1000));


            for (int i = 0; i < logMessages.Count; i++)
            {
                LogMessage logMessage = logMessages[i];
                ImGui.Text(logMessage.Message);
            }
        }
        private void OnLogMessageReceived(string? message, LogType logType)
        {
            logMessages.Add(new LogMessage(message, logType));
        }

        public class LogMessage
        {
            public string? Message;
            public LogType LogType;
            public IEnumerable<StackFrame> StackFrames;

            public LogMessage(string? message, LogType logType)
            {
                Message = message;
                LogType = logType;
                StackFrames = new StackTrace(true).GetFrames();
            }
        }
    }
}
