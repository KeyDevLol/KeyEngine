using FreeTypeSharp;
using KeyEngine.Assets;
using KeyEngine.Mathematics;
using KeyEngine.Serialization;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;
using static FreeTypeSharp.FT;

namespace KeyEngine
{
    // TIP: Мейби лучше не инициализировать каждый раз либу
    public class Font : Asset, IDisposable
    {
        public const string ENG_SYMBOLS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string RUS_SYMBOLS = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        public const string NUMBER_SYMBOLS = "1234567890";
        public const string SPEC_SYMBOLS = "!\"@#$%&'()*+,-./\\:;<=>?[]^_`{|}~∎ ";
        public const string ALL_PRESETS = ENG_SYMBOLS + RUS_SYMBOLS + NUMBER_SYMBOLS + SPEC_SYMBOLS;

        public string? Name;
        public short Asscender;

        public uint Width { get; set; }
        public uint Height { get; set; }
        public override bool AssetLoaded => glyphs != null;


        public string? UsedSymbols;

        private Dictionary<char, Glyph> glyphs = [];

        public Font() { }

        public unsafe Font(string path, string characters, uint resWidth = 500, uint resHeight = 500)
        {
            AssetsManager.RegisterAssetType<Font>("ttf");
            LoadFromFile(path, characters, resWidth, resHeight);
        }

        private unsafe void LoadFromFile(string path, string characters, uint resWidth = 500, uint resHeight = 500)
        {
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

                glyphs.Add(c, glyph);
            }

            GL.PixelStore(PixelStoreParameter.UnpackAlignment, 4);

            FT_Done_Face(face);
            FT_Done_FreeType(lib);

            UsedSymbols = characters;
            AssetPath = path;
        }

        public void Dispose()
        {
            foreach (Glyph glyph in glyphs.Values)
            {
                glyph.Dispose();
            }

            glyphs = null!;

            GC.SuppressFinalize(this);
        }

        public bool TryGetGlyph(char ch, out Glyph glyph)
        {
            return glyphs.TryGetValue(ch, out glyph);
        }

        private void CheckError(FT_Error error, string operation)
        {
            if (error != FT_Error.FT_Err_Ok)
            {
                glyphs = null!;
                throw new InvalidOperationException($"{operation}. Error: {error}");
            }
        }

        internal override void LoadAsset(string sourcePath)
        {
            LoadFromFile(sourcePath, UsedSymbols!, Width, Height);
        }

        internal override void UnloadAsset()
        {
            Dispose();
        }

        public override SerializeData Serialize()
        {
            SerializeData serializeData = new SerializeData();
            serializeData.AddData("symbols", UsedSymbols);
            serializeData.AddData("width", Width);
            serializeData.AddData("height", Height);
            return serializeData;
        }

        public override void Deserialize(SerializeData data)
        {
            UsedSymbols = data.GetData<string>("symbols");
            Width = data.GetData<uint>("width");
            Height = data.GetData<uint>("height");
        }

        internal override SerializeData? GetDefaultAssetData()
        {
            SerializeData serializeData = new SerializeData();
            serializeData.AddData("symbols", ALL_PRESETS);
            serializeData.AddData("width", 512);
            serializeData.AddData("height", 512);

            return serializeData;
        }
    }
}
