using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Markup.Localizer;
using FluentScheduler;

namespace Player
{
    /// <summary>
    /// Медиа-контролер, управляет воспроизведением по расписанию.
    /// </summary>
    class MediaController :IDisposable
    {
        // инкапсулирована логика работы по расписанию


        private IMediaEngine mediaEngine;

        private MediaPlaylist background;
        private MediaPlaylist interrupt;
        private bool disposedValue;

        public MediaController(IMediaEngine mediaEngine)
        {
            this.mediaEngine = mediaEngine;
            this.background = new MediaPlaylist(mediaEngine, true);
            this.interrupt = new MediaPlaylist(mediaEngine, false);
        }

        /// <summary>
        /// Загрузка расписания из строк
        /// </summary>
        /// <param name="shedule">Строки с конфигурацией</param>
        public void LoadShedule(IEnumerable<string> shedule)
        {
            //парсим строки
            var sh = shedule
                .Where(arg=>arg!=string.Empty)
                .Select(arg => new SheduleItem(arg))
                .OrderBy(arg => arg.Start)
                .ToArray();

            // проверяем пересечение периодов
            foreach(var item in sh.Where(arg=>arg.Type == MediaTypes.Background))
            {
                if (sh.Any(arg => item.Start < arg.End && item.End > arg.Start && arg != item))
                    throw new ApplicationException("Пересечение периодов");
            }

            //программируем лпанировщик задач
            JobManager.Stop();

            var registry = new Registry();

            foreach (var item in sh)
            {
                switch (item.Type)
                {
                    case MediaTypes.Background:
                        registry.Schedule(() => UpdateBackground(item.Path, true)).ToRunEvery(1).Days().At(item.Start.Hour, item.Start.Minute);
                        registry.Schedule(() => mediaEngine.Stop()).ToRunEvery(1).Days().At(item.End.Hour, item.End.Minute);
                        break;
                    case MediaTypes.Interrupt:
                        registry.Schedule(() => SetInterrupt(item.Path)).ToRunEvery(1).Days().At(item.Start.Hour, item.Start.Minute);
                        break;
                }
            }

            JobManager.Initialize(registry);

            //запускаем текущее событие, если нужно

            var currentBg = sh.FirstOrDefault(arg => arg.Start <= DateTime.Now && arg.End >= DateTime.Now);
            if (currentBg == null)
                return;

            UpdateBackground(currentBg.Path);
            if (!mediaEngine.HasFileOpened)
            {
                background.PlayNext();
            }
        }

        private void UpdateBackground(string path, bool playNext=false)
        {

            var dir = new DirectoryInfo(path);
            background.Update(dir.GetFiles());

            // если указано принудительное вопроизведение следующего файла, 
            // проверяем, чтобы в этот момент не было Interrupt
            if (playNext)
            {
                if (interrupt.IsEmpty)
                    background.PlayNext();
                else
                    background.Reset();
            }
        }


        private void SetInterrupt(string path)
        {
            // ставим Background на паузу и заводим Interrupt
            background.Pause();
            var dir = new DirectoryInfo(path);
            interrupt.Update(dir.GetFiles());
            interrupt.PlaylistEnded += Interrupt_PlaylistEnded;
            interrupt.PlayNext();
        }

        private void Interrupt_PlaylistEnded(object sender, EventArgs e)
        {
            // восстанавливаем Background
            interrupt.PlaylistEnded -= Interrupt_PlaylistEnded;
            interrupt = null;
            background.Continue();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    background.Dispose();
                    interrupt.Dispose();
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
