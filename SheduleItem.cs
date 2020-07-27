using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;

namespace Player
{
    class SheduleItem
    {
        public string Type { get; }
        public DateTime Start { get; }
        public DateTime End { get; }
        public string Path { get; }

        public SheduleItem(string configLine)
        {
            var parts = configLine.Split();

            switch (parts[0])
            {
                case MediaTypes.Background:
                    Type = MediaTypes.Background;
                    Start = DateTime.ParseExact(parts[1], "HH:mm", CultureInfo.InvariantCulture);
                    End = DateTime.ParseExact(parts[2], "HH:mm", CultureInfo.InvariantCulture);
                    Path = string.Join(" ", parts.Skip(3));
                    break;

                case MediaTypes.Interrupt:
                    Type = MediaTypes.Interrupt;
                    Start = DateTime.ParseExact(parts[1], "HH:mm", CultureInfo.InvariantCulture);
                    Path = string.Join(" ", parts.Skip(2));
                    break;

                default:
                    throw new ApplicationException($"Неизвестный тип события: {parts[0]}");
            }

            if (!Directory.Exists(Path))
            {
                throw new ApplicationException($"Указанный путь не существует: {Path}");
            }

        }

    }
}
