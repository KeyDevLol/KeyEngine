using KeyEngine.Graphics;

namespace KeyEngine.Game
{
    public static class TileSprites
    {
        public readonly static List<AssetReference<Texture>> Sprites;

        static TileSprites()
        {
            Sprites = new List<AssetReference<Texture>>();

            foreach (string path in Directory.GetFiles("Assets/Tiles"))
            {
                //Sprites.Add(new AssetReference<Texture>(path));
            }
        }
    }
}
