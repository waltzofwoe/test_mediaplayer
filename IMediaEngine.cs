using System;
using System.IO;

namespace Player
{
    interface IMediaEngine
    {
        bool HasFileOpened { get; }

        event EventHandler MediaEnded;

        TimeSpan GetPosition();
        void Open(FileInfo file);
        void Pause();
        void Play();
        void Play(TimeSpan position);
        void Stop();
    }
}