using KeyEngine.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Game
{
    public class SpriteChanger : Component
    {
        private SpriteRenderer spriteRenderer = null!;
        public static int CurrentSprite
        {
            get => currentSprite;
            set { currentSprite = value; CurrentSpriteChanged(); }
        }
        private static int currentSprite = 0;
        private static SpriteChanger instance = null!;

        public SpriteChanger(Entity owner) : base(owner)
        {
            instance = this;
        }

        public override void Start()
        {
            spriteRenderer = Owner.AddComponent<SpriteRenderer>();
            Owner.Scale = new Vector2(4, 4);
            Owner.Position = new Vector2(10, -7);
        }

        public override void Update(float deltaTime)
        {
            if (Input.IsKeyPressed(KeyCode.Q))
            {
                CurrentSprite--;
            }
            else if (Input.IsKeyPressed(KeyCode.E))
            {
                CurrentSprite++;
            }
        }

        private static void CurrentSpriteChanged()
        {
            //instance.spriteRenderer.Texture = TileSprites.Sprites[currentSprite].Value;
        }
    }
}
