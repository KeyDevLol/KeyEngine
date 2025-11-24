using KeyEngine.Audio;
using KeyEngine.Graphics;

namespace KeyEngine
{
    public static class AssetsManager
    {
        public const string ASSETS_FOLDER_PATH = "Assets";

        public static readonly Dictionary<string, AssetData> Assets = [];

        public static AssetReference<T>? GetAsset<T>(string programRelativePath) where T : class, IAsset
        {
            programRelativePath = programRelativePath.Replace('/', '\\');

            if (Assets.TryGetValue(programRelativePath, out AssetData? data))
            {
                if (data.Instance == null)
                    return default;

                return new AssetReference<T>((T)data.Instance);
            }
            else
            {
                if (!File.Exists(programRelativePath))
                    throw new FileNotFoundException();

                Type? assetType = GetAssetType(programRelativePath);

                AssetData assetData;

                if (assetType != null)
                {
                    if (typeof(T) != assetType)
                        throw new Exception($"Generic given type: {typeof(T)} and founded asset type: {assetType} is diffirent.");

                    assetData = new AssetData(assetType, programRelativePath);
                }
                else
                    throw new Exception("This asset type is not supported.");

                Assets.Add(programRelativePath, assetData);

                if (assetData.Instance == null)
                    return default;

                return new AssetReference<T>((T)assetData.Instance);
            }
        }

        private static Type? GetAssetType(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);

            return fileExtension switch
            {
                ".png" => typeof(Texture),
                ".bmp" => typeof(Texture),
                ".jpg" => typeof(Texture),
                ".jpeg" => typeof(Texture),
                ".wav" => typeof(AudioSample),
                ".mp3" => typeof(AudioSample),
                ".ogg" => typeof(AudioSample),
                _ => null,
            };
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
