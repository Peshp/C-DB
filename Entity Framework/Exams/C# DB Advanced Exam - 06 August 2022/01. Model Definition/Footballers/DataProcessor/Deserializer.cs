namespace Footballers.DataProcessor
{
    using Footballers.Data;
    using Footballers.Data.Models;
    using Footballers.Data.Models.Enums;
    using Footballers.DataProcessor.ImportDto;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedCoach
            = "Successfully imported coach - {0} with {1} footballers.";

        private const string SuccessfullyImportedTeam
            = "Successfully imported team - {0} with {1} footballers.";

        public static string ImportCoaches(FootballersContext context, string xmlString)
        {
            StringBuilder output = new StringBuilder();
            
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCoaches[]), 
                new XmlRootAttribute("Coaches"));
            var coaches = (ImportCoaches[])xmlSerializer.Deserialize(new StringReader(xmlString))!;

            foreach ( var xmlCoache in coaches )
            {
                if(!IsValid(xmlCoache) || String.IsNullOrEmpty(xmlCoache.Nationality))
                {
                    output.AppendLine(ErrorMessage);
                    continue;
                }

                Coach coach = new Coach
                {
                    Name = xmlCoache.Name,
                    Nationality = xmlCoache.Nationality,
                };

                foreach (var xmlFoot in xmlCoache.Footballers)
                {
                    if(!IsValid(xmlFoot))
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isStartDateValid = DateTime.TryParseExact(xmlFoot.ContractStartDate, "dd/mm/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime ValidStartDate);
                    bool isEndDateValid = DateTime.TryParseExact(xmlFoot.ContractEndDate, "dd/mm/yyyy",
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime ValidEndDate);

                    if(!isStartDateValid || !isEndDateValid || ValidEndDate < ValidStartDate)
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    Footballer foot = new Footballer
                    {
                        Name = xmlFoot.Name,
                        ContractStartDate = ValidStartDate,
                        ContractEndDate = ValidEndDate,
                        BestSkillType = (BestSkillType)xmlFoot.BestSkillType,
                        PositionType = (PositionType)xmlFoot.PositionType,
                        Coach = coach,
                    };

                    coach.Footballers.Add(foot);
                }
                context.Coaches.Add(coach);
                output.AppendLine(String.Format(SuccessfullyImportedCoach, coach.Name, coach.Footballers.Count));
            }
            context.SaveChanges();
            return output.ToString().TrimEnd();
        }

        public static string ImportTeams(FootballersContext context, string jsonString)
        {            
            StringBuilder output = new StringBuilder();

            ImportTeams[] teams = JsonConvert.DeserializeObject<ImportTeams[]>(jsonString)!;
            List<Team> outTeams = new List<Team>();
            foreach (var jsonTeam in teams)
            { 
                if(!IsValid(jsonTeam))
                {
                    output.AppendLine(ErrorMessage); 
                    continue;
                }

                Team team = new Team()
                {
                    Name = jsonTeam.Name,
                    Nationality = jsonTeam.Nationality,
                    Trophies = jsonTeam.Trophies,
                };

                foreach (var jsonFoot in jsonTeam.Footballers.Distinct())
                {
                    Footballer foorballer = context.Footballers.Find(jsonFoot);

                    if (foorballer == null) 
                    {
                        output.AppendLine(ErrorMessage);
                        continue;
                    }

                    team.TeamsFootballers.Add(new TeamFootballer()
                    {
                        Footballer = foorballer,
                        Team = team,
                    });
                }
                outTeams.Add(team);
                output.AppendLine(String.Format(SuccessfullyImportedTeam, team.Name, team.TeamsFootballers.Count));
            }
            context.AddRange(outTeams);
            context.SaveChanges();
            return output.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}
