namespace Theatre.DataProcessor
{
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;
    using Theatre.Data;
    using Theatre.Data.Models;
    using Theatre.Data.Models.Enums;
    using Theatre.DataProcessor.ImportDto;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfulImportPlay
            = "Successfully imported {0} with genre {1} and a rating of {2}!";

        private const string SuccessfulImportActor
            = "Successfully imported actor {0} as a {1} character!";

        private const string SuccessfulImportTheatre
            = "Successfully imported theatre {0} with #{1} tickets!";



        public static string ImportPlays(TheatreContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPlaysDto[]),
                new XmlRootAttribute("Plays"));
            var plays = (ImportPlaysDto[])xmlSerializer.Deserialize(new StringReader(xmlString))!;

            foreach ( var xmlPlay in plays )
            {
                if(!IsValid(xmlPlay))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                bool isDurationValid = TimeSpan.TryParseExact(xmlPlay.Duration,"c", CultureInfo.InvariantCulture
                    ,out TimeSpan validDuration);
                bool isGenreValid = Enum.TryParse<Genre>(xmlPlay.Genre, out Genre validGenre);

                if(!isDurationValid || !isGenreValid || validDuration < TimeSpan.FromHours(1))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Play play = new Play
                {
                    Title = xmlPlay.Title,
                    Duration = validDuration,
                    Rating = xmlPlay.Rating,
                    Genre = validGenre,
                    Description = xmlPlay.Description,
                    Screenwriter = xmlPlay.Screenwriter,
                };

                context.Plays.Add(play);
                output.AppendLine(String.Format(SuccessfulImportPlay, play.Title, play.Genre, play.Rating)); ;
            }
            context.SaveChanges();
            return output.ToString().TrimEnd();
        }

        public static string ImportCasts(TheatreContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCastsDto[]),
                new XmlRootAttribute("Casts"));
            var casts = (ImportCastsDto[])xmlSerializer.Deserialize(new StringReader(xmlString))!;

            foreach ( var xmlCast in casts ) 
            {
                if (!IsValid(xmlCast))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Cast cast = new Cast
                {
                    FullName = xmlCast.FullName,
                    IsMainCharacter = xmlCast.IsMainCharacter,
                    PhoneNumber = xmlCast.PhoneNumber,
                    PlayId = xmlCast.PlayId,
                };

                context.Casts.Add(cast);
                string isMainCharValue = cast.IsMainCharacter ? "main" : "lesser";
                output.AppendLine(String.Format(SuccessfulImportActor, cast.FullName, isMainCharValue));
            }
            context.SaveChanges();
            return output.ToString().TrimEnd();
        }

        public static string ImportTtheatersTickets(TheatreContext context, string jsonString)
        {
            StringBuilder output = new StringBuilder();

            ImportTheatresDto[] theatres = JsonConvert.DeserializeObject<ImportTheatresDto[]>(jsonString)!;
            foreach (var xmlTheatre in theatres)
            {
                if(!IsValid(xmlTheatre))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Theatre theatre = new Theatre
                {
                    Name = xmlTheatre.Name,
                    NumberOfHalls = xmlTheatre.NumberOfHalls,
                    Director = xmlTheatre.Director,
                };

                foreach (var xmlTicket in xmlTheatre.Tickets)
                {
                    if(!IsValid(xmlTicket))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    Ticket ticket = new Ticket
                    {
                        Price = xmlTicket.Price,
                        RowNumber = xmlTicket.RowNumber,
                        PlayId = xmlTicket.PlayId,
                    };

                    theatre.Tickets.Add(ticket);
                }
                context.Theatres.Add(theatre);
                output.AppendLine(String.Format(SuccessfulImportTheatre, theatre.Name, theatre.Tickets.Count));
            }
            context.SaveChanges();
            return output.ToString().TrimEnd();
        }


        private static bool IsValid(object obj)
        {
            var validator = new ValidationContext(obj);
            var validationRes = new List<ValidationResult>();

            var result = Validator.TryValidateObject(obj, validator, validationRes, true);
            return result;
        }
    }
}
