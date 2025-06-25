using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Audio
{
    public class AudioListener : Component
    {
        //private Vector2 pos = new Vector2(0f, 0f);
        //private float dis;
        public static AudioListener? Instance { get; private set; }

        public AudioListener(Entity owner) : base(owner) { Instance = this; Owner.OnTransformChanged += OnOwnerTransformChanged; }

        //public override void Start()
        //{
        //    pos = Owner.Position;
        //}

        private void OnOwnerTransformChanged()
        {
            Vector2 position = Owner.Position;
            AL.Listener(ALListener3f.Position, position.X, position.Y, 0);
        }

        //public override void Update(float deltaTime)
        //{
        //    Vector2 velocity = (Owner.Position - pos) / deltaTime;
        //    AL.Listener(ALListener3f.Velocity, velocity.X, velocity.Y, 0);
        //    pos = Owner.Position;
        //}
    }
}
