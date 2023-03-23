namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);

            Console.WriteLine(RemoveBooks(db));
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var enumValue = Enum.Parse<AgeRestriction>(command, true);

            var titles = context.Books
                .Where(b => b.AgeRestriction == enumValue)
                .Select(b => b.Title)
                .OrderBy(t => t)
                .ToArray();

            return string.Join("\n", titles);
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.EditionType == EditionType.Gold &&
                        b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title);
            
            return string.Join("\n", books);
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new { b.Title, b.Price });

            foreach (var b in books)
                sb.AppendLine($"{b.Title} - ${b.Price:f2}");

            return sb.ToString().TrimEnd();
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title);

            return string.Join("\n", books);
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            var categoris = input.Split().ToArray();

            var books = context.BooksCategories
                .Where(b => categoris.Contains(b.Category.Name.ToLower()))
                .Select(b => b.Book.Title)
                .OrderBy(t => t)
                .ToArray();

            return string.Join("\n", books);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            StringBuilder sb = new StringBuilder();

            DateTime parseDate = DateTime.ParseExact(date, "dd-MM-yyyy", null);

            var books = context.Books
                .Where(b => b.ReleaseDate < parseDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    EditionType = b.EditionType.ToString(),
                    b.Price
                });

            foreach (var b in books)
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:f2}");

            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => a.FirstName + " " + a.LastName)
                .OrderBy(t => t);

            return string.Join("\n", authors);
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => b.Title)
                .OrderBy(t => t);

            return string.Join("\n", books);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();

            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new
                {
                    b.Title,
                    Author = b.Author.FirstName + " " + b.Author.LastName
                });

            foreach (var b in books)
                sb.AppendLine($"{b.Title} ({b.Author})");

            return sb.ToString().TrimEnd();
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var titlesLenght = context.Books
                .Where(b => b.Title.Length > lengthCheck);

            return titlesLenght.Count();
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var authors = context.Authors
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName,
                    Count = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.Count);

            foreach (var a in authors)
                sb.AppendLine($"{a.FullName} - {a.Count}");

            return sb.ToString().TrimEnd();
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories
                .OrderBy(c => c.Name)
                .Select(c => new
                {
                    c.Name,
                    Book = c.CategoryBooks
                        .OrderByDescending(cb => cb.Book.ReleaseDate)
                        .Select(cb => new
                        {
                            cb.Book.Title,
                            Year = cb.Book.ReleaseDate.Value.Year
                        })
                        .Take(3)
                }).ToArray();

            foreach (var c in categories)
            {
                sb.AppendLine($"--{c.Name}");
                foreach (var b in c.Book)
                    sb.AppendLine($"{b.Title} ({b.Year})");
            }

            return sb.ToString().TrimEnd();
        }

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var b in books)
                b.Price *= 5;
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var booksToRemove = context
                .Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            context.RemoveRange(booksToRemove);
            context.SaveChanges();

            return booksToRemove.Count();
        }
    }
}


