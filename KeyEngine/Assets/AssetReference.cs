using KeyEngine.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace KeyEngine.Assets
{
    public class AssetReference<T> where T : Asset
    {
        private readonly WeakReference weakReference;
        public T? Value
        {
            get => weakReference.Target as T;
            set => weakReference.Target = value;
        }
        [MemberNotNullWhen(true, nameof(Value))]
        public bool IsLoaded => weakReference.IsAlive;

        public AssetReference()
        {
            weakReference = new WeakReference(null);
        }

        public AssetReference(T? asset)
        {
            weakReference = new WeakReference(asset);
        }

        public static implicit operator T?(AssetReference<T> reference)
        {
            return reference.Value;
        }
    }
}
