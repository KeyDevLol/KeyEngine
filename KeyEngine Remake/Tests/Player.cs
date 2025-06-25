using KeyEngine.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.Tests
{
    //
    public class Player : Component
    {
        private Camera? camera;
        public float Speed = 10;
        public float CamSpeed = 10;

        public Player(Entity owner) : base(owner)
        {
            camera = Camera.Main;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            float x = Input.GetAxisRaw(KeyCode.A, KeyCode.D);
            float y = Input.GetAxisRaw(KeyCode.S, KeyCode.W);

            //camera.Position = Owner.Position;
            Owner.Position += new Vector2(x, y).Normalized * deltaTime * Speed;
            camera.Position = Vector2.Lerp(camera.Position, Owner.Position, CamSpeed * deltaTime);
            //camera.RefreshProjection();
            //camera.RefreshView();
            //camera.RefreshProjectionView();
            Log.Print(camera.Position);

            //if (camera != null)
            //{
            //    camera.Position = Owner.Position;
            //    Log.Print(camera.Position);
            //}
        }
    }
}
