using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Documents;

namespace Player
{
    class MediaPlaylist : IDisposable
    {
        private IMediaEngine engine;

        private Queue<FileInfo> playlist = new Queue<FileInfo>();

        private FileInfo currentFile;
        private TimeSpan currentPosition;

        private bool repeat;

        public event EventHandler PlaylistEnded;

        private bool disposedValue;


        public MediaPlaylist(IMediaEngine engine, bool repeat)
        {
            this.engine = engine;
            this.repeat = repeat;
            this.engine.MediaEnded += MediaEndedHandler;
        }

        public bool IsEmpty => playlist.Count == 0 && currentFile == null;

        public void Reset()
        {
            currentFile = null;
            currentPosition = TimeSpan.Zero;
        }

        public void Pause()
        {
            engine.Pause();
            engine.MediaEnded -= MediaEndedHandler;
            currentPosition = engine.GetPosition();
        }

        public void Continue()
        {
            engine.MediaEnded += MediaEndedHandler;
            if (currentFile == null)
                PlayNext();
            else
            {
                engine.Open(currentFile);
                engine.Play(currentPosition);
            }
        }

        public void Update(IEnumerable<FileInfo> files)
        {
            var orderedFiles = files.OrderBy(arg => arg.Name);
            playlist = new Queue<FileInfo>(orderedFiles);
        }

        private void MediaEndedHandler(object o, EventArgs e)
        {
            PlayNext();
        }

        public void PlayNext()
        {
            if (playlist.Count == 0)
            {
                currentFile = null;
                PlaylistEnded?.Invoke(this, EventArgs.Empty);
            }

            else
            {
                if (repeat && currentFile != null)
                {
                    playlist.Enqueue(currentFile);
                }
                currentFile = playlist.Dequeue();
                engine.Open(currentFile);
                engine.Play();
                
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    engine.Stop();
                    engine.MediaEnded -= MediaEndedHandler;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
