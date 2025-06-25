using KeyEngine.Graphics;

namespace KeyEngine.Editor
{
    public class AssetReference<T> where T : class, IAsset
    {
        private readonly WeakReference weakReference;
        public T? Value
        {
            get => weakReference.Target as T;
            set => weakReference.Target = value;
        }
        public bool IsLoaded => weakReference.IsAlive;


        public AssetReference()
        {
            weakReference = new WeakReference(null);
        }

        public AssetReference(T asset)
        {
            weakReference = new WeakReference(asset);
        }

        public AssetReference(string filePath)
        {
            weakReference = new WeakReference(AssetsManager.GetAsset<T>(filePath));
        }
    }
}
