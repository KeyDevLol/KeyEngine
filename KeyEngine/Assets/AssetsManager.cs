using KeyEngine.Serialization;

namespace KeyEngine.Assets
{
    // TODO: Получение assetdata пробует получить ассет по пути, а не ключепути
    public static class AssetsManager
    {
        public const string ASSETS_FOLDER_PATH = "Assets";
        public const char PATH_KEY_CHAR = ':';

        public static readonly Dictionary<string, AssetInfo> Assets = [];
        private static readonly Dictionary<string, Type> registeredAssetTypes = [];

        public static void RegisterAssets(IAssetRegistration assetRegistration)
        {
            assetRegistration.Register();
        }

        public static void RegisterAssetType<T>(params string[] extensions) where T : Asset
        {
            foreach (string extension in extensions)
            {
                if (registeredAssetTypes.ContainsKey(extension))
                    continue;

                registeredAssetTypes.Add(extension, typeof(T));
            }
        }

        public static AssetReference<T>? GetAssetReference<T>(string programRelativePath) where T : Asset
        {
            return new AssetReference<T>(GetAsset<T>(programRelativePath));
        }

        public static Asset? GetAsset(string programRelativePath, Type type)
        {
            string pathKey = PathToDictKey(programRelativePath);
            string normalizedPath = NormalizePath(programRelativePath);

            if (Assets.TryGetValue(pathKey, out AssetInfo? data))
            {
                if (data.Instance == null)
                    return null;

                return data.Instance;
            }
            else
            {
                if (!File.Exists(normalizedPath))
                {
                    Console.WriteLine($"TEST: {normalizedPath}");
                    Console.WriteLine($"TEST2: {Environment.CurrentDirectory}");
                    throw new FileNotFoundException();
                }

                string dataPath = GetAssetDataPath(normalizedPath);
                string extension = Path.GetExtension(normalizedPath)[1..];

                Asset assetInstance = CreateAsset(type, normalizedPath, dataPath);

                AssetInfo assetInfo = new AssetInfo(normalizedPath, dataPath, assetInstance);
                Assets.Add(pathKey, assetInfo);

                return assetInfo.Instance;
            }
        }

        public static T? GetAsset<T>(string programRelativePath) where T : Asset
        {
            return GetAsset(programRelativePath, typeof(T)) as T;
        }

        public static Asset? GetAssetAuto(string programRelativePath)
        {
            string extension = Path.GetExtension(programRelativePath)[1..];

            if (registeredAssetTypes.TryGetValue(extension, out Type? assetType))
            {
                return GetAsset(programRelativePath, assetType);
            }
            else
            {
                Log.Print($"This asset extension: '{extension}' is not registered.");
                return null;
            }
        }

        public static AssetInfo? GetAssetInfo(string programRelativePath)
        {
            string normalizedPath = NormalizePath(programRelativePath);
            string pathKey = PathToDictKey(programRelativePath);

            if (Assets.TryGetValue(normalizedPath, out AssetInfo? info))
            {
                return info;
            }

            if (GetAssetAuto(normalizedPath) == null)
                return null;

            return Assets[pathKey];
        }
        public static void SaveAsset(Asset asset)        
        {
            string? path = asset.AssetPath;

            if (!string.IsNullOrEmpty(path))
                SerializationManager.SerializeToFile(GetAssetDataPath(path), asset);
        }

        // TODO: В линуксе и на макосе нету расширений файлов, так что нужно будет потестить
        public static string GetAssetDataPath(string sourcePath)
        {
            string assetFileNameWithoutExtension = Path.GetFileNameWithoutExtension(sourcePath);
            string dataFileName = assetFileNameWithoutExtension + ".assetdata";
            string? assetFolder = Path.GetDirectoryName(sourcePath) ?? throw new NullReferenceException();

            return Path.Combine(assetFolder, dataFileName);
        }

        public static void UnloadAllAssets()
        {
            foreach (AssetInfo asset in Assets.Values)
            {
                asset.Unload();
            }

            Assets.Clear();
        }

        private static Asset CreateAsset(Type type, string path, string dataPath)
        {
            //if (type.GetInterface(nameof(IAsset)) != null)
            //    throw new InvalidCastException();

            object? obj = Activator.CreateInstance(type) ?? throw new NullReferenceException();
            Asset result = (Asset)obj;
            CreateAssetData(result, dataPath);
            result.LoadAsset(path);

            return result;
        }

        private static void CreateAssetData(Asset? asset, string dataPath)
        {
            if (!File.Exists(dataPath) && asset != null)
            {
                SerializeData? defaultData = asset.GetDefaultAssetData();

                if (defaultData == null)
                    SerializationManager.SerializeToFile(dataPath, asset);
                else
                    SerializationManager.SerializeToFile(dataPath, defaultData);
            }

            if (asset != null)
            {
                SerializeData? serializeData = SerializationManager.DeserializeFile(dataPath);

                if (serializeData != null)
                    asset.Deserialize(serializeData);
            }
        }

        private static string NormalizePath(string path)
        {
            path = path.Replace('\\', Path.DirectorySeparatorChar);
            path = path.Replace('/', Path.DirectorySeparatorChar);

            return path;
        }

        private static string PathToDictKey(string path)
        {
            path = path.Replace('\\', PATH_KEY_CHAR);
            path = path.Replace('/', PATH_KEY_CHAR);
            path = path.Replace(Path.DirectorySeparatorChar, PATH_KEY_CHAR);

            return path;
        }
    }
}
