using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace KeyEngine.Rendering
{
    public class Shader : IDisposable
    {
        public static readonly Shader Default = new Shader("Assets/Shaders/Default.vert", "Assets/Shaders/Default.frag");

        public readonly int Handle = -1;
        private bool disposed;

        private readonly Dictionary<string, int> cachedUniformLocations;

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

        public void SetMatrix4(in string name, in bool transpose, Matrix4 value)
        {
            if (!TryGetUniformLocation(name, out int location))
            {
                Log.Print($"{nameof(Shader)}.{nameof(SetMatrix4)} failed to set ({name}) value.", LogType.Error);
                return;
            }
            GL.UniformMatrix4(location, transpose, ref value);
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
    }
}
