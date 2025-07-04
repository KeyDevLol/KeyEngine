﻿using OpenTK.Mathematics;

namespace KeyEngine.Rendering
{
    public class Transformable
    {
        protected bool isDirty;

        public Vector2 Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    isDirty = true;
                    TransformChanged();
                }
            }
        }
        protected Vector2 _position;

        public Vector2 Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    isDirty = true;
                    TransformChanged();
                }
            }
        }
        protected Vector2 _scale;

        public float Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    isDirty = true;
                    TransformChanged();
                }
            }
        }
        protected float _rotation;

        public Matrix4 Model
        {
            get 
            {
                RefreshIfDirty();
                return _model;
            }
        }
        protected Matrix4 _model;

        public event Action? OnTransformChanged;
        public bool quietChange;

        public Transformable()
        {
            Position = Vector2.Zero;
            Scale = Vector2.One;
            Rotation = 0;
            RefreshIfDirty();
        }

        protected virtual void RefreshIfDirty()
        {
            if (!isDirty)
                return;

            Matrix4 transform = Matrix4.Identity;

            transform *= Matrix4.CreateScale(_scale.X, _scale.Y, 0);
            transform *= Matrix4.CreateRotationZ(_rotation * Mathf.DEG_2_RAD);
            transform *= Matrix4.CreateTranslation(_position.X, _position.Y, 1);

            _model = transform;
            isDirty = false;
        }

        private void TransformChanged()
        {
            if (quietChange == false)
                OnTransformChanged?.Invoke();
        }

        public void BeginQuiteMode()
        {
            quietChange = true;
        }

        public void EndQuiteMode()
        {
            quietChange = false;
        }
    }
}
