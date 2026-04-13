using KeyEngine.Graphics;
using KeyEngine.Audio;

namespace KeyEngine.Assets
{
    public struct DefaultAssetsRegistration : IAssetRegistration
    {
        public readonly void Register()
        {
            AssetsManager.RegisterAssetType<Texture>("png", "jpg", "jpeg", "bmp", "psd", "tga", "hdr");
            AssetsManager.RegisterAssetType<AudioSample>("wav");
            AssetsManager.RegisterAssetType<Font>("ttf");
        }
    }
}
