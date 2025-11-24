using KeyEngine.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace KeyEngine
{
    /// <summary>
    /// Manages keyboard, mouse and other input devices.
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// Called when a character is typed on the keyboard.
        /// </summary>
        public static Action<char>? OnTextInput { get; set; }
#if ENABLE_EDITOR
        public static bool EnableInput { get; set; } = true;
#endif // ENABLE_EDITOR

        static Input()
        {
            MainWindow.Instance.TextInput += TextInputed;
        }

        #region Keyboard

        /// <summary>
        /// Gets a <see cref="bool" /> indicating whether this key is currently down.
        /// </summary>
        /// <param name="key">The <see cref="KeyCode">key</see> to check.</param>
        /// <returns><c>true</c> if <paramref name="keyCode"/> is in the down state; otherwise, <c>false</c>.</returns>
        public static bool IsKeyDown(KeyCode keyCode)
        {
            return MainWindow.Instance.IsKeyDown((Keys)keyCode);
        }

        /// <summary>
        /// Gets whether the specified key is pressed in the current frame but released in the previous frame.
        /// </summary>
        /// <param name="keyCode">The <see cref="KeyCode">key</see> to check.</param>
        /// <returns>True if the key is pressed in this frame, but not the last frame.</returns>
        public static bool IsKeyPressed(KeyCode keyCode)
        {
            return MainWindow.Instance.IsKeyPressed((Keys)keyCode);
        }

        /// <summary>
        ///     Gets whether the specified key is released in the current frame but pressed in the previous frame.
        /// </summary>
        /// <param name="keyCode">The <see cref="KeyCode">key</see> to check.</param>
        /// <returns>True if the key is released in this frame, but pressed the last frame.</returns>
        public static bool IsKeyReleased(KeyCode keyCode)
        {
            return MainWindow.Instance.IsKeyReleased((Keys)keyCode);
        }

        /// <summary>
        /// Gets raw axis value (-minMax, 0, or +minMax) from two opposing keys.
        /// </summary>
        /// <param name="negativeKey">Negative value KeyCode</param>
        /// <param name="positiveKey">Positive value KeyCode</param>
        /// <param name="minMax">Positive and negative value limit</param>
        /// <returns>Axis</returns>
        public static float GetAxisRaw(KeyCode negativeKey, KeyCode positiveKey, float minMax = 1)
        {
            if (IsKeyDown(negativeKey) && !IsKeyDown(positiveKey))
                return -minMax;
            else if (IsKeyDown(positiveKey) && !IsKeyDown(negativeKey))
                return minMax;

            return 0;
        }

        /// <summary>
        /// Gets raw axis value (-minMax, 0, or +minMax) from two opposing keys.
        /// </summary>
        /// <param name="negativeKey">Negative value KeyCode</param>
        /// <param name="positiveKey">Positive value KeyCode</param>
        /// <param name="axis">Current axis value</param>
        /// <param name="minMax">Positive and negative value limit</param>
        public static void GetAxisRaw(KeyCode negativeKey, KeyCode positiveKey, ref float axis, float minMax = 1)
        {
            if (IsKeyDown(negativeKey) && !IsKeyDown(positiveKey))
                axis = -minMax;
            else if (IsKeyDown(positiveKey) && !IsKeyDown(negativeKey))
                axis = minMax;

            axis = 0;
        }

        /// <summary>
        /// Smoothly interpolates axis value towards target based on key states.
        /// </summary>
        /// <param name="negativeKey">Key for negative direction</param>
        /// <param name="positiveKey">Key for positive direction</param>
        /// <param name="axis">Current axis value</param>
        /// <param name="acceleration">Interpolation speed (0-1)</param>
        /// <param name="minMax">Maximum axis value</param>
        /// <returns>Interpolated axis value</returns>
        public static float GetAxis(KeyCode negativeKey, KeyCode positiveKey, float axis, float acceleration = 0.2f, float minMax = 1)
        {
            if (IsKeyDown(negativeKey) && !IsKeyDown(positiveKey))
                return Mathf.Lerp(axis, -minMax, acceleration);
            else if (IsKeyDown(positiveKey) && !IsKeyDown(negativeKey))
                return Mathf.Lerp(axis, minMax, acceleration);

            return Mathf.Lerp(axis, 0, acceleration);
        }

        /// <summary>
        /// Smoothly interpolates axis value towards target based on key states.
        /// </summary>
        /// <param name="negativeKey">Key for negative direction</param>
        /// <param name="positiveKey">Key for positive direction</param>
        /// <param name="axis">Current axis value</param>
        /// <param name="acceleration">Interpolation speed (0-1)</param>
        /// <param name="minMax">Maximum axis value</param>
        /// <returns>Interpolated axis value</returns>
        public static void GetAxis(KeyCode negativeKey, KeyCode positiveKey, ref float axis, float acceleration = 0.2f, float minMax = 1)
        {
            if (IsKeyDown(negativeKey) && !IsKeyDown(positiveKey))
                axis = Mathf.Lerp(axis, -minMax, acceleration);
            else if (IsKeyDown(positiveKey) && !IsKeyDown(negativeKey))
                axis = Mathf.Lerp(axis, minMax, acceleration);

            axis = Mathf.Lerp(axis, 0, acceleration);
        }

        /// <summary>
        /// Parses string into KeyCode enum.
        /// </summary>
        /// <param name="str">String representation of KeyCode</param>
        /// <returns>Parsed KeyCode value</returns>
        public static KeyCode GetKeyCodeFromString(string str)
        {
            return Enum.Parse<KeyCode>(str);
        }

        private static void TextInputed(OpenTK.Windowing.Common.TextInputEventArgs e)
        {
            OnTextInput?.Invoke((char)e.Unicode);
        }

        /// <summary>
        /// Returns true if at least one key is pressed.
        /// </summary>
        public static bool IsAnyKeyDown => MainWindow.Instance.KeyboardState.IsAnyKeyDown;

        /// <summary>
        /// Gets or sets the clipboard string.
        /// </summary>
        public static string Clipboard
        {
            get { return MainWindow.Instance.ClipboardString; }
            set { MainWindow.Instance.ClipboardString = value; }
        }

        #endregion Keyboard

        #region Mouse

        /// <summary>
        /// Gets a <see cref="bool" /> indicating whether this button is currently down.
        /// </summary>
        /// <param name="mouseButton">The <see cref="MouseButtonCode" /> to check.</param>
        /// <returns><c>true</c> if <paramref name="mouseButton"/> is in the down state; otherwise, <c>false</c>.</returns>
        public static bool IsMouseButtonDown(MouseButtonCode mouseButton)
        {
            return MainWindow.Instance.IsMouseButtonDown((MouseButton)mouseButton);
        }

        /// <summary>
        /// Gets whether the specified mouse button is pressed in the current frame but released in the previous frame.
        /// </summary>
        /// <param name="mouseButton">The <see cref="MouseButtonCode"/> to check.</param>
        /// <returns>True if the <paramref name="mouseButton"/> is pressed in this frame, but not the last frame.</returns>
        public static bool IsMouseButtonPressed(MouseButtonCode mouseButton)
        {
#if ENABLE_EDITOR
            return MainWindow.Instance.IsMouseButtonPressed((MouseButton)mouseButton) && EnableInput;
#else
            return MainWindow.Instance.IsMouseButtonPressed((MouseButton)mouseButton);
#endif
        }

        /// <summary>
        /// Gets whether the specified mouse button is released in the current frame but pressed in the previous frame.
        /// </summary>
        /// <param name="mouseButton">The <see cref="MouseButtonCode"/> to check.</param>
        /// <returns>True if the <paramref name="mouseButton"/> is released in this frame, but pressed the last frame.</returns>
        public static bool IsMouseButtonReleased(MouseButtonCode mouseButton)
        {
            return MainWindow.Instance.IsMouseButtonReleased((MouseButton)mouseButton);
        }

        /// <summary>
        /// Get a Vector2 representing the amount that the mouse wheel moved since the last frame.
        /// </summary>
        public static Vector2 MouseScrollDelta => MainWindow.Instance.MouseState.ScrollDelta;

        /// <summary>
        /// Gets a <see cref="Vector2"/> representing the absolute position of the pointer
        /// in the current frame, relative to the top-left corner of the contents of the window.
        /// </summary>
        public static Vector2 MousePosition => MainWindow.Instance.MouseState.Position;

        /// <summary>
        /// Gets a <see cref="Vector2"/> representing the amount that the mouse moved since the last frame.
        /// This does not necessarily correspond to pixels, for example in the case of raw input.
        /// </summary>
        public static Vector2 MousePositionDelta => MainWindow.Instance.MouseState.Delta;

        /// <summary>
        /// Gets or sets the cursor state of the windows cursor.
        /// </summary>
        public static CursorState CursorState
        {
            get { return (CursorState)MainWindow.Instance.CursorState; }
            set { MainWindow.Instance.CursorState = (OpenTK.Windowing.Common.CursorState)value; }
        }

        /// <summary>
        /// Gets a value indicating whether any button is down.
        /// </summary>
        /// <value><c>true</c> if any button is down; otherwise, <c>false</c>.</value>
        public static bool IsAnyMouseButtonPressed => MainWindow.Instance.MouseState.IsAnyButtonDown;

        #endregion Mouse

        #region Other

        /// <summary>
        /// Called when files are dropped onto the application window.
        /// </summary>
        public static event Action<string[]>? OnFileDropped;

        #endregion Other
    }

    #region Enums

    /// <summary>
    /// Mouse button codes
    /// </summary>
    public enum MouseButtonCode
    {
        Button1 = 0,
        Left = 0,
        Button2 = 1,
        Right = 1,
        Button3 = 2,
        Middle = 2,
        Button4 = 3,
        Button5 = 4,
        Button6 = 5,
        Button7 = 6,
        Button8 = 7,
        Last = 7
    }

    /// <summary>
    /// Mouse cursor state
    /// </summary>
    public enum CursorState : byte
    {
        Normal = 0,
        Hidden = 1,
        Locked = 2
    }

    #endregion Enums
}
