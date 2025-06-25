using KeyEngine.Audio;
using KeyEngine.Graphics;

namespace KeyEngine.Editor
{
    // TODO: Сделать чтобы все ассеты загружались не сразу, только после вызова метода GetAsset, если таковой ассет не был загружен.
    public static class AssetsManager
    {
        public const string ASSETS_FOLDER_PATH = "Assets";

        public static Dictionary<string, AssetData> Assets = new Dictionary<string, AssetData>();

        public static T? GetAsset<T>(string filePath) where T : IAsset
        {
            filePath = filePath.Replace('/', '\\');

            if (Assets.TryGetValue(filePath, out AssetData? data))
            {
                Log.Print("Successfully");

                if (data.Instance == null)
                    return default;

                return (T)data.Instance;
            }
            else
            {
                Log.Print("Load new asset");

                if (!File.Exists(filePath))
                    throw new FileNotFoundException();

                Type? assetType = GetAssetType(filePath);

                AssetData assetData;

                if (assetType != null)
                {
                    if (typeof(T) != assetType)
                        throw new Exception($"Generic given type: {typeof(T)} and founded asset type: {assetType} is diffirent.");

                    assetData = new AssetData(assetType, filePath);
                }
                else
                    throw new Exception("This asset type is not supported.");

                Assets.Add(filePath, assetData);

                if (assetData.Instance == null)
                    return default;

                return (T)assetData.Instance;
            }
        }

        private static Type? GetAssetType(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);

            switch (fileExtension)
            {
                case ".png":    return typeof(Texture);
                case ".bmp":    return typeof(Texture);
                case ".jpg":    return typeof(Texture);
                case ".jpeg":   return typeof(Texture);
                case ".wav":   return typeof(AudioSample);
                case ".mp3":   return typeof(AudioSample);
                case ".ogg":   return typeof(AudioSample);

                default: return null;
            }
        }

        public static void UnloadAllAssets()
        {
            foreach (AssetData asset in Assets.Values)
            {
                asset.Unload();
            }

            Assets.Clear();
        }
    }
}
