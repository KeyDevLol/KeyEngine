using KeyEngine.Audio;

namespace KeyEngine
{
    public class AudioScene : IScene
    {
        public void Load()
        {
            Entity audioSourceEntity = ECS.AddEntity("AudioSource");
            AudioSource audioSource = audioSourceEntity.AddComponent<AudioSource>();
            audioSource.AudioSample = new AudioSample("Assets/Audio/Vigilantism.wav");
            audioSource.Play();
        }

        public void Unload() { }
    }
}
