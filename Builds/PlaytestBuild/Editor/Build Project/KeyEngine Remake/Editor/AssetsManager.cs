using KeyEngine.Graphics;
using System.Diagnostics;

namespace KeyEngine.Editor
{
    // ToDo: Сделать чтобы все ассеты загружались не сразу, только после вызова метода GetAsset, если таковой ассет не был загружен.
    public static class AssetsManager
    {
        public const string ASSETS_FOLDER_PATH = "Assets";

        public static Dictionary<string, AssetData> Assets = new Dictionary<string, AssetData>();

        static AssetsManager()
        {
            string[] files = Directory.GetFiles(ASSETS_FOLDER_PATH, "*.*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                string fileExtension = Path.GetExtension(file);
                Type? type = null;

                switch (fileExtension)
                {
                    case ".png": type = typeof(Texture); break;
                    case ".bmp": type = typeof(Texture); break;
                    case ".jpg": type = typeof(Texture); break;
                    case ".jpeg": type = typeof(Texture); break;
                }

                Log.Print(type);

                if (type != null)
                    Assets.Add(file, new AssetData(type, file));
            }
        }

        public static T? GetAsset<T>(string path) where T : IAsset
        {
            path = path.Replace('/', '\\');

            if (Assets.TryGetValue(path, out AssetData? data))
            {
                return (T)data.GetAssetInstance();
            }

            return default;
        }

        //public static Type GetAssetTypeByFileExtension(string extension)
        //{

        //}
    }

    public class AssetData
    {
        public bool WasUsed { get; private set; }
        public readonly string Path;
        private IAsset? instance;
        private Type type;

        public AssetData(Type type, string path)
        {
            Path = path;
            if (type.GetInterface(nameof(IAsset)) != null)
                this.type = type;
            else
                throw new InvalidCastException();
        }

        public IAsset GetAssetInstance()
        {
            Log.Print(WasUsed);

            if (WasUsed)
                return instance!;

            object? obj = Activator.CreateInstance(type) ?? throw new NullReferenceException();
            instance = (IAsset)obj;
            instance.LoadAsset(Path);

            WasUsed = true;

            return instance;
        }
    }

    public interface IAsset
    {
        public void LoadAsset(string path);
        public void UnloadAsset();
    }
}
