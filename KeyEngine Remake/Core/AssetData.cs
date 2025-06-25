namespace KeyEngine.Editor
{
    public class AssetData
    {
        public readonly string Path;
        public readonly IAsset? Instance;
        private readonly Type type;

        public AssetData(Type type, string path)
        {
            Path = path;

            if (type.GetInterface(nameof(IAsset)) != null)
                this.type = type;
            else
                throw new InvalidCastException();

            object? obj = Activator.CreateInstance(type) ?? throw new NullReferenceException();
            Instance = (IAsset)obj;
            Instance.LoadAsset(Path);
        }

        public void Unload() => Instance?.UnloadAsset();
    }
}
