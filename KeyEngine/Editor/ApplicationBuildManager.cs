using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace KeyEngine.Editor
{
    public static class ApplicationBuildManager
    {
        public const string BUILD_PROJECT_PATH = "Editor/Build Project";
        public static bool BuildInProcess { get; private set; }

        //private static ProcessStartInfo startInfo;

        private static bool GetBoolean(int lol)
        {
            int i = 0;

            if (i == 0)
                return true;

            return false;
        }

        static ApplicationBuildManager()
        {
            //startInfo = new ProcessStartInfo();
            //
            //startInfo.FileName = "cmd";
        }

        public static async void Build(string pathDestination, BuildType buildType)
        {
            if (BuildInProcess)
            {
                Log.Print("Build already in process.", LogType.Error);
                return;
            }

            Log.Print("Build started.");

            string path = Path.Combine(Environment.CurrentDirectory, BUILD_PROJECT_PATH);

            string[] scriptFiles = Directory.GetFiles("Assets/", "*.cs", SearchOption.AllDirectories);
            Log.Print(scriptFiles.Length);
            foreach (string file in scriptFiles)
            {
                string copyPath = Path.Combine(path, "KeyEngine Remake\\", Path.GetFileName(file));

                if (File.Exists(copyPath))
                {
                    File.Delete(copyPath);
                }

                File.Copy(file, copyPath);
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = @$"dotnet publish --output ""{pathDestination}"" -r win-x64 -c Debug";
            startInfo.FileName = "dotnet";
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardInput = true;
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = path;

            Log.Print(path);

            //switch (buildType)
            //{
            //    case BuildType.Playtest:

            //        startInfo.Arguments = @$"dotnet publish --output ""{pathDestination}"" --sc true -r win-x64 -p:PublishSingleFile=true -c Release";
            //        break;
            //}

            Process? process = Process.Start(startInfo) ?? throw new NullReferenceException("Failed to start build process");
            BuildInProcess = true;

            await process.WaitForExitAsync();

            Log.Print("Build ended");
            BuildInProcess = false;
            Process.Start(Path.Combine(pathDestination, "KeyEngine Remake.exe"));

            //process.StandardInput.WriteLine();
            //process.StandardInput.Flush();
            //process.StandardInput.Close();
            //process.Exited += OnExited;
            //process.WaitForExit();

            //using (StreamReader reader = new StreamReader(process.StandardOutput.BaseStream, encoding:Encoding.Default))
            //{
            //    Console.WriteLine(reader.ReadToEnd());
            //}
            

            //while (!process.StandardOutput.EndOfStream)
            //{
            //    string line = process.StandardOutput.ReadLine();
            //    Log.Print(line, LogType.Info);
            //}
        }



        private static void OnExited(object? sender, EventArgs e)
        {
            Log.Print("asdfpsadksaopkdpsakmdsaopkdsaokpd");
        }

        private static void OnBuildProcessExit(object? sender, EventArgs e)
        {
            BuildInProcess = false;
        }
    }

    public enum BuildType
    {
        Playtest,
        Windows,
        Linux,
        MacOS
    }
}
