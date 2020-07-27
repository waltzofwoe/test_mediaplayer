using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Documents;

namespace Player
{
    /// <summary>
    /// Плейлист. 
    /// </summary>
    class MediaPlaylist : IDisposable
    {
        // инкапсулирует логику воспроизведения списка файлов


        private IMediaEngine engine;

        private Queue<FileInfo> playlist = new Queue<FileInfo>();

        private FileInfo currentFile;
        private TimeSpan currentPosition;

        private bool repeat;

        public event EventHandler PlaylistEnded;

        private bool disposedValue;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="engine">Медиа-движок</param>
        /// <param name="repeat">Повторять вопроизведение по кругу</param>
        public MediaPlaylist(IMediaEngine engine, bool repeat)
        {
            this.engine = engine;
            this.repeat = repeat;
            this.engine.MediaEnded += MediaEndedHandler;
        }

        /// <summary>
        /// Плейлист пуст
        /// </summary>
        public bool IsEmpty => playlist.Count == 0 && currentFile == null;

        /// <summary>
        /// Сбросить параметры текущего файла
        /// </summary>
        public void Reset()
        {
            currentFile = null;
            currentPosition = TimeSpan.Zero;
        }

        /// <summary>
        /// Поставить воспроизведение на паузу
        /// </summary>
        public void Pause()
        {
            engine.Pause();
            //отписываемся от события, чтобы наш текущий файл не менялся
            engine.MediaEnded -= MediaEndedHandler;
            //сохраняем текущую позицию в текущем файле
            currentPosition = engine.GetPosition();
        }

        /// <summary>
        /// Продолжить воспроизведение
        /// </summary>
        public void Continue()
        {
            //возвращаем подписку на событие
            engine.MediaEnded += MediaEndedHandler;

            //ставим на воспроизведение текущий файл, или следующий, если такового нет
            if (currentFile == null)
                PlayNext();
            else
            {
                engine.Open(currentFile);
                engine.Play(currentPosition);
            }
        }

        /// <summary>
        /// Обновить содержимое плейлиста
        /// </summary>
        /// <param name="files"></param>
        public void Update(IEnumerable<FileInfo> files)
        {
            var orderedFiles = files.OrderBy(arg => arg.Name);
            playlist = new Queue<FileInfo>(orderedFiles);
        }

        private void MediaEndedHandler(object o, EventArgs e)
        {
            PlayNext();
        }

        /// <summary>
        /// Воспроизвести следующий файл
        /// </summary>
        public void PlayNext()
        {
            if (playlist.Count == 0)
            {
                // если больше нечего вопроизводить - отправить сигнал об этом
                currentFile = null;
                PlaylistEnded?.Invoke(this, EventArgs.Empty);
            }

            else
            {
                // если список воспроизведения циклический, то ставим файл обратно в очередь
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
