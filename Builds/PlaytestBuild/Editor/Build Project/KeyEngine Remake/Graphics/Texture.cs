using KeyEngine.Editor;
using KeyEngine.Renderer;
using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KeyEngine.Graphics
{
    public class Texture : IDisposable, IAsset
    {
        public static readonly Texture Square = new([255, 255, 255, 255], 1, 1);

        public readonly int Handle = -1;
        private bool disposed;

        public WrapMode WrapMode
        {
            get { return _wrapMode; }
            set { _wrapMode = value; UpdateParams(); }
        }
        private WrapMode _wrapMode = WrapMode.Clamp;

        public FilterMode FilterMode
        {
            get { return _filterMode; }
            set { _filterMode = value; UpdateParams(); }
        }
        private FilterMode _filterMode = FilterMode.Nearest;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public Texture()
        {
            Handle = GL.GenTexture();
            UpdateParams();
        }

        public Texture(in string? path)
        {
            Handle = GL.GenTexture();
            UpdateParams();
            LoadFromFile(path);
        }

        public Texture(in byte[] data, in int width, in int height)
        {
            Handle = GL.GenTexture();
            UpdateParams();
            LoadFromBytes(data, width, height);
        }

#if WARN_MEMORY_LEAKS
        ~Texture()
        {
            Log.Assert(disposed, $"{nameof(Texture)} was finalized but not disposed, this is a memory leak.", LogType.Warning);
        }
#endif // WARN_MEMORY_LEAKS

        public void Bind()
        {
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Unbind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void LoadFromFile(in string? path)
        {
            if (!File.Exists(path))
            {
                Log.Print($"{nameof(Texture)}.{nameof(LoadFromFile)} file does not exists.", LogType.Error);
                return;
            }

            Bind();

            StbImage.stbi_set_flip_vertically_on_load(1);

            using (Stream stream = File.OpenRead(path))
            {
                ImageResult imageResult = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

                Width = imageResult.Width;
                Height = imageResult.Height;

                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, imageResult.Data);
            }

            Unbind();
        }

        public void LoadFromBytes(in byte[] data, in int width, in int height)
        {
            Bind();

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data);

            Width = width;
            Height = height;

            Unbind();
        }

        private void UpdateParams()
        {
            Bind();

            switch (_filterMode)
            {
                // Nearest filter
                case FilterMode.Nearest:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                        (int)TextureMinFilter.Nearest);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                        (int)TextureMagFilter.Nearest);
                    break;
                // Linear filter
                case FilterMode.Linear:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter,
                        (int)TextureMinFilter.Linear);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter,
                        (int)TextureMagFilter.Linear);
                    break;
            }


            switch (_wrapMode)
            {
                // Clamp wrap
                case WrapMode.Clamp:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                        (int)TextureWrapMode.ClampToEdge);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                        (int)TextureWrapMode.ClampToEdge);
                    break;
                // Repeat wrap
                case WrapMode.Repeat:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                        (int)TextureWrapMode.Repeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                        (int)TextureWrapMode.Repeat);
                    break;
                // MirroredRepeat wrap
                case WrapMode.MirroredRepeat:
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS,
                        (int)TextureWrapMode.MirroredRepeat);
                    GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT,
                        (int)TextureWrapMode.MirroredRepeat);
                    break;
            }

            Unbind();
        }

        public void Dispose()
        {
            if (disposed)
                return;

            GL.DeleteTexture(Handle);
            GC.SuppressFinalize(this);
            disposed = true;
        }

        public void LoadAsset(string path)
        {
            UpdateParams();
            LoadFromFile(path);
        }

        public void UnloadAsset()
        {
            Dispose();
        }
    }

    public enum FilterMode : byte
    {
        Nearest,
        Linear
    }

    public enum WrapMode : byte
    {
        Clamp,
        Repeat,
        MirroredRepeat,
    }
}
