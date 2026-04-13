namespace KeyEngine.Assets
{
    public class AssetInfo
    {
        public string Path { get; private set; }
        public string DataPath { get; private set; }
        public Asset? Instance { get; private set; }

        public AssetInfo(string path, string dataPath, Asset? instance)
        {
            Path = path;
            DataPath = dataPath;
            Instance = instance;
        }

        public void Unload() => Instance?.UnloadAsset();
    }
}
