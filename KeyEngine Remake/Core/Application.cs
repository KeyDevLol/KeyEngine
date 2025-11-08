using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using System.Globalization;

namespace KeyEngine
{
    public static class Application
    {
        private static readonly MainWindow window = MainWindow.Instance;

        #region RunInBackground

        /// <summary>
        /// Default is true
        /// </summary>
        public static bool RunInBackground { get; set; } = true;

        #endregion // RunInBackground

        #region VSync

        public static bool VSync
        {
            get => _vSync;
            set { _vSync = value; VSyncChanged(); }
        }
        private static bool _vSync = true;
        private static void VSyncChanged()
        {
            window.VSync = _vSync ? VSyncMode.On : VSyncMode.Off;
        }

        #endregion // VSync

        #region MsaaEnabled

        public static bool MsaaEnabled 
        {
            get => _msaaEnabled;
            set { _msaaEnabled = value; MsaaEnabledChanged(); }
        }
        private static bool _msaaEnabled = true;

        private static void MsaaEnabledChanged()
        {
            if (_msaaEnabled == true)
                GL.Enable(EnableCap.Multisample);
            else
                GL.Disable(EnableCap.Multisample);
        }

        #endregion // MsaaEnabled

        #region MsaaSamplesCount

        public const MsaaSamples MsaaSamplesCount = MsaaSamples.SamplesX2;
        public enum MsaaSamples
        {
            SamplesX2 = 2,
            SamplesX4 = 4,
            SamplesX8 = 8,
            SamplesX16 = 16,
        }

        #endregion // MsaaSamplesCount

        #region MaxFramerate

        public static int MaxFramerate
        {
            get => _maxFramerate;
            set { _maxFramerate = value; OnMaxFramerateChanged(); }
        }
        private static int _maxFramerate;
        private static void OnMaxFramerateChanged()
        {
            window.UpdateFrequency = _maxFramerate;
        }

        #endregion // MaxFramerate

        #region WindowTitle

        public static string WindowTitle
        {
            get => _windowTitle;
            set { _windowTitle = value; WindowTitleChanged(); }
        }
        private static string _windowTitle = "KeyGayngine Window";
        private static void WindowTitleChanged()
        {
            window.Title = _windowTitle;
        }

        #endregion // WindowTitle

        #region WindowState

        public static WindowStateMode WindowState
        {
            get => _windowState;
            set { _windowState = value; WindowStateChanged(); }
        }
        private static WindowStateMode _windowState;
        private static void WindowStateChanged()
        {
            window.WindowState = (OpenTK.Windowing.Common.WindowState)_windowState;
        }
        public enum WindowStateMode
        {
            Normal = 0,
            Minimized,
            Maximized,
            Fullscreen,
        }

        #endregion // WindowState

        #region WindowBorder

        public static WindowBorderMode WindowBorder
        {
            get => _windowBorder;
            set { _windowBorder = value; WindowBorderChanged(); }
        }
        private static WindowBorderMode _windowBorder;
        private static void WindowBorderChanged()
        {
            window.WindowBorder = (OpenTK.Windowing.Common.WindowBorder)_windowBorder;
        }
        public enum WindowBorderMode
        {
            Resizable = 0,
            Fixed,
            Hidden
        }

        #endregion // WindowBorder

        #region CurrentOS

        public static CurrentOSEnum CurrentOS
        {
            get
            {
                if (OperatingSystem.IsWindows())
                    return CurrentOSEnum.Windows;
                else if (OperatingSystem.IsLinux())
                    return CurrentOSEnum.Linux;
                else if (OperatingSystem.IsMacOS())
                    return CurrentOSEnum.MacOS;
                else if (OperatingSystem.IsFreeBSD())
                    return CurrentOSEnum.FreeBSD;

                // How??
                return CurrentOSEnum.Undefined;
            }
        }
        public enum CurrentOSEnum
        {
            Undefined,
            Windows,
            Linux,
            MacOS,
            FreeBSD
        }

        #endregion // CurrentOS

        public static CultureInfo SystemLanguage => CultureInfo.CurrentUICulture;
    }
}
