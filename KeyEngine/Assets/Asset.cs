using KeyEngine.Serialization;

namespace KeyEngine.Assets
{
    public abstract class Asset : ISerializable
    {
        public string? AssetPath { get; protected set; }
        public virtual bool AssetLoaded { get; protected set; }

        internal abstract void LoadAsset(string sourcePath);
        internal abstract void UnloadAsset();
        internal virtual SerializeData? GetDefaultAssetData() { return null; }

        public abstract SerializeData Serialize();
        public abstract void Deserialize(SerializeData data);
    }
}
