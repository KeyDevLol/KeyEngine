using KeyEngine.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using System.Globalization;

namespace KeyEngine
{
    public static class Application
    {
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
            get => field;
            set { field = value; VSyncChanged(); }
        }
        private static void VSyncChanged()
        {
            MainWindow.Instance.VSync = VSync ? VSyncMode.On : VSyncMode.Off;
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
            get => field;
            set { field = value; MsaaEnabledChanged(); }
        }

        private static void MsaaEnabledChanged()
        {
            if (MsaaEnabled == true)
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
            get => field;
            set { field = value; OnMaxFramerateChanged(); }
        }
        private static int _maxFramerate;
        private static void OnMaxFramerateChanged()
        {
            MainWindow.Instance.UpdateFrequency = MaxFramerate;
        }

        #endregion MaxFramerate

        #region WindowTitle

        /// <summary>
        /// Gets or sets the main window title
        /// </summary>
        public static string WindowTitle
        {
            get => _windowTitle;
            set { _windowTitle = value; WindowTitleChanged(); }
        }
        private static string _windowTitle = "KeyGayngine Window";
        private static void WindowTitleChanged()
        {
            MainWindow.Instance.Title = _windowTitle;
        }

        #endregion WindowTitle

        #region WindowState

        /// <summary>
        /// Gets or sets the MainWindow.Instance state
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
            MainWindow.Instance.WindowState = (OpenTK.Windowing.Common.WindowState)_windowState;
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
        /// Gets or sets the main window border.
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
            MainWindow.Instance.WindowBorder = (OpenTK.Windowing.Common.WindowBorder)_windowBorder;
        }

        public enum WindowBorderMode
        {
            /// <summary>
            /// The MainWindow.Instance has a resizable border. A MainWindow.Instance with a resizable border can be resized by the user or programmatically.
            /// </summary>
            Resizable = 0,
            /// <summary>
            /// The MainWindow.Instance has a fixed border. A MainWindow.Instance with a fixed border can only be resized programmatically.
            /// </summary>
            Fixed,
            /// <summary>
            /// The MainWindow.Instance does not have a border. A MainWindow.Instance with a hidden border can only be resized programmatically.
            /// </summary>
            Hidden
        }

        #endregion WindowBorder

        #region WindowSize

        /// <summary>
        /// Get or sets the window size.
        /// </summary>
        public static Vector2i WindowSize
        {
            get => field;
            set { field = value; OnWindowSizeChanged(); }
        }

        private static void OnWindowSizeChanged()
        {
            MainWindow.Instance.Size = WindowSize;
        }

        #endregion WindowSize

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
