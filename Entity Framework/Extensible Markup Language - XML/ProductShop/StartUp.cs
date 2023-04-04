using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

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
            string xmlUsers = File.ReadAllText(@"../../../Datasets/users.xml");
            Console.WriteLine(ImportUsers(context, xmlUsers));

            //02
            string xmlProducts = File.ReadAllText(@"../../../Datasets/products.xml");
            Console.WriteLine(ImportProducts(context, xmlProducts));

            //03
            string xmlCategories = File.ReadAllText(@"../../../Datasets/categories.xml");
            Console.WriteLine(ImportCategories(context, xmlCategories));

            //04
            string xmlCategoryProducts = File.ReadAllText(@"../../../Datasets/categories-products.xml");
            Console.WriteLine(ImportCategoryProducts(context, xmlCategoryProducts));

            //05
            Console.WriteLine(GetProductsInRange(context));
            File.WriteAllText(@"../../../Results/products-in-range.xml", GetProductsInRange(context));

            //06
            Console.WriteLine(GetSoldProducts(context));
            File.WriteAllText(@"../../../Results/users-sold-products.xml", GetSoldProducts(context));

            //07
            Console.WriteLine(GetCategoriesByProductsCount(context));
            File.WriteAllText(@"../../../Results/categories-by-products.xml", GetCategoriesByProductsCount(context));

            //08
            //Console.WriteLine(GetUsersWithProducts(context));
            //File.WriteAllText(@"../../../Results/users-and-products.json", GetUsersWithProducts(context));
        }
        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            T dtos = (T)serializer.Deserialize(reader)!;
            return dtos;
        }
        private static string Serialize<T>(T dataTransferObject, string rootName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));

            StringBuilder sb = new StringBuilder();
            using var write = new StringWriter(sb);

            XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();
            xmlNamespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(write, dataTransferObject, xmlNamespaces);

            return sb.ToString();
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            var usersDto = Deserialize<importUsersDto[]>(inputXml, "Users");

            List<User> users = usersDto
                .Select(u => new User()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age
                })
                .ToList();

            context.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            var productsDto = Deserialize<ImportProductsDto[]>(inputXml, "Products");

            List<Product> products = productsDto
                .Select(p => new Product()
                {
                    Name = p.Name,
                    Price = p.Price,
                    SellerId = p.SellerId,
                    BuyerId = p.BuyerId == 0 ? null : p.BuyerId
                })
                .ToList();

            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            var categoriesDto = Deserialize<importCategoriesDto[]>(inputXml, "Categories");

            var categories = categoriesDto
                .Where(c => c.Name != null)
                .Select(c => new Category()
                {
                    Name = c.Name
                })
                .ToList();

            context.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            var categoryProductsDto = Deserialize<CategoryProductDto[]>(inputXml, "CategoryProducts");

            var categoryProducts = categoryProductsDto
                .Select(cp => new CategoryProduct()
                {
                    CategoryId = cp.CategoryId,
                    ProductId = cp.ProductId
                })
                .ToList();

            context.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .Select(p => new ExportProductsInRangeDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                })               
                .ToArray();

            return Serialize<ExportProductsInRangeDto[]>(products, "Products");
        }
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(x => x.BuyerId != null))
                .OrderBy(u => u.LastName).ThenBy(u => u.FirstName)
                .Take(5)
                .Select(u => new ExportSoldProducts()
                {
                    FirstName = u.FirstName!,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Select(ps => new ProductDto()
                        {
                            Name = ps.Name,
                            Price = ps.Price
                        })
                        .ToArray()
                })
                .ToArray();

            return Serialize<ExportSoldProducts[]>(users, "Users");
        }
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            ExportCategoriesProductsDto[] categoriesProduct = context
                .Categories
                .Select(c => new ExportCategoriesProductsDto()
                {
                    Name = c.Name,
                    Count = c.CategoriesProducts.Count(),
                    AveragePrice = c.CategoriesProducts.Average(p => p.Product.Price),
                    TotalRevenue = c.CategoriesProducts.Sum(p => p.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            return Serialize<ExportCategoriesProductsDto[]>(categoriesProduct, "CategoryProducts");
        }
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var userProducts = context.Users
                .Where(u => u.ProductsSold.Any())
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new ExportUsersProductsDto()
                {
                    FirstName = u.FirstName!,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new SoldProductsDto()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(p => new ProductsDto()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .Take(10)
                .ToArray();

            ExportUsersProductsDto exportUserCountDto = new ExportUserCountDto()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = usersInfo
            };

            return Serializer<ExportUsersProductsDto(exportUserCountDto, "Users");
        }
    }
}