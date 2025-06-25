using FreeTypeSharp;
using static FreeTypeSharp.FT;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using KeyEngine.Graphics;
using KeyEngine.Editor;

namespace KeyEngine
{
    public class Font : IDisposable, IAsset
    {
        public const string ENG_SYMBOLS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string RUS_SYMBOLS = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        public const string NUMBER_SYMBOLS = "1234567890";
        public const string SPEC_SYMBOLS = "!\"@#$%&'()*+,-./\\:;<=>?[]^_`{|}~∎ ";
        public const string ALL_PRESETS = ENG_SYMBOLS + RUS_SYMBOLS + NUMBER_SYMBOLS + SPEC_SYMBOLS;

        public readonly string Name;
        public readonly short Asscender;

        public readonly string Path;
        public readonly string UsedSymbols;

        public bool Loaded { get; private set; }

        public bool AssetLoaded => throw new NotImplementedException();

        private Dictionary<char, Glyph> glyphs;

        public unsafe Font(string characters, string path, uint resWidth = 500, uint resHeight = 500)
        {
            glyphs = new Dictionary<char, Glyph>();

            FT_LibraryRec_* lib;
            FT_FaceRec_* face;
            FT_Error error = FT_Error.FT_Err_Ok;

            //Lib init
            error = FT_Init_FreeType(&lib);
            CheckError(error, "FT_Init_FreeType");

            IntPtr pathPtr = Marshal.StringToHGlobalAnsi(path);

            //Face init
            error = FT_New_Face(lib, (byte*)pathPtr, 0, &face);
            CheckError(error, "FT_New_Face");

            Marshal.FreeHGlobal(pathPtr);

            Name = Marshal.PtrToStringAnsi((IntPtr)face->family_name) ?? "Failed to get name.";
            Asscender = face->ascender;

            //Set size
            error = FT_Set_Pixel_Sizes(face, 0, 128);
            //error = FT_Set_Char_Size(face, 0, 16 * 64, resWidth, resHeight);
            CheckError(error, "FT_Set_Char_Size");

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);

            foreach (char c in characters)
            {
                error = FT_Load_Char(face, (uint)c, FT_LOAD.FT_LOAD_RENDER);
                CheckError(error, "FT_Load_Char");

                int texture = GL.GenTexture();
                GL.BindTexture(TextureTarget.Texture2D, texture);

                byte* buffer = face->glyph->bitmap.buffer;

                GL.TexImage2D(
                    TextureTarget.Texture2D,
                    0,
                    PixelInternalFormat.CompressedRed,
                    (int)face->glyph->bitmap.width,
                    (int)face->glyph->bitmap.rows,
                    0, PixelFormat.Red,
                    PixelType.UnsignedByte,
                    (IntPtr)buffer);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMinFilter.Linear);

                Vector2 size = new Vector2(face->glyph->bitmap.width, face->glyph->bitmap.rows);
                Vector2 bearing = new Vector2(face->glyph->bitmap_left, face->glyph->bitmap_top);
                Glyph glyph = new Glyph(texture, size, bearing, face->glyph->advance.x);
                //{
                //    texture = charTexture,
                //    Size = new Vector2(face->glyph->bitmap.width, face->glyph->bitmap.rows),
                //    Bearing = new Vector2(face->glyph->bitmap_left, face->glyph->bitmap_top),
                //    Advance = face->glyph->advance.x
                //};

                glyphs.Add(c, glyph);
            }

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            FT_Done_Face(face);
            FT_Done_FreeType(lib);

            Loaded = true;
            UsedSymbols = characters;
            this.Path = path;
        }

        public void Dispose()
        {
            foreach (Glyph glyph in glyphs.Values)
            {
                glyph.Dispose();
            }

            glyphs = null;
            Loaded = false;

            GC.SuppressFinalize(this);
            Log.Print("Font - disposed.");
        }

        public bool TryGetGlyph(char ch, out Glyph glyph)
        {
            return glyphs.TryGetValue(ch, out glyph);
        }

        private void CheckError(FT_Error error, string operation)
        {
            if (error != FT_Error.FT_Err_Ok)
            {
                Loaded = false;
                throw new InvalidOperationException($"{operation}. Error: {error}");
            }
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }

        public void LoadAsset(string path)
        {
            throw new NotImplementedException();
        }

        public void UnloadAsset()
        {
            throw new NotImplementedException();
        }
    }
}
