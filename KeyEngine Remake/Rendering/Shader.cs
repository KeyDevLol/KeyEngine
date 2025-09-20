using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using Vector2 = KeyEngine.Mathematics.Vector2;

namespace KeyEngine.Rendering
{
    public class Shader : IDisposable
    {
        public static readonly Shader Default = new Shader("Assets/Shaders/Default.vert", "Assets/Shaders/Default.frag");

        public readonly int Handle = -1;
        private bool disposed;

        private readonly Dictionary<string, int> cachedUniformLocations;
        public string? Name { get; private set; }

        public Shader()
        {
            Handle = GL.CreateProgram();
            cachedUniformLocations = new Dictionary<string, int>();
        }

        public Shader(in string vertPath, in string fragPath)
        {
            Handle = GL.CreateProgram();
            cachedUniformLocations = new Dictionary<string, int>();

            LoadFromFile(vertPath, fragPath);
        }

#if WARN_MEMORY_LEAKS
        ~Shader()
        {
            Log.Assert(disposed, $"{nameof(Shader)} was finalized but not disposed, this is a memory leak.", LogType.Warning);
        }
#endif // WARN_MEMORY_LEAKS

        public void Bind()
        {
            Bind(Handle);
        }

        public static void Bind(int handle)
        {
            GL.UseProgram(handle);
        }

        public static void Bind(Shader shader)
        {
            GL.UseProgram(shader.Handle);
        }

        public static void Unbind()
        {
            GL.UseProgram(0);
        }

        public void LoadFromFile(in string vertexPath, in string fragmentPath)
        {
            LoadFromSource(LoadFileSource(vertexPath), LoadFileSource(fragmentPath));
            Name = Path.GetFileNameWithoutExtension(vertexPath);
        }

        public void LoadFromSource(in string vertexSource, in string fragmentSource)
        {
            // Vertex
            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);

            // Fragment
            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);

            if (!CheckShaderCompile(vertexShader) || !CheckShaderCompile(fragmentShader))
                return;

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);

            GL.LinkProgram(Handle);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public static string LoadFileSource(in string path)
        {
            string source = string.Empty;

            using (StreamReader reader = new StreamReader(File.OpenRead(path)))
            {
                source = reader.ReadToEnd();
            }

            return source;
        }

        public void Dispose()
        {
            if (disposed)
                return;

            GL.DeleteProgram(Handle);
            GC.SuppressFinalize(this);
            disposed = true;
        }

        private bool CheckShaderCompile(int handler)
        {
            GL.GetShader(handler, ShaderParameter.CompileStatus, out int success);

            if (success == 0)
            {
                Log.Print($"{nameof(Shader)} {Name}: compile error\n{GL.GetShaderInfoLog(handler)}", LogType.Error);
                GL.DeleteShader(handler);
                return false;
            }

            return true;
        }

        public void SetMatrix4(in string name, in bool transpose, Matrix4 value)
        {
            if (!TryGetUniformLocation(name, out int location))
            {
                PrintUniformNotFoundedError(name);
                return;
            }
            GL.UniformMatrix4(location, transpose, ref value);
        }

        public void SetMatrix4(in string name, in bool transpose, Matrix4[] value)
        {
            if (!TryGetUniformLocation(name, out int location))
            {
                PrintUniformNotFoundedError(name);
                return;
            }
            float[] values = new float[value.Length * 16];
            for (int i = 0; i < value.Length; i++)
            {
                values[i    ] = value[i].M11;
                values[i + 1] = value[i].M12;
                values[i + 2] = value[i].M13;
                values[i + 3] = value[i].M14;

                values[i + 4] = value[i].M21;
                values[i + 5] = value[i].M22;
                values[i + 6] = value[i].M23;
                values[i + 7] = value[i].M24;

                values[i + 8] = value[i].M31;
                values[i + 9] = value[i].M32;
                values[i + 10] = value[i].M33;
                values[i + 11] = value[i].M34;

                values[i + 12] = value[i].M41;
                values[i + 13] = value[i].M42;
                values[i + 14] = value[i].M43;
                values[i + 15] = value[i].M44;
            }
            GL.UniformMatrix4(location, value.Length, transpose, values);
        }

        public void SetVector2(in string name, Vector2 value)
        {
            if (!TryGetUniformLocation(name, out int location))
            {
                PrintUniformNotFoundedError(name);
                return;
            }
            GL.Uniform2(location, value.X, value.Y);
        }

        public void SetVector2(in string name, params Vector2[] value)
        {
            if (!TryGetUniformLocation(name, out int location))
            {
                PrintUniformNotFoundedError(name);
                return;
            }
            float[] values = new float[value.Length * 2];
            for (int i = 0; i < value.Length; i++)
            {
                values[i] = value[i].X;
                values[i+1] = value[i].Y;
            }
            GL.Uniform2(location, value.Length, values);
        }

        private bool TryGetUniformLocation(in string name, out int location)
        {
            if (!cachedUniformLocations.TryGetValue(name, out location))
            {
                location = GL.GetUniformLocation(Handle, name);

                if (location == -1)
                    return false;

                cachedUniformLocations.Add(name, location);
            }

            return true;
        }

        private void PrintUniformNotFoundedError(string uniformName)
        {
            Log.Print($"{nameof(Shader)} {Name}: failed to set ({uniformName}) value.", LogType.Error);
        }
    }
}
