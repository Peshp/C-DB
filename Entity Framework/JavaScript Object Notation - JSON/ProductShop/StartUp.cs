using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();


            //01
            string jsonUsers = File.ReadAllText(@"../../../Datasets/users.json");
            Console.WriteLine(ImportUsers(context, jsonUsers));

            //02
            string jsonProducts = File.ReadAllText(@"../../../Datasets/products.json");
            Console.WriteLine(ImportProducts(context, jsonProducts));

            //03
            string jsonCategories = File.ReadAllText(@"../../../Datasets/categories.json");
            Console.WriteLine(ImportCategories(context, jsonCategories));

            //04
            string jsonCategoryProducts = File.ReadAllText(@"../../../Datasets/categories-products.json");
            Console.WriteLine(ImportCategoryProducts(context, jsonCategoryProducts));

            //05
            Console.WriteLine(GetProductsInRange(context));
            File.WriteAllText(@"../../../Results/products-in-range.json", GetProductsInRange(context));

            //06
            Console.WriteLine(GetSoldProducts(context));
            File.WriteAllText(@"../../../Results/users-sold-products.json", GetSoldProducts(context));

            //07
            Console.WriteLine(GetCategoriesByProductsCount(context));
            File.WriteAllText(@"../../../Results/categories-by-products.json", GetCategoriesByProductsCount(context));

            //08
            Console.WriteLine(GetUsersWithProducts(context));
            File.WriteAllText(@"../../../Results/users-and-products.json", GetUsersWithProducts(context));
        }
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            List<User> users = JsonConvert.DeserializeObject<List<User>>(inputJson)!;

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(inputJson)!;

            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson)!
                .Where(c => c.Name != null)
                .ToList();

            context.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoryProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson)!;

            context.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
            .Where(p => p.Price >= 500 && p.Price <= 1000)
            .Select(p => new
            {
                name = p.Name,
                price = p.Price,
                seller = p.Seller.FirstName + " " + p.Seller.LastName
            })
            .OrderBy(p => p.price)
            .AsNoTracking()
            .ToArray();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(x => x.ProductsSold.Any(p => p.BuyerId != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold
                        .Select(ps => new
                        {
                            name = ps.Name,
                            price = ps.Price,
                            buyerFirstName = ps.Buyer.FirstName,
                            buyerLastName = ps.Buyer.LastName
                        })
                })
                .OrderBy(x => x.lastName).ThenBy(x => x.firstName)
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(users, Formatting.Indented);
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories               
                .Select(c => new
                {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count,
                    averagePrice = Math.Round((double)c.CategoriesProducts.Average(cp => cp.Product.Price), 2),
                    totalRevenue = Math.Round((double)c.CategoriesProducts.Sum(cp => cp.Product.Price), 2)
                })
                .OrderByDescending(c => c.productsCount)
                .AsNoTracking()
                .ToArray();

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersWithProducts = context.Users
                .Where(x => x.ProductsSold.Any(p => p.BuyerId != null))
                .OrderByDescending(x => x.ProductsSold.Where(p => p.BuyerId != null).Count())
                .Select(x => new
                {
                    lastName = x.LastName,
                    age = x.Age,
                    soldProducts = new
                    {
                        count = x.ProductsSold
                            .Where(p => p.BuyerId != null).Count(),
                        products = x.ProductsSold
                            .Where(p => p.BuyerId != null)
                            .Select(p => new
                            {
                                name = p.Name,
                                price = p.Price
                            })
                    }
                })
                .ToArray();

            var usersInfo = new
            {
                usersCount = usersWithProducts.Count(),
                users = usersWithProducts
            };

            return JsonConvert.SerializeObject(usersInfo, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}