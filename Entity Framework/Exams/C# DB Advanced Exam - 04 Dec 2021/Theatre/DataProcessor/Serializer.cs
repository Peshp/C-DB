namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.DataProcessor.ExportDto;

    public class Serializer
    {
        public static string ExportTheatres(TheatreContext context, int numbersOfHalls)
        {
            var theatres = context.Theatres
                .Where(t => t.NumberOfHalls >= numbersOfHalls &&
                 t.Tickets.Count >= 20)
                .OrderByDescending(t => t.NumberOfHalls)
                .ThenBy(t => t.Name)
                .Select(t => new ExportTheatersDto()
                {
                    Name = t.Name,
                    Halls = t.NumberOfHalls,
                    TotalIncome = t.Tickets.Where(tc => tc.RowNumber > 0 && tc.RowNumber <= 5)
                                  .Sum(tc => tc.Price),
                    Tickets = t.Tickets.Where(tc => tc.RowNumber > 0 && tc.RowNumber <= 5)
                    .Select(tc => new ExportTicketsDto()
                    {
                        Price = tc.Price,
                        RowNumber = tc.RowNumber,
                    })
                    .OrderByDescending(tc => tc.Price)
                    .ToArray()
                }).ToArray();

            return JsonConvert.SerializeObject(theatres, Formatting.Indented);
        }

        public static string ExportPlays(TheatreContext context, double raiting)
        {
            ExportPlaysDto[] plays = context.Plays
                .Where(p => p.Rating <= raiting)
                .OrderBy(p => p.Title)
                .ThenByDescending(p => p.Genre)
                .Select(p => new ExportPlaysDto()
                {
                    Title = p.Title,
                    Duration = p.Duration.ToString("c"),
                    Rating = p.Rating == 0 ? "Premier" : p.Rating.ToString(),
                    Genre = p.Genre.ToString(),
                    Actors = p.Casts.Where(c => c.IsMainCharacter)
                    .Select(c => new ExportActorsDto()
                    {
                        FullName = c.FullName,
                        MainCharacter = $"Plays main character in '{p.Title}'."
                    })
                    .OrderByDescending(c => c.FullName)
                    .ToArray()
                })
                .ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportPlaysDto[]),
                new XmlRootAttribute("Users"));
            StringWriter reader = new StringWriter();
            xmlSerializer.Serialize(reader, plays);
            return reader.ToString();
        }
    }
}
