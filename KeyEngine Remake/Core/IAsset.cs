namespace KeyEngine
{
    public interface IAsset
    {
        public void LoadAsset(string path);
        public void UnloadAsset();

        public bool AssetLoaded { get; }
    }
}
