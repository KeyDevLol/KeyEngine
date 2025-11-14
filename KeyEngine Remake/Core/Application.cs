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
        /// Determines whether the program should run in the background.
        /// Default value: <c>true</c>.
        /// </summary>
        public static bool RunInBackground { get; set; } = false;

        #endregion RunInBackground

        #region VSync

        /// <summary>
        /// Determines whether to enable Vertical Sync.
        /// Default value: <c>true</c>.
        /// </summary>
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

        #endregion VSync

        #region MsaaEnabled

        /// <summary>
        /// Determines whether MSAA antialiasing should be enabled.
        /// To set the number of samples use <see cref="MsaaSamplesCount"/>.
        /// Default value: <c>true</c>.
        /// </summary>
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

        #endregion MsaaEnabled

        #region MsaaSamplesCount

        /// <summary>
        /// Determines how many samples to use for MSAA antialiasing. 
        /// To enable MSAA antialiasing use <see cref="MsaaEnabled"/>.
        /// Cannot be changed at runtime.
        /// Default value: <see cref="MsaaSamples.SamplesX2"/>.
        /// </summary>
        public const MsaaSamples MsaaSamplesCount = MsaaSamples.SamplesX2;
        public enum MsaaSamples
        {
            SamplesX2 = 2,
            SamplesX4 = 4,
            SamplesX8 = 8,
            SamplesX16 = 16,
        }

        #endregion MsaaSamplesCount

        #region MaxFramerate

        /// <summary>
        /// Gets or sets the FPS limit.
        /// Set the value to 0 to remove the FPS limit.
        /// Default value: <c>0</c>
        /// </summary>
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

        #endregion MaxFramerate

        #region WindowTitle

        /// <summary>
        /// Gets or sets the window title
        /// </summary>
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

        #endregion WindowTitle

        #region WindowState

        /// <summary>
        /// Gets or sets the window state
        /// Default value: <see cref="WindowStateMode.Normal"/>.
        /// </summary>
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

        #endregion WindowState

        #region WindowBorder

        /// <summary>
        /// Gets or sets the window border.
        /// Default value: <see cref="WindowBorderMode.Resizable"/>.
        /// </summary>
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
            /// <summary>
            /// The window has a resizable border. A window with a resizable border can be resized by the user or programmatically.
            /// </summary>
            Resizable = 0,
            /// <summary>
            /// The window has a fixed border. A window with a fixed border can only be resized programmatically.
            /// </summary>
            Fixed,
            /// <summary>
            /// The window does not have a border. A window with a hidden border can only be resized programmatically.
            /// </summary>
            Hidden
        }

        #endregion WindowBorder

        #region CurrentOS

        /// <summary>
        /// Gets current Operating System platform.
        /// </summary>
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
                return CurrentOSEnum.Unidentified;
            }
        }
        public enum CurrentOSEnum
        {
            /// <summary>
            /// Unsupported (how?) or unidentified OS
            /// </summary>
            Unidentified,
            Windows,
            Linux,
            MacOS,
            FreeBSD
        }

        #endregion CurrentOS

        /// <summary>
        /// Gets current system language.
        /// </summary>
        public static CultureInfo SystemLanguage => CultureInfo.CurrentUICulture;
    }
}
