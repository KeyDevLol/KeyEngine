using KeyEngine.Assets;
using KeyEngine.Core;
using KeyEngine.Samples;

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
