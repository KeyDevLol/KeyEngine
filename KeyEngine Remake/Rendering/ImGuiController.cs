using ImGuiNET;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;
using ErrorCode = OpenTK.Graphics.OpenGL4.ErrorCode;
using NUMVector2 = System.Numerics.Vector2;

namespace KeyEngine.Rendering
{
    public class ImGuiController : IDisposable
    {
        private bool frameIsBegun;

        private int vertexArray;
        private int vertexBuffer;
        private int vertexBufferSize;
        private int indexBuffer;
        private int indexBufferSize;

        private int fontTexture;

        private int shader;
        private int shaderFontTextureLocation;
        private int shaderProjectionMatrixLocation;

        private int windowWidth;
        private int windowHeight;

        private NUMVector2 scaleFactor = NUMVector2.One;

        private static bool khrDebugAvailable = false;

        private readonly int glVersion;
        private readonly bool compatibilityProfile;

        private readonly Dictionary<Keys, ImGuiKey> convertedImGuiKeys;

        /// <summary>
        /// Constructs a new ImGuiController.
        /// </summary>
        public ImGuiController(int width, int height)
        {
            windowWidth = width;
            windowHeight = height;

            convertedImGuiKeys = [];

            int major = GL.GetInteger(GetPName.MajorVersion);
            int minor = GL.GetInteger(GetPName.MinorVersion);

            glVersion = major * 100 + minor * 10;

            khrDebugAvailable = (major == 4 && minor >= 3) || IsExtensionSupported("KHR_debug");

            compatibilityProfile = (GL.GetInteger((GetPName)All.ContextProfileMask) & (int)All.ContextCompatibilityProfileBit) != 0;

            IntPtr context = ImGui.CreateContext();
            ImGui.SetCurrentContext(context);
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.AddFontDefault();

            io.BackendFlags |= ImGuiBackendFlags.RendererHasVtxOffset;

            CreateDeviceResources();
            InitializeKeys();

            SetPerFrameImGuiData(1f / 60f);
            io.ConfigFlags = ImGuiConfigFlags.DockingEnable;

            //ImGui.NewFrame();
            //_frameBegun = true;
        }

        ~ImGuiController()
        {
            GL.DeleteVertexArray(vertexArray);
            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteBuffer(indexBuffer);

            GL.DeleteTexture(fontTexture);
            GL.DeleteProgram(shader);
        }

        /// <summary>
        /// Frees all graphics resources used by the renderer.
        /// </summary>
        public void Dispose()
        {
            GL.DeleteVertexArray(vertexArray);
            GL.DeleteBuffer(vertexBuffer);
            GL.DeleteBuffer(indexBuffer);

            GL.DeleteTexture(fontTexture);
            GL.DeleteProgram(shader);
            GC.SuppressFinalize(this);
        }

        public void WindowResized(int width, int height)
        {
            windowWidth = width;
            windowHeight = height;
        }

        public void DestroyDeviceObjects()
        {
            Dispose();
        }

        public void CreateDeviceResources()
        {
            vertexBufferSize = 10000;
            indexBufferSize = 2000;

            int prevVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int prevArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);

            vertexArray = GL.GenVertexArray();
            GL.BindVertexArray(vertexArray);
            LabelObject(ObjectLabelIdentifier.VertexArray, vertexArray, "ImGui");

            vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            LabelObject(ObjectLabelIdentifier.Buffer, vertexBuffer, "VBO: ImGui");
            GL.BufferData(BufferTarget.ArrayBuffer, vertexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            LabelObject(ObjectLabelIdentifier.Buffer, indexBuffer, "EBO: ImGui");
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexBufferSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);

            RecreateFontDeviceTexture();

            string VertexSource = @"#version 330 core

uniform mat4 projection_matrix;

layout(location = 0) in vec2 in_position;
layout(location = 1) in vec2 in_texCoord;
layout(location = 2) in vec4 in_color;

out vec4 color;
out vec2 texCoord;

void main()
{
    gl_Position = projection_matrix * vec4(in_position, 0, 1);
    color = in_color;
    texCoord = in_texCoord;
}";
            string FragmentSource = @"#version 330 core

uniform sampler2D in_fontTexture;

in vec4 color;
in vec2 texCoord;

out vec4 outputColor;

void main()
{
    outputColor = color * texture(in_fontTexture, texCoord);
}";

            shader = CreateProgram("ImGui", VertexSource, FragmentSource);
            shaderProjectionMatrixLocation = GL.GetUniformLocation(shader, "projection_matrix");
            shaderFontTextureLocation = GL.GetUniformLocation(shader, "in_fontTexture");

            int stride = Unsafe.SizeOf<ImDrawVert>();
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, stride, 0);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, stride, 8);
            GL.VertexAttribPointer(2, 4, VertexAttribPointerType.UnsignedByte, true, stride, 16);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.BindVertexArray(prevVAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, prevArrayBuffer);

            CheckGLError("End of ImGui setup");
        }

        /// <summary>
        /// Recreates the device texture used to render text.
        /// </summary>
        public void RecreateFontDeviceTexture()
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.Fonts.GetTexDataAsRGBA32(out IntPtr pixels, out int width, out int height, out int bytesPerPixel);

            int mips = (int)Math.Floor(Math.Log(Math.Max(width, height), 2));

            int prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
            GL.ActiveTexture(TextureUnit.Texture0);
            int prevTexture2D = GL.GetInteger(GetPName.TextureBinding2D);

            fontTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, fontTexture);
            GL.TexStorage2D(TextureTarget2d.Texture2D, mips, SizedInternalFormat.Rgba8, width, height);
            LabelObject(ObjectLabelIdentifier.Texture, fontTexture, "ImGui Text Atlas");

            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Bgra, PixelType.UnsignedByte, pixels);

            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMaxLevel, mips - 1);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);

            // Restore state
            GL.BindTexture(TextureTarget.Texture2D, prevTexture2D);
            GL.ActiveTexture((TextureUnit)prevActiveTexture);

            io.Fonts.SetTexID((IntPtr)fontTexture);

            io.Fonts.ClearTexData();
        }

        /// <summary>
        /// Renders the ImGui draw list data.
        /// </summary>
        public void Render()
        {
            if (frameIsBegun)
            {
                frameIsBegun = false;
                ImGui.Render();
                RenderImDrawData(ImGui.GetDrawData());
            }
        }

        /// <summary>
        /// Updates ImGui input and IO configuration state.
        /// </summary>
        public void Update(GameWindow wnd, float deltaSeconds)
        {
            if (frameIsBegun)
            {
                ImGui.Render();
            }

            SetPerFrameImGuiData(deltaSeconds);
            UpdateImGuiInput(wnd);

            frameIsBegun = true;
            ImGui.NewFrame();
        }

        /// <summary>
        /// Sets per-frame data based on the associated window.
        /// This is called by Update(float).
        /// </summary>
        private void SetPerFrameImGuiData(float deltaSeconds)
        {
            ImGuiIOPtr io = ImGui.GetIO();
            io.DisplaySize = new System.Numerics.Vector2(
                windowWidth / scaleFactor.X,
                windowHeight / scaleFactor.Y);
            io.DisplayFramebufferScale = scaleFactor;
            io.DeltaTime = deltaSeconds; // DeltaTime is in seconds.
        }

        readonly List<char> PressedChars = new List<char>();

        private void UpdateImGuiInput(GameWindow wnd)
        {
            ImGuiIOPtr io = ImGui.GetIO();

            MouseState MouseState = wnd.MouseState;
            KeyboardState KeyboardState = wnd.KeyboardState;

            io.MouseDown[0] = MouseState[MouseButton.Left];
            io.MouseDown[1] = MouseState[MouseButton.Right];
            io.MouseDown[2] = MouseState[MouseButton.Middle];
            io.MouseDown[3] = MouseState[MouseButton.Button4];
            io.MouseDown[4] = MouseState[MouseButton.Button5];

            Vector2i screenPoint = new Vector2i((int)MouseState.X, (int)MouseState.Y);
            Vector2i point = screenPoint;//wnd.PointToClient(screenPoint);
            io.MousePos = new NUMVector2(point.X, point.Y);

            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (key == Keys.Unknown)
                {
                    continue;
                }
                if (convertedImGuiKeys.TryGetValue(key, out ImGuiKey imKey))
                    io.AddKeyEvent(imKey, KeyboardState.IsKeyDown(key));
            }

            foreach (char c in PressedChars)
            {
                io.AddInputCharacter(c);
            }
            PressedChars.Clear();

            io.AddKeyEvent(ImGuiKey.ModCtrl, KeyboardState.IsKeyDown(Keys.LeftControl) || KeyboardState.IsKeyDown(Keys.RightControl));
            io.AddKeyEvent(ImGuiKey.ModShift, KeyboardState.IsKeyDown(Keys.LeftShift) || KeyboardState.IsKeyDown(Keys.RightShift));
            io.AddKeyEvent(ImGuiKey.ModAlt, KeyboardState.IsKeyDown(Keys.LeftAlt) || KeyboardState.IsKeyDown(Keys.RightAlt));
            io.AddKeyEvent(ImGuiKey.ModSuper, KeyboardState.IsKeyDown(Keys.LeftSuper) || KeyboardState.IsKeyDown(Keys.RightSuper));
        }

        internal void PressChar(char keyChar)
        {
            PressedChars.Add(keyChar);
        }

        internal void MouseScroll(OpenTK.Mathematics.Vector2 offset)
        {
            ImGuiIOPtr io = ImGui.GetIO();

            io.MouseWheel = offset.Y;
            io.MouseWheelH = offset.X;
        }

        private void RenderImDrawData(ImDrawDataPtr draw_data)
        {
            if (draw_data.CmdListsCount == 0)
            {
                return;
            }

            // Get intial state.
            int prevVAO = GL.GetInteger(GetPName.VertexArrayBinding);
            int prevArrayBuffer = GL.GetInteger(GetPName.ArrayBufferBinding);
            int prevProgram = GL.GetInteger(GetPName.CurrentProgram);
            bool prevBlendEnabled = GL.GetBoolean(GetPName.Blend);
            bool prevScissorTestEnabled = GL.GetBoolean(GetPName.ScissorTest);
            int prevBlendEquationRgb = GL.GetInteger(GetPName.BlendEquationRgb);
            int prevBlendEquationAlpha = GL.GetInteger(GetPName.BlendEquationAlpha);
            int prevBlendFuncSrcRgb = GL.GetInteger(GetPName.BlendSrcRgb);
            int prevBlendFuncSrcAlpha = GL.GetInteger(GetPName.BlendSrcAlpha);
            int prevBlendFuncDstRgb = GL.GetInteger(GetPName.BlendDstRgb);
            int prevBlendFuncDstAlpha = GL.GetInteger(GetPName.BlendDstAlpha);
            bool prevCullFaceEnabled = GL.GetBoolean(GetPName.CullFace);
            bool prevDepthTestEnabled = GL.GetBoolean(GetPName.DepthTest);
            int prevActiveTexture = GL.GetInteger(GetPName.ActiveTexture);
            GL.ActiveTexture(TextureUnit.Texture0);
            int prevTexture2D = GL.GetInteger(GetPName.TextureBinding2D);
            Span<int> prevScissorBox = stackalloc int[4];
            unsafe
            {
                fixed (int* iptr = &prevScissorBox[0])
                {
                    GL.GetInteger(GetPName.ScissorBox, iptr);
                }
            }
            Span<int> prevPolygonMode = stackalloc int[2];
            unsafe
            {
                fixed (int* iptr = &prevPolygonMode[0])
                {
                    GL.GetInteger(GetPName.PolygonMode, iptr);
                }
            }

            if (glVersion <= 310 || compatibilityProfile)
            {
                GL.PolygonMode(TriangleFace.Front, PolygonMode.Fill);
                GL.PolygonMode(TriangleFace.Back, PolygonMode.Fill);
            }
            else
            {
                GL.PolygonMode(TriangleFace.FrontAndBack, PolygonMode.Fill);
            }

            // Bind the element buffer (thru the VAO) so that we can resize it.
            GL.BindVertexArray(vertexArray);
            // Bind the vertex buffer so that we can resize it.
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            for (int i = 0; i < draw_data.CmdListsCount; i++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdLists[i];

                int vertexSize = cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>();
                if (vertexSize > vertexBufferSize)
                {
                    int newSize = (int)Math.Max(vertexBufferSize * 1.5f, vertexSize);

                    GL.BufferData(BufferTarget.ArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    vertexBufferSize = newSize;
                }

                int indexSize = cmd_list.IdxBuffer.Size * sizeof(ushort);
                if (indexSize > indexBufferSize)
                {
                    int newSize = (int)Math.Max(indexBufferSize * 1.5f, indexSize);
                    GL.BufferData(BufferTarget.ElementArrayBuffer, newSize, IntPtr.Zero, BufferUsageHint.DynamicDraw);
                    indexBufferSize = newSize;
                }
            }

            // Setup orthographic projection matrix into our constant buffer
            ImGuiIOPtr io = ImGui.GetIO();
            Matrix4 mvp = Matrix4.CreateOrthographicOffCenter(
                0.0f,
                io.DisplaySize.X,
                io.DisplaySize.Y,
                0.0f,
                -1.0f,
                1.0f);

            GL.UseProgram(shader);
            GL.UniformMatrix4(shaderProjectionMatrixLocation, false, ref mvp);
            GL.Uniform1(shaderFontTextureLocation, 0);
            CheckGLError("Projection");

            GL.BindVertexArray(vertexArray);
            CheckGLError("VAO");

            draw_data.ScaleClipRects(io.DisplayFramebufferScale);

            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.ScissorTest);
            GL.BlendEquation(BlendEquationMode.FuncAdd);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Disable(EnableCap.CullFace);
            GL.Disable(EnableCap.DepthTest);

            // Render command lists
            for (int n = 0; n < draw_data.CmdListsCount; n++)
            {
                ImDrawListPtr cmd_list = draw_data.CmdLists[n];

                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, cmd_list.VtxBuffer.Size * Unsafe.SizeOf<ImDrawVert>(), cmd_list.VtxBuffer.Data);
                CheckGLError($"Data Vert {n}");

                GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, cmd_list.IdxBuffer.Size * sizeof(ushort), cmd_list.IdxBuffer.Data);
                CheckGLError($"Data Idx {n}");

                for (int cmd_i = 0; cmd_i < cmd_list.CmdBuffer.Size; cmd_i++)
                {
                    ImDrawCmdPtr pcmd = cmd_list.CmdBuffer[cmd_i];
                    if (pcmd.UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        GL.ActiveTexture(TextureUnit.Texture0);
                        GL.BindTexture(TextureTarget.Texture2D, (int)pcmd.TextureId);
                        CheckGLError("Texture");

                        // We do _windowHeight - (int)clip.W instead of (int)clip.Y because gl has flipped Y when it comes to these coordinates
                        var clip = pcmd.ClipRect;
                        GL.Scissor((int)clip.X, windowHeight - (int)clip.W, (int)(clip.Z - clip.X), (int)(clip.W - clip.Y));
                        CheckGLError("Scissor");

                        if ((io.BackendFlags & ImGuiBackendFlags.RendererHasVtxOffset) != 0)
                        {
                            GL.DrawElementsBaseVertex(PrimitiveType.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (IntPtr)(pcmd.IdxOffset * sizeof(ushort)), unchecked((int)pcmd.VtxOffset));
                        }
                        else
                        {
                            GL.DrawElements(BeginMode.Triangles, (int)pcmd.ElemCount, DrawElementsType.UnsignedShort, (int)pcmd.IdxOffset * sizeof(ushort));
                        }
                        CheckGLError("Draw");
                    }
                }
            }

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.ScissorTest);

            // Reset state
            GL.BindTexture(TextureTarget.Texture2D, prevTexture2D);
            GL.ActiveTexture((TextureUnit)prevActiveTexture);
            GL.UseProgram(prevProgram);
            GL.BindVertexArray(prevVAO);
            GL.Scissor(prevScissorBox[0], prevScissorBox[1], prevScissorBox[2], prevScissorBox[3]);
            GL.BindBuffer(BufferTarget.ArrayBuffer, prevArrayBuffer);
            GL.BlendEquationSeparate((BlendEquationMode)prevBlendEquationRgb, (BlendEquationMode)prevBlendEquationAlpha);
            GL.BlendFuncSeparate(
                (BlendingFactorSrc)prevBlendFuncSrcRgb,
                (BlendingFactorDest)prevBlendFuncDstRgb,
                (BlendingFactorSrc)prevBlendFuncSrcAlpha,
                (BlendingFactorDest)prevBlendFuncDstAlpha);
            if (prevBlendEnabled) GL.Enable(EnableCap.Blend); else GL.Disable(EnableCap.Blend);
            if (prevDepthTestEnabled) GL.Enable(EnableCap.DepthTest); else GL.Disable(EnableCap.DepthTest);
            if (prevCullFaceEnabled) GL.Enable(EnableCap.CullFace); else GL.Disable(EnableCap.CullFace);
            if (prevScissorTestEnabled) GL.Enable(EnableCap.ScissorTest); else GL.Disable(EnableCap.ScissorTest);
            if (glVersion <= 310 || compatibilityProfile)
            {
                GL.PolygonMode(TriangleFace.Front, (PolygonMode)prevPolygonMode[0]);
                GL.PolygonMode(TriangleFace.Back, (PolygonMode)prevPolygonMode[1]);
            }
            else
            {
                GL.PolygonMode(TriangleFace.FrontAndBack, (PolygonMode)prevPolygonMode[0]);
            }
        }

        public static void LabelObject(ObjectLabelIdentifier objLabelIdent, int glObject, string name)
        {
            if (khrDebugAvailable)
                GL.ObjectLabel(objLabelIdent, glObject, name.Length, name);
        }

        static bool IsExtensionSupported(string name)
        {
            int n = GL.GetInteger(GetPName.NumExtensions);
            for (int i = 0; i < n; i++)
            {
                string extension = GL.GetString(StringNameIndexed.Extensions, i);
                if (extension == name) return true;
            }

            return false;
        }

        public static int CreateProgram(string name, string vertexSource, string fragmentSoruce)
        {
            int program = GL.CreateProgram();
            LabelObject(ObjectLabelIdentifier.Program, program, $"Program: {name}");

            int vertex = CompileShader(name, ShaderType.VertexShader, vertexSource);
            int fragment = CompileShader(name, ShaderType.FragmentShader, fragmentSoruce);

            GL.AttachShader(program, vertex);
            GL.AttachShader(program, fragment);

            GL.LinkProgram(program);

            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                Debug.WriteLine($"GL.LinkProgram had info log [{name}]:\n{info}");
            }

            GL.DetachShader(program, vertex);
            GL.DetachShader(program, fragment);

            GL.DeleteShader(vertex);
            GL.DeleteShader(fragment);

            return program;
        }

        private static int CompileShader(string name, ShaderType type, string source)
        {
            int shader = GL.CreateShader(type);
            LabelObject(ObjectLabelIdentifier.Shader, shader, $"Shader: {name}");

            GL.ShaderSource(shader, source);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetShaderInfoLog(shader);
                Debug.WriteLine($"GL.CompileShader for shader '{name}' [{type}] had info log:\n{info}");
            }

            return shader;
        }

        public static void CheckGLError(string title)
        {
            ErrorCode error;
            int i = 1;
            while ((error = GL.GetError()) != ErrorCode.NoError)
            {
                Debug.Print($"{title} ({i++}): {error}");
            }
        }

        private void InitializeKeys()
        {
            foreach (Keys tkKey in Enum.GetValues<Keys>())
            {
                if (Enum.TryParse(tkKey.ToString(), out ImGuiKey imKey))
                    convertedImGuiKeys.TryAdd(tkKey, imKey);
            }

            #region Manually Mapping

            convertedImGuiKeys.Add(Keys.Left, ImGuiKey.LeftArrow);
            convertedImGuiKeys.Add(Keys.Right, ImGuiKey.RightArrow);
            convertedImGuiKeys.Add(Keys.Up, ImGuiKey.UpArrow);
            convertedImGuiKeys.Add(Keys.Down, ImGuiKey.DownArrow);

            convertedImGuiKeys.Add(Keys.D0, ImGuiKey._0);
            convertedImGuiKeys.Add(Keys.D1, ImGuiKey._1);
            convertedImGuiKeys.Add(Keys.D2, ImGuiKey._2);
            convertedImGuiKeys.Add(Keys.D3, ImGuiKey._3);
            convertedImGuiKeys.Add(Keys.D4, ImGuiKey._4);
            convertedImGuiKeys.Add(Keys.D5, ImGuiKey._5);
            convertedImGuiKeys.Add(Keys.D6, ImGuiKey._6);
            convertedImGuiKeys.Add(Keys.D7, ImGuiKey._7);
            convertedImGuiKeys.Add(Keys.D8, ImGuiKey._8);
            convertedImGuiKeys.Add(Keys.D9, ImGuiKey._9);

            convertedImGuiKeys.Add(Keys.KeyPad0, ImGuiKey.Keypad0);
            convertedImGuiKeys.Add(Keys.KeyPad1, ImGuiKey.Keypad1);
            convertedImGuiKeys.Add(Keys.KeyPad2, ImGuiKey.Keypad2);
            convertedImGuiKeys.Add(Keys.KeyPad3, ImGuiKey.Keypad3);
            convertedImGuiKeys.Add(Keys.KeyPad4, ImGuiKey.Keypad4);
            convertedImGuiKeys.Add(Keys.KeyPad5, ImGuiKey.Keypad5);
            convertedImGuiKeys.Add(Keys.KeyPad6, ImGuiKey.Keypad6);
            convertedImGuiKeys.Add(Keys.KeyPad7, ImGuiKey.Keypad7);
            convertedImGuiKeys.Add(Keys.KeyPad8, ImGuiKey.Keypad8);
            convertedImGuiKeys.Add(Keys.KeyPad9, ImGuiKey.Keypad9);

            #endregion Manually Mapping
        }
    }
}