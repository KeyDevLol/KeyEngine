using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Tests
{
    public class ChaseScript : Component
    {
        public float speed = 25f;
        private Entity? player;

        public ChaseScript(Entity owner) : base(owner)
        {
        }

        public override void Start()
        {
            player = ECS.FindEntityByName("Audio Listener (Player)");
        }

        public override void Update(float deltaTime)
        {
            Owner.Position = Vector2.MoveTowards(Owner.Position, player.Position, speed * deltaTime);
        }
    }
}
