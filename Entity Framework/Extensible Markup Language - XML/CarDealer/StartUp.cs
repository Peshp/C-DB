using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Newtonsoft.Json;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //09
            string suppliersXml = File.ReadAllText(@"../../../Datasets/suppliers.xml");
            Console.WriteLine(ImportSuppliers(context, suppliersXml));

            //10
            string partsJson = File.ReadAllText(@"../../../Datasets/parts.json");
            Console.WriteLine(ImportParts(context, partsJson));

            //11
            string carsJson = File.ReadAllText(@"../../../Datasets/cars.json");
            Console.WriteLine(ImportCars(context, carsJson));

            //12
            string customersJson = File.ReadAllText(@"../../../Datasets/customers.json");
            Console.WriteLine(ImportCustomers(context, customersJson));

            //13
            string salesJson = File.ReadAllText(@"../../../Datasets/sales.json");
            Console.WriteLine(ImportSales(context, salesJson));

            //14
            Console.WriteLine(GetOrderedCustomers(context));
            File.WriteAllText(@"../../../Results/ordered-customers.json", GetOrderedCustomers(context));

            //15
            Console.WriteLine(GetCarsFromMakeToyota(context));
            File.WriteAllText(@"../../../Results/toyota-cars.json", GetCarsFromMakeToyota(context));

            //16
            Console.WriteLine(GetLocalSuppliers(context));
            File.WriteAllText(@"../../../Results/local-suppliers.json", GetLocalSuppliers(context));

            //17
            Console.WriteLine(GetCarsWithTheirListOfParts(context));
            File.WriteAllText(@"../../../Results/cars-and-parts.json", GetCarsWithTheirListOfParts(context));
        }
        private static T Deserialize<T>(string inputXml, string rootName)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            T dtos = (T)serializer.Deserialize(reader)!;
            return dtos;
        }
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            var suppliersDto = Deserialize<ImportSuppliersDto[]>(inputXml, "Suppliers");

            List<Supplier> suppliers = suppliersDto
                .Select(s => new Supplier()
                {
                    Name = s.Name,
                    IsImporter = s.IsImporter
                })
                .ToList();

            context.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            int[] suppliersIds = context.Suppliers.Select(s => s.Id).ToArray();

            var parts = JsonConvert.DeserializeObject<List<Part>>(inputJson)!
                .Where(p => suppliersIds.Contains(p.SupplierId))
                .ToList();

            context.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var cars = JsonConvert.DeserializeObject<List<Car>>(inputJson)!;

            context.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<List<Customer>>(inputJson)!;

            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<List<Sale>>(inputJson)!;

            context.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate).ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    name = c.Name,
                    BirthDate = c.BirthDate.ToString(@"dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                })
                .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new
                {
                    id = s.Id,
                    name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance
                    },
                    parts = c.PartsCars
                    .Select(p => new
                    {
                        p.Part.Name,
                        price = p.Part.Price
                    })
                }).ToArray();
                
            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }
    }
}