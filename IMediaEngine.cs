using System;
using System.IO;

namespace Player
{
    /// <summary>
    /// Медиа-движок
    /// </summary>
    interface IMediaEngine
    {
        /// <summary>
        /// Открыт файл
        /// </summary>
        bool HasFileOpened { get; }

        /// <summary>
        /// Событие, возникающее при окончании воспроизведения файла
        /// </summary>
        event EventHandler MediaEnded;

        /// <summary>
        /// Текущая позиция в файле
        /// </summary>
        /// <returns></returns>
        TimeSpan GetPosition();

        /// <summary>
        /// Открыть файл для воспроизведения
        /// </summary>
        /// <param name="file"></param>
        void Open(FileInfo file);

        /// <summary>
        /// Пауза
        /// </summary>
        void Pause();

        /// <summary>
        /// Возспроизвести с начала
        /// </summary>
        void Play();

        /// <summary>
        /// Воспроизвести с указанного места
        /// </summary>
        /// <param name="position"></param>
        void Play(TimeSpan position);

        /// <summary>
        /// Остановить и выгрузить файл
        /// </summary>
        void Stop();
    }
}