using KeyEngine.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Windowing.Common;
using System.Globalization;
using System.Runtime.InteropServices;

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
        } = true;
        private static void VSyncChanged()
        {
            MainWindow.Instance?.VSync = VSync ? VSyncMode.On : VSyncMode.Off;
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
        = false;

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
        private static void OnMaxFramerateChanged()
        {
            MainWindow.Instance?.UpdateFrequency = MaxFramerate;
        }

        #endregion MaxFramerate

        #region WindowTitle

        /// <summary>
        /// Gets or sets the main window title
        /// </summary>
        public static string WindowTitle
        {
            get => field;
            set { field = value; WindowTitleChanged(); }
        } = "KeyEngine 5";
        private static void WindowTitleChanged()
        {
            MainWindow.Instance?.Title = WindowTitle;
        }

        #endregion WindowTitle

        #region WindowState

        /// <summary>
        /// Gets or sets the MainWindow.Instance state
        /// Default value: <see cref="WindowStateEnum.Normal"/>.
        /// </summary>
        public static WindowStateEnum WindowState
        {
            get => field;
            set { field = value; WindowStateChanged(); }
        }
        private static void WindowStateChanged()
        {
            MainWindow.Instance?.WindowState = (WindowState)WindowState;
        }
        public enum WindowStateEnum
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
        /// Default value: <see cref="WindowBorderEnum.Resizable"/>.
        /// </summary>
        public static WindowBorderEnum WindowBorder
        {
            get => field;
            set { field = value; WindowBorderChanged(); }
        }
        private static void WindowBorderChanged()
        {
            MainWindow.Instance?.WindowBorder = (WindowBorder)WindowBorder;
        }

        public enum WindowBorderEnum
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
        public static Vector2Int WindowSize
        {
            get => field;
            set { field = value; OnWindowSizeChanged(); }
        }

        private static void OnWindowSizeChanged()
        {
            MainWindow.Instance?.Size = WindowSize;
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
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return CurrentOSEnum.Windows;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    return CurrentOSEnum.Linux;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    return CurrentOSEnum.OSX;
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
                    return CurrentOSEnum.FreeBSD;

                // How??
                return CurrentOSEnum.Unidentified;
            }
        }
        public enum CurrentOSEnum
        {
            /// <summary>
            /// Unsupported or unidentified OS (how?)
            /// </summary>
            Unidentified,
            /// <summary>
            /// Windows OS
            /// </summary>
            Windows,
            /// <summary>
            /// Linux OS
            /// </summary>
            Linux,
            /// <summary>
            /// OSX
            /// </summary>
            OSX,
            /// <summary>
            /// FreeBSD OS
            /// </summary>
            FreeBSD
        }

        #endregion CurrentOS

        /// <summary>
        /// Gets current system language.
        /// </summary>
        public static CultureInfo SystemLanguage => CultureInfo.CurrentUICulture;
    }
}
