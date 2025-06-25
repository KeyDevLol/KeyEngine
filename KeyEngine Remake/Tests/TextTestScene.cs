using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Tests
{
    public class TextTestScene : IScene
    {
        public void Load()
        {
            Entity textEntity = ECS.AddEntity("TextRenderer");
            TextRenderer textRenderer = textEntity.AddComponent<TextRenderer>();
            textRenderer.Font = new Font(Font.ALL_PRESETS, "Assets/Fonts/Pixel KeyDev font.ttf");
        }

        public void Unload()
        {
            //throw new NotImplementedException();
        }
    }
}
