using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Player
{
    /// <summary>
    /// Реализация медиа-движка на основе WPF MediaElement.
    /// </summary>
    class MediaEngine : IMediaEngine
    {
        // инкапсулирует MediaElement
        // дает возможность управлять воспроизведением из разных потоков
        
        
        private MediaElement player;

        public MediaEngine(MediaElement player)
        {
            this.player = player;
            player.LoadedBehavior = MediaState.Manual;
            player.MediaEnded += (o, e) => MediaEnded(this, EventArgs.Empty);
        }

        public event EventHandler MediaEnded;

        public void Open(FileInfo file)
        {
            player.Dispatcher.Invoke(() =>
            {
                var uri = new Uri(file.FullName);
                player.Source = uri;
            });
        }

        public bool HasFileOpened => player.Source != null;

        public void Pause()
        {
            player.Dispatcher.Invoke(() => player.Pause());
        }

        public void Stop()
        {
            player.Dispatcher.Invoke(() => { player.Source = null; player.Close(); });

        }

        public void Play()
        {
            player.Dispatcher.Invoke(() =>
            {
                player.Position = TimeSpan.Zero;
                player.Play();
            });
        }
        public void Play(TimeSpan position)
        {
            player.Dispatcher.Invoke(() =>
            {
                player.Position = position;
                player.Play();
            });
        }

        public TimeSpan GetPosition()
        {
            TimeSpan res = TimeSpan.Zero;

            player.Dispatcher.Invoke(() =>
            {
                res = player.Position;
            });
            return res;
        }
    }
}
