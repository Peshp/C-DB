namespace MusicHub
{
    using System;
    using System.Text;
    using System.Xml.Linq;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            DbInitializer.ResetDatabase(context);

            Console.WriteLine(ExportSongsAboveDuration(context, 4)); 
        }

        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            StringBuilder sb = new StringBuilder();

            var producers = context.Producers
                .First(p => p.Id == producerId)
                .Albums                 
                .Select(a => new
                {
                    a.Name,
                    ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                    ProducerName = a.Producer.Name,                   
                    Songs = a.Songs.Select(s => new
                    {
                        s.Name,
                        s.Price,
                        SongWriterName = s.Writer.Name
                    }).OrderByDescending(s => s.Name).ThenBy(s => s.SongWriterName).ToArray(),
                    TotalAlbumPrice = a.Price
                }).OrderByDescending(a => a.TotalAlbumPrice).ToArray();

            int count = 0;
            foreach (var a in producers)
            {
                sb.AppendLine($"-AlbumName: {a.Name}");
                sb.AppendLine($"-ReleaseDate: {a.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {a.ProducerName}");
                sb.AppendLine($"-Songs:");
                foreach (var s in a.Songs)
                {
                    count++;
                    sb.AppendLine($"---#{count}");
                    sb.AppendLine($"---SongName: {s.Name}");
                    sb.AppendLine($"---Price: {s.Price:f2}");
                    sb.AppendLine($"---Writer: {s.SongWriterName}");                   
                }
                sb.AppendLine($"-AlbumPrice: {a.TotalAlbumPrice:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {
            StringBuilder sb = new StringBuilder();

            var songs = context.Songs
                .Where(s => s.Duration.TotalSeconds > duration)
                .Select(s => new
                {
                    s.Name,
                    PerformerFullName = s.SongPerformers
                    .Select(sp => sp.Performer.FirstName + " " + sp.Performer.LastName)
                    .OrderBy(name => name)
                    .ToList(),

                    WriterName = s.Writer.Name,
                    AlbumProducer = s.Album.Producer.Name,
                    Duration = s.Duration.ToString("c")
                }).OrderBy(s => s.Name).ThenBy(s => s.WriterName).ToList();

            int count = 0;
            foreach (var s in songs)
            {
                count++;
                sb.AppendLine($"-Song #{count}");
                sb.AppendLine($"---SongName: {s.Name}");
                sb.AppendLine($"---Writer: {s.WriterName}");

                if(s.PerformerFullName.Any())
                {
                    sb.AppendLine(string
                        .Join("\n", s.PerformerFullName
                        .Select(p => $"---Performer: {p}")));
                }

                sb.AppendLine($"---AlbumProducer: {s.AlbumProducer}");
                sb.AppendLine($"---Duration: {s.Duration}");
            }

            return sb.ToString().TrimEnd();
        }
    }
}
