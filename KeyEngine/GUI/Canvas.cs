using KeyEngine.Mathematics;
using KeyEngine.Rendering;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyEngine.GUI
{
    public class Button : Component
    {
        public Vector2 OriginalPosition { get; set; }
        public Vector2 OriginalSize { get; set; }
        public Vector2 CurrentPosition { get; private set; }
        public Vector2 CurrentSize { get; private set; }

        private Vector2 _originalSize;
        private Vector2 _currentSize;

        public Button(Entity owner) : base(owner)
        {

        }

        public void Init()
        {
            _originalSize = new Vector2(MainWindow.Instance.ClientSize.X, MainWindow.Instance.ClientSize.Y);
            _currentSize = _originalSize;

            OriginalPosition = Camera.Main.WorldToScreenCoords(Owner.Position);
            OriginalSize = Owner.Scale;
            CurrentPosition = OriginalPosition;
            CurrentSize = OriginalSize;

            MainWindow.Instance.Resize += Resized;
        }

        public override void Update(float deltaTime)
        {
            Log.Print(Input.MousePosition);
        }

        private void Resized(ResizeEventArgs args)
        {
            Log.Print(Camera.Main.WorldToScreenCoords(Owner.Position));
            Vector2 vec = new Vector2(OriginalPosition.X + (args.Width / 2), OriginalPosition.Y);
            Owner.Position = Camera.Main.ScreenToWorldCoords(vec);

    //        _currentSize = new Vector2(args.Width, args.Height);

            //        Vector2 scale = new Vector2(
            //_currentSize.X / _originalSize.X,
            //_currentSize.Y / _originalSize.Y);

            //        Log.Print(scale);

            //        CurrentPosition = OriginalPosition * scale;
            //        CurrentSize = OriginalSize * scale;

            //        Owner.Position = CurrentPosition;
            //Owner.Scale = CurrentSize;
        }
    }
}
