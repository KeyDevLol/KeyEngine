using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyEngine;
using SFML.Graphics;

namespace KeyEngine
{
    class Animator : Component
    {
        public List<Texture> frames;
        private Stopwatch _timer;

        public bool IsStarted { get; private set; }
        public bool loop = false;

        public int CurrentFrame { get; private set; }

        public ushort framesPerSecond = 24;

        public override void OnObjectDestroy()
        {
            //Console.WriteLine("Аниматор, уничтожен.");

            //Reset();

            //for (int i = 0; i < frames.Count; i++)
            //    frames[i].Dispose();

            //frames.Clear();

            //frames = null;
            //_timer = null;
        }

        public Animator(ushort framesPerSecond, Texture[] frames, bool loop = true)
        {
            Console.WriteLine("Инициализация");

            _timer = new Stopwatch();

            this.loop = loop;
            this.frames = frames.ToList();
            this.framesPerSecond = (ushort)(framesPerSecond / this.frames.Count);
        }

        ~Animator()
        {

        }

        public new void Start()
        {
            if (frames.Count == 0)
            {
                Console.WriteLine("Кадров в аниматоре нет! Невозможно проиграть анимацию.");
                return;
            }

            if (IsStarted)
                Stop();

            Console.WriteLine("Запущена анимация");

            IsStarted = true;

            if (!loop)
            {
                CurrentFrame++;
                gameObject.shape.Texture = frames[CurrentFrame];
            }
            if (_timer != null)
                _timer.Restart();
        }

        public void Stop()
        {
            if (IsStarted == false)
                return;

            IsStarted = false;

            Console.WriteLine("Остановлена анимация");
    
            Reset();
        }

        public void Pause()
        {
            IsStarted = false;

            if (_timer != null)
                _timer.Stop();
        }
        
        public void Unpause()
        {
            IsStarted = true;

            if (_timer != null)
                _timer.Start();
        }

        private void Reset()
        {
            if (_timer != null)
                _timer.Reset();

            CurrentFrame = -1;
        }

        public override void Update()
        {
            if (IsStarted)
            {
                if (_timer.ElapsedMilliseconds >= framesPerSecond && CurrentFrame <= frames.Count - 1)
                {
                    CurrentFrame++;
                    Console.WriteLine($"Кадр {CurrentFrame}");

                    gameObject.shape.Texture = frames[CurrentFrame];
                    _timer.Restart();
                }
                else if (CurrentFrame >= frames.Count - 1)
                {
                    if (loop)
                        Start();
                    else
                        Stop();
                }
            }
        }
    }
}
