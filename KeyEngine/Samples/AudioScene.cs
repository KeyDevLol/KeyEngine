using KeyEngine.Mathematics;
using KeyEngine.Assets;
using KeyEngine.Audio;

namespace KeyEngine.Samples
{
    public class AudioScene : IScene
    {
        private readonly AssetReference<AudioSample>? audio = AssetsManager.GetAssetReference<AudioSample>("Assets/Audio/Vigilantism.mp3");

        public void Load()
        {
            if (audio == null || audio.Value == null)
                throw new NullReferenceException("Failed to load audio file. Check content folder.");

            Entity audioListener = ECS.AddEntity("Audio Listener");
            audioListener.AddComponent<AudioListener>();
            audioListener.AddComponent<SpriteRenderer>();

            Entity audioSourceEntity = ECS.AddEntity("Audio Source");
            audioSourceEntity.Scale = new(0.5f);
            AudioSource audioSource = audioSourceEntity.AddComponent<AudioSource>();
            audioSourceEntity.AddComponent<SpriteRenderer>().Color = Color32.Pink;
            audioSourceEntity.AddComponent<AudioSourceRotation>();
            audioSource.ReferenceDistance = 4.4f;
            audioSource.PanSmoothness = 2.5f;

            audioSource.SetAudioSample(audio.Value);
            audioSource.Play();

        }

        public void Unload() { }

        private class AudioSourceRotation(Entity owner) : Component(owner)
        {
            public float Speed = 3;
            public float Radius = 4.2f;
            private float angle = 0;

            public override void Update(float deltaTime)
            {
                float x = MathF.Cos(angle) * Radius;
                float y = MathF.Sin(angle) * Radius;

                Owner.Position = new Vector2(x, y);

                angle += Speed * deltaTime;
            }
        }
    }
}
