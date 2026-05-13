using KeyEngine.Samples;
using KeyEngine.Assets;
using KeyEngine.Core;

namespace KeyEngine
{
    internal class Program
    {
        private static void Main()
        {
            // Move to KeyEngine Studio
            AssetsManager.RegisterAssets(new DefaultAssetsRegistration());
            Engine.Run(new SampleScene());
        }
    }
}
