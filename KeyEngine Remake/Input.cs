using OpenTK.Windowing.GraphicsLibraryFramework;

namespace KeyEngine
{
    /// <summary>
    /// Provides access to keyboard and mouse input
    /// </summary>
    public static class Input
    {
        /// <summary>
        /// Called when something is typed on the keyboard
        /// </summary>
        public static Action<TextInputArgs> OnTextInput;
#if ENABLE_EDITOR
        public static bool enableInput = true;
#endif // ENABLE_EDITOR

        public static void Init()
        {
            MainWindow.Instance.TextInput += TextInputed;
        }

        #region Keyboard

        /// <summary>
        /// Checks if a specified key is in the down state.
        /// </summary>
        public static bool IsKeyDown(KeyCode keyCode)
        {
            return MainWindow.Instance.IsKeyDown((Keys)keyCode);
        }

        /// <summary>
        /// Checks if a specified key has been pressed since the last update.
        /// </summary>
        public static bool IsKeyPressed(KeyCode keyCode)
        {
            return MainWindow.Instance.IsKeyPressed((Keys)keyCode);
        }

        /// <summary>
        /// Checks if a specified key has been released since the last update.
        /// </summary>
        public static bool IsKeyReleased(KeyCode keyCode)
        {
            return MainWindow.Instance.IsKeyReleased((Keys)keyCode);
        }

        /// <summary>
        /// Returns a value depending on the currently pressed key
        /// </summary>
        /// <param name="negativeKey">Negative value KeyCode</param>
        /// <param name="positiveKey">Positive value KeyCode</param>
        /// <param name="minMax">Positive and negative value limit</param>
        public static float GetAxisRaw(KeyCode negativeKey, KeyCode positiveKey, float minMax = 1)
        {
            if (IsKeyDown(negativeKey) && !IsKeyDown(positiveKey))
                return -minMax;
            else if (IsKeyDown(positiveKey) && !IsKeyDown(negativeKey))
                return minMax;

            return 0;
        }

        /// <summary>
        /// Returns a value depending on the currently pressed key
        /// </summary>
        /// <param name="negativeKey">Negative value KeyCode</param>
        /// <param name="positiveKey">Positive value KeyCode</param>
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
        /// Returns an interpolated value based on the currently pressed key
        /// </summary>
        /// <param name="negativeKey">Negative value KeyCode</param>
        /// <param name="positiveKey">Positive value KeyCode</param>
        /// <param name="minMax">Positive and negative value limit</param>
        public static float GetAxis(KeyCode negativeKey, KeyCode positiveKey, float axis, float acceleration = 0.2f, float minMax = 1)
        {
            if (IsKeyDown(negativeKey) && !IsKeyDown(positiveKey))
                return Mathf.Lerp(axis, -minMax, acceleration);
            else if (IsKeyDown(positiveKey) && !IsKeyDown(negativeKey))
                return Mathf.Lerp(axis, minMax, acceleration);

            return Mathf.Lerp(axis, 0, acceleration);
        }

        /// <summary>
        /// Returns an interpolated value based on the currently pressed key
        /// </summary>
        /// <param name="negativeKey">Negative value KeyCode</param>
        /// <param name="positiveKey">Positive value KeyCode</param>
        /// <param name="minMax">Positive and negative value limit</param>
        public static void GetAxis(KeyCode negativeKey, KeyCode positiveKey, ref float axis, float acceleration = 0.2f, float minMax = 1)
        {
            if (IsKeyDown(negativeKey) && !IsKeyDown(positiveKey))
                axis = Mathf.Lerp(axis, -minMax, acceleration);
            else if (IsKeyDown(positiveKey) && !IsKeyDown(negativeKey))
                axis = Mathf.Lerp(axis, minMax, acceleration);

            axis = Mathf.Lerp(axis, 0, acceleration);
        }

        private static void TextInputed(OpenTK.Windowing.Common.TextInputEventArgs e)
        {
            OnTextInput?.Invoke(new TextInputArgs(e.Unicode));
        }

        /// <summary>
        /// If any key is pressed
        /// </summary>
        public static bool isAnyKeyDown => MainWindow.Instance.KeyboardState.IsAnyKeyDown;

        /// <summary>
        /// Gets or sets the clipboard string
        /// </summary>
        public static string clipboard
        {
            get { return MainWindow.Instance.ClipboardString; }
            set { MainWindow.Instance.ClipboardString = value; }
        }

        #endregion // Keyboard

        #region Mouse
        /// <summary>
        /// The mouse button was pressed
        /// </summary>
        /// <param name="mouseButton">MouseButtonCode</param>
        /// <returns>If the mouse button was just pressed: true</returns>
        public static bool IsMouseButtonDown(MouseButtonCode mouseButton)
        {
#if ENABLE_EDITOR
            return MainWindow.Instance.IsMouseButtonPressed((MouseButton)mouseButton) && enableInput;
#else
            return MainWindow.Instance.IsMouseButtonPressed((MouseButton)mouseButton);
#endif
        }

        /// <summary>
        /// The mouse button was released
        /// </summary>
        /// <param name="mouseButton">MouseButtonCode</param>
        /// <returns>If the mouse button was just released: true</returns>
        public static bool IsMouseButtonUp(MouseButtonCode mouseButton)
        {
            return MainWindow.Instance.IsMouseButtonReleased((MouseButton)mouseButton);
        }

        /// <summary>
        /// The mouse button is held down
        /// </summary>
        /// <param name="mouseButton">MouseButtonCode</param>
        /// <returns>While the mouse button is held down: true</returns>
        public static bool IsMouseButtonHold(MouseButtonCode mouseButton)
        {
            return MainWindow.Instance.IsMouseButtonDown((MouseButton)mouseButton);
        }

        /// <summary>
        /// Mouse wheel scrolling delta
        /// </summary>
        public static Vector2 mouseScrollDelta => MainWindow.Instance.MouseState.ScrollDelta;
        /// <summary>
        /// Mouse cursor position in screen coordinates
        /// </summary>
        public static Vector2 mousePosition => MainWindow.Instance.MouseState.Position;
        /// <summary>
        /// Mouse movement delta
        /// </summary>
        public static Vector2 mousePositionDelta => MainWindow.Instance.MouseState.Delta;
        /// <summary>
        /// Mouse cursor state
        /// </summary>
        public static CursorState cursorState
        {
            get { return (CursorState)MainWindow.Instance.CursorState; }
            set { MainWindow.Instance.CursorState = (OpenTK.Windowing.Common.CursorState)value; }
        }
        /// <summary>
        /// If any mouse button is pressed
        /// </summary>
        public static bool isAnyMouseButtonDown => MainWindow.Instance.MouseState.IsAnyButtonDown;
#endregion // Mouse

        #region Other

        /// <summary>
        /// Called when files have been dropped into the program window
        /// </summary>
        public static Action<string[]>? onFileDropped;

        #endregion // Other
    }

    #region Enums
    /// <summary>
    /// Keyboard Key Codes
    /// </summary>
    public enum KeyCode
    {
        //
        // Сводка:
        //     An unknown key.
        Unknown = -1,
        //
        // Сводка:
        //     The spacebar key.
        Space = 32,
        //
        // Сводка:
        //     The apostrophe key.
        Apostrophe = 39,
        //
        // Сводка:
        //     The comma key.
        Comma = 44,
        //
        // Сводка:
        //     The minus key.
        Minus = 45,
        //
        // Сводка:
        //     The period key.
        Period = 46,
        //
        // Сводка:
        //     The slash key.
        Slash = 47,
        //
        // Сводка:
        //     The 0 key.
        D0 = 48,
        //
        // Сводка:
        //     The 1 key.
        D1 = 49,
        //
        // Сводка:
        //     The 2 key.
        D2 = 50,
        //
        // Сводка:
        //     The 3 key.
        D3 = 51,
        //
        // Сводка:
        //     The 4 key.
        D4 = 52,
        //
        // Сводка:
        //     The 5 key.
        D5 = 53,
        //
        // Сводка:
        //     The 6 key.
        D6 = 54,
        //
        // Сводка:
        //     The 7 key.
        D7 = 55,
        //
        // Сводка:
        //     The 8 key.
        D8 = 56,
        //
        // Сводка:
        //     The 9 key.
        D9 = 57,
        //
        // Сводка:
        //     The semicolon key.
        Semicolon = 59,
        //
        // Сводка:
        //     The equal key.
        Equal = 61,
        //
        // Сводка:
        //     The A key.
        A = 65,
        //
        // Сводка:
        //     The B key.
        B = 66,
        //
        // Сводка:
        //     The C key.
        C = 67,
        //
        // Сводка:
        //     The D key.
        D = 68,
        //
        // Сводка:
        //     The E key.
        E = 69,
        //
        // Сводка:
        //     The F key.
        F = 70,
        //
        // Сводка:
        //     The G key.
        G = 71,
        //
        // Сводка:
        //     The H key.
        H = 72,
        //
        // Сводка:
        //     The I key.
        I = 73,
        //
        // Сводка:
        //     The J key.
        J = 74,
        //
        // Сводка:
        //     The K key.
        K = 75,
        //
        // Сводка:
        //     The L key.
        L = 76,
        //
        // Сводка:
        //     The M key.
        M = 77,
        //
        // Сводка:
        //     The N key.
        N = 78,
        //
        // Сводка:
        //     The O key.
        O = 79,
        //
        // Сводка:
        //     The P key.
        P = 80,
        //
        // Сводка:
        //     The Q key.
        Q = 81,
        //
        // Сводка:
        //     The R key.
        R = 82,
        //
        // Сводка:
        //     The S key.
        S = 83,
        //
        // Сводка:
        //     The T key.
        T = 84,
        //
        // Сводка:
        //     The U key.
        U = 85,
        //
        // Сводка:
        //     The V key.
        V = 86,
        //
        // Сводка:
        //     The W key.
        W = 87,
        //
        // Сводка:
        //     The X key.
        X = 88,
        //
        // Сводка:
        //     The Y key.
        Y = 89,
        //
        // Сводка:
        //     The Z key.
        Z = 90,
        //
        // Сводка:
        //     The left bracket(opening bracket) key.
        LeftBracket = 91,
        //
        // Сводка:
        //     The backslash.
        Backslash = 92,
        //
        // Сводка:
        //     The right bracket(closing bracket) key.
        RightBracket = 93,
        //
        // Сводка:
        //     The grave accent key.
        GraveAccent = 96,
        //
        // Сводка:
        //     The escape key.
        Escape = 256,
        //
        // Сводка:
        //     The enter key.
        Enter = 257,
        //
        // Сводка:
        //     The tab key.
        Tab = 258,
        //
        // Сводка:
        //     The backspace key.
        Backspace = 259,
        //
        // Сводка:
        //     The insert key.
        Insert = 260,
        //
        // Сводка:
        //     The delete key.
        Delete = 261,
        //
        // Сводка:
        //     The right arrow key.
        Right = 262,
        //
        // Сводка:
        //     The left arrow key.
        Left = 263,
        //
        // Сводка:
        //     The down arrow key.
        Down = 264,
        //
        // Сводка:
        //     The up arrow key.
        Up = 265,
        //
        // Сводка:
        //     The page up key.
        PageUp = 266,
        //
        // Сводка:
        //     The page down key.
        PageDown = 267,
        //
        // Сводка:
        //     The home key.
        Home = 268,
        //
        // Сводка:
        //     The end key.
        End = 269,
        //
        // Сводка:
        //     The caps lock key.
        CapsLock = 280,
        //
        // Сводка:
        //     The scroll lock key.
        ScrollLock = 281,
        //
        // Сводка:
        //     The num lock key.
        NumLock = 282,
        //
        // Сводка:
        //     The print screen key.
        PrintScreen = 283,
        //
        // Сводка:
        //     The pause key.
        Pause = 284,
        //
        // Сводка:
        //     The F1 key.
        F1 = 290,
        //
        // Сводка:
        //     The F2 key.
        F2 = 291,
        //
        // Сводка:
        //     The F3 key.
        F3 = 292,
        //
        // Сводка:
        //     The F4 key.
        F4 = 293,
        //
        // Сводка:
        //     The F5 key.
        F5 = 294,
        //
        // Сводка:
        //     The F6 key.
        F6 = 295,
        //
        // Сводка:
        //     The F7 key.
        F7 = 296,
        //
        // Сводка:
        //     The F8 key.
        F8 = 297,
        //
        // Сводка:
        //     The F9 key.
        F9 = 298,
        //
        // Сводка:
        //     The F10 key.
        F10 = 299,
        //
        // Сводка:
        //     The F11 key.
        F11 = 300,
        //
        // Сводка:
        //     The F12 key.
        F12 = 301,
        //
        // Сводка:
        //     The F13 key.
        F13 = 302,
        //
        // Сводка:
        //     The F14 key.
        F14 = 303,
        //
        // Сводка:
        //     The F15 key.
        F15 = 304,
        //
        // Сводка:
        //     The F16 key.
        F16 = 305,
        //
        // Сводка:
        //     The F17 key.
        F17 = 306,
        //
        // Сводка:
        //     The F18 key.
        F18 = 307,
        //
        // Сводка:
        //     The F19 key.
        F19 = 308,
        //
        // Сводка:
        //     The F20 key.
        F20 = 309,
        //
        // Сводка:
        //     The F21 key.
        F21 = 310,
        //
        // Сводка:
        //     The F22 key.
        F22 = 311,
        //
        // Сводка:
        //     The F23 key.
        F23 = 312,
        //
        // Сводка:
        //     The F24 key.
        F24 = 313,
        //
        // Сводка:
        //     The F25 key.
        F25 = 314,
        //
        // Сводка:
        //     The 0 key on the key pad.
        KeyPad0 = 320,
        //
        // Сводка:
        //     The 1 key on the key pad.
        KeyPad1 = 321,
        //
        // Сводка:
        //     The 2 key on the key pad.
        KeyPad2 = 322,
        //
        // Сводка:
        //     The 3 key on the key pad.
        KeyPad3 = 323,
        //
        // Сводка:
        //     The 4 key on the key pad.
        KeyPad4 = 324,
        //
        // Сводка:
        //     The 5 key on the key pad.
        KeyPad5 = 325,
        //
        // Сводка:
        //     The 6 key on the key pad.
        KeyPad6 = 326,
        //
        // Сводка:
        //     The 7 key on the key pad.
        KeyPad7 = 327,
        //
        // Сводка:
        //     The 8 key on the key pad.
        KeyPad8 = 328,
        //
        // Сводка:
        //     The 9 key on the key pad.
        KeyPad9 = 329,
        //
        // Сводка:
        //     The decimal key on the key pad.
        KeyPadDecimal = 330,
        //
        // Сводка:
        //     The divide key on the key pad.
        KeyPadDivide = 331,
        //
        // Сводка:
        //     The multiply key on the key pad.
        KeyPadMultiply = 332,
        //
        // Сводка:
        //     The subtract key on the key pad.
        KeyPadSubtract = 333,
        //
        // Сводка:
        //     The add key on the key pad.
        KeyPadAdd = 334,
        //
        // Сводка:
        //     The enter key on the key pad.
        KeyPadEnter = 335,
        //
        // Сводка:
        //     The equal key on the key pad.
        KeyPadEqual = 336,
        //
        // Сводка:
        //     The left shift key.
        LeftShift = 340,
        //
        // Сводка:
        //     The left control key.
        LeftControl = 341,
        //
        // Сводка:
        //     The left alt key.
        LeftAlt = 342,
        //
        // Сводка:
        //     The left super key.
        LeftSuper = 343,
        //
        // Сводка:
        //     The right shift key.
        RightShift = 344,
        //
        // Сводка:
        //     The right control key.
        RightControl = 345,
        //
        // Сводка:
        //     The right alt key.
        RightAlt = 346,
        //
        // Сводка:
        //     The right super key.
        RightSuper = 347,
        //
        // Сводка:
        //     The menu key.
        Menu = 348,
        //
        // Сводка:
        //     The last valid key in this enum.
        LastKey = 348
    }

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
    #endregion //Enums

    #region Structures
    /// <summary>
    /// Occurs when a symbol is entered on the keyboard
    /// </summary>
    public readonly struct TextInputArgs
    {
        public int Unicode { get; }

        public TextInputArgs(int unicode)
        {
            Unicode = unicode;
        }

        public string AsString => char.ConvertFromUtf32(Unicode);
    }
    #endregion //Structures
}
