// Головнич Вікторія КН21
// Тема лабораторної роботи №5 - "Магазин побутової техніки"

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace HomeAppliancesStore
{
    struct Product
    {
        public int Id;
        public string Name;
        public string Category;
        public double Price;
        public int Quantity;
    }

    class Program
    {
        const string ProductsFile = "products.csv";
        const string UsersFile = "users.csv";

        static void Main(string[] args)
        {
            Console.Title = "Магазин побутової техніки";
            InitializeFiles();

            bool accessGranted = false;

            while (!accessGranted)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("--- Система входу ---");
                Console.ResetColor();
                Console.WriteLine("1. Авторизація");
                Console.WriteLine("2. Реєстрація");
                Console.WriteLine("0. Вихід");
                Console.Write("Ваш вибір: ");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        if (Login()) accessGranted = true;
                        break;
                    case "2":
                        Register();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір.");
                        Console.ReadKey();
                        break;
                }
            }

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("\n--- ГОЛОВНЕ МЕНЮ ---");
                Console.ResetColor();
                Console.WriteLine("1. Додати елемент");
                Console.WriteLine("2. Вивести всі елементи (Таблиця)");
                Console.WriteLine("3. Пошук елемента");
                Console.WriteLine("4. Видалення елемента");
                Console.WriteLine("5. Редагування елемента (Lab 5)");
                Console.WriteLine("6. Сортування (List.Sort vs Bubble Sort)");
                Console.WriteLine("7. Статистика");
                Console.WriteLine("8. Калькулятор покупки (Lab 1-2)");
                Console.WriteLine("0. Вихід");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddProduct();
                        Console.ReadKey();
                        break;
                    case "2":
                        ShowAllProducts();
                        Console.ReadKey();
                        break;
                    case "3":
                        SearchProduct();
                        Console.ReadKey();
                        break;
                    case "4":
                        DeleteProduct();
                        Console.ReadKey();
                        break;
                    case "5":
                        EditProduct();
                        Console.ReadKey();
                        break;
                    case "6":
                        SortProducts();
                        Console.ReadKey();
                        break;
                    case "7":
                        ShowStatistics();
                        Console.ReadKey();
                        break;
                    case "8":
                        CalculatePurchase();
                        Console.ReadKey();
                        break;
                    case "0":
                        exit = true;
                        Console.WriteLine("Вихід з програми...");
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Невірний вибір! Спробуйте ще раз.");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void InitializeFiles()
        {
            if (!File.Exists(ProductsFile))
            {
                File.WriteAllText(ProductsFile, "Id,Name,Category,Price,Quantity\n");
            }
            if (!File.Exists(UsersFile))
            {
                File.WriteAllText(UsersFile, "Id,Email,Password\n");
            }
        }

        static int GetNextId(string filePath)
        {
            if (!File.Exists(filePath)) return 1;
            var lines = File.ReadAllLines(filePath);
            if (lines.Length <= 1) return 1;

            int maxId = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var parts = lines[i].Split(',');
                    if (int.TryParse(parts[0], out int id))
                    {
                        if (id > maxId) maxId = id;
                    }
                }
                catch { continue; }
            }
            return maxId + 1;
        }

        static void Register()
        {
            Console.WriteLine("\n--- Реєстрація ---");
            Console.Write("Введіть Email: ");
            string email = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(email))
            {
                Console.WriteLine("Email не може бути порожнім.");
                Console.ReadKey();
                return;
            }

            var lines = File.ReadAllLines(UsersFile);
            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length >= 2 && parts[1] == email)
                {
                    Console.WriteLine("Користувач з таким Email вже існує.");
                    Console.ReadKey();
                    return;
                }
            }

            Console.Write("Введіть пароль: ");
            string password = Console.ReadLine();
            string passwordHash = password.GetHashCode().ToString();

            int newId = GetNextId(UsersFile);
            File.AppendAllText(UsersFile, $"{newId},{email},{passwordHash}\n");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Реєстрація успішна!");
            Console.ResetColor();
            Console.ReadKey();
        }

        static bool Login()
        {
            Console.WriteLine("\n--- Авторизація ---");
            Console.Write("Email: ");
            string email = Console.ReadLine();
            Console.Write("Пароль: ");
            string password = Console.ReadLine();
            string passwordHash = password.GetHashCode().ToString();

            var lines = File.ReadAllLines(UsersFile);
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    var parts = lines[i].Split(',');
                    if (parts.Length < 3) continue;

                    if (parts[1] == email && parts[2] == passwordHash)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Вхід успішний!");
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(800);
                        return true;
                    }
                }
                catch { continue; }
            }

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Невірний Email або пароль.");
            Console.ResetColor();
            Console.ReadKey();
            return false;
        }

        static List<Product> ReadProductsFromFile()
        {
            List<Product> list = new List<Product>();
            if (!File.Exists(ProductsFile)) return list;

            string[] lines = File.ReadAllLines(ProductsFile);
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');
                    if (parts.Length != 5) continue;

                    Product p = new Product
                    {
                        Id = int.Parse(parts[0]),
                        Name = parts[1],
                        Category = parts[2],
                        Price = double.Parse(parts[3]),
                        Quantity = int.Parse(parts[4])
                    };
                    list.Add(p);
                }
                catch { continue; }
            }
            return list;
        }

        static void RewriteProductsFile(List<Product> products)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Id,Name,Category,Price,Quantity");
            foreach (var p in products)
            {
                sb.AppendLine($"{p.Id},{p.Name},{p.Category},{p.Price},{p.Quantity}");
            }
            File.WriteAllText(ProductsFile, sb.ToString());
        }

        static void AddProduct()
        {
            Console.WriteLine("\n--- Додавання товару ---");
            try
            {
                int id = GetNextId(ProductsFile);
                Console.Write("Назва: ");
                string name = Console.ReadLine().Replace(",", "");
                Console.Write("Категорія: ");
                string category = Console.ReadLine().Replace(",", "");
                Console.Write("Ціна (грн): ");
                double price = Convert.ToDouble(Console.ReadLine());
                if (price < 0) price = 0;
                Console.Write("Кількість: ");
                int quantity = Convert.ToInt32(Console.ReadLine());
                if (quantity < 0) quantity = 0;

                File.AppendAllText(ProductsFile, $"{id},{name},{category},{price},{quantity}\n");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Товар успішно додано!");
                Console.ResetColor();
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Помилка формату даних! Товар не додано.");
                Console.ResetColor();
            }
        }

        static void ShowAllProducts()
        {
            Console.WriteLine("\n--- Список товарів ---");
            var products = ReadProductsFromFile();

            if (products.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10} {4,-10}", "ID", "Назва", "Категорія", "Ціна", "Кількість");
            Console.WriteLine(new string('-', 65));
            foreach (var p in products)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10} {4,-10}", p.Id, p.Name, p.Category, p.Price, p.Quantity);
            }
        }

        static void SearchProduct()
        {
            Console.WriteLine("\n--- Пошук товару ---");
            Console.Write("Введіть назву або категорію: ");
            string query = Console.ReadLine().ToLower();

            var products = ReadProductsFromFile();
            bool found = false;

            Console.WriteLine("\nРезультати пошуку:");
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10} {4,-10}", "ID", "Назва", "Категорія", "Ціна", "Кількість");

            foreach (var p in products)
            {
                if (p.Name.ToLower().Contains(query) || p.Category.ToLower().Contains(query))
                {
                    Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10} {4,-10}", p.Id, p.Name, p.Category, p.Price, p.Quantity);
                    found = true;
                }
            }

            if (!found) Console.WriteLine("Нічого не знайдено.");
        }

        static void DeleteProduct()
        {
            Console.WriteLine("\n--- Видалення товару ---");
            ShowAllProducts();
            Console.Write("\nВведіть ID товару для видалення: ");

            try
            {
                int idToDelete = Convert.ToInt32(Console.ReadLine());
                var products = ReadProductsFromFile();
                var productToRemove = products.FirstOrDefault(p => p.Id == idToDelete);

                if (productToRemove.Id != 0)
                {
                    products.RemoveAll(p => p.Id == idToDelete);
                    RewriteProductsFile(products);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Товар видалено.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Товар з таким ID не знайдено.");
                    Console.ResetColor();
                }
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Невірний формат ID.");
                Console.ResetColor();
            }
        }

        static void EditProduct()
        {
            Console.WriteLine("\n--- Редагування товару ---");
            ShowAllProducts();
            Console.Write("\nВведіть ID для редагування: ");

            try
            {
                int idToEdit = Convert.ToInt32(Console.ReadLine());
                var products = ReadProductsFromFile();
                int index = products.FindIndex(p => p.Id == idToEdit);

                if (index != -1)
                {
                    Product p = products[index];
                    Console.WriteLine($"Редагуємо: {p.Name}");

                    Console.Write("Нова назва (Enter - пропустити): ");
                    string name = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(name)) p.Name = name.Replace(",", "");

                    Console.Write("Нова категорія (Enter - пропустити): ");
                    string cat = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(cat)) p.Category = cat.Replace(",", "");

                    Console.Write("Нова ціна (Enter - пропустити): ");
                    string priceStr = Console.ReadLine();
                    if (double.TryParse(priceStr, out double price)) p.Price = price;

                    Console.Write("Нова кількість (Enter - пропустити): ");
                    string qtyStr = Console.ReadLine();
                    if (int.TryParse(qtyStr, out int qty)) p.Quantity = qty;

                    products[index] = p;
                    RewriteProductsFile(products);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Зміни збережено.");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("ID не знайдено.");
                }
            }
            catch
            {
                Console.WriteLine("Помилка вводу.");
            }
        }

        static void SortProducts()
        {
            Console.WriteLine("\n--- Сортування (за ціною) ---");
            Console.WriteLine("1. Вбудоване сортування (List.Sort)");
            Console.WriteLine("2. Власне сортування (Bubble Sort)");
            Console.Write("Оберіть метод: ");

            string sortChoice = Console.ReadLine();
            var products = ReadProductsFromFile();

            if (products.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            if (sortChoice == "1")
            {
                products.Sort((p1, p2) => p1.Price.CompareTo(p2.Price));
                Console.WriteLine("Відсортовано вбудованим методом.");
            }
            else if (sortChoice == "2")
            {
                for (int i = 0; i < products.Count - 1; i++)
                {
                    for (int j = 0; j < products.Count - i - 1; j++)
                    {
                        if (products[j].Price > products[j + 1].Price)
                        {
                            Product temp = products[j];
                            products[j] = products[j + 1];
                            products[j + 1] = temp;
                        }
                    }
                }
                Console.WriteLine("Відсортовано методом бульбашки.");
            }
            else
            {
                Console.WriteLine("Невірний вибір.");
                return;
            }

            Console.WriteLine("\nРезультат сортування (відображення):");
            Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10} {4,-10}", "ID", "Назва", "Категорія", "Ціна", "Кількість");
            Console.WriteLine(new string('-', 65));
            foreach (var p in products)
            {
                Console.WriteLine("{0,-5} {1,-20} {2,-15} {3,-10} {4,-10}", p.Id, p.Name, p.Category, p.Price, p.Quantity);
            }
        }

        static void ShowStatistics()
        {
            var products = ReadProductsFromFile();
            if (products.Count == 0)
            {
                Console.WriteLine("Список порожній!");
                return;
            }

            double totalValue = 0;
            double maxPrice = double.MinValue;
            double minPrice = double.MaxValue;
            string mostExpensive = "";

            foreach (var p in products)
            {
                totalValue += p.Price * p.Quantity;
                if (p.Price > maxPrice)
                {
                    maxPrice = p.Price;
                    mostExpensive = p.Name;
                }
                if (p.Price < minPrice)
                {
                    minPrice = p.Price;
                }
            }

            double averagePrice = products.Count > 0 ? totalValue / products.Count : 0; // Середня вартість по складу (спрощено)

            Console.WriteLine("\n--- Статистика ---");
            Console.WriteLine($"Кількість позицій: {products.Count}");
            Console.WriteLine($"Загальна вартість складу: {totalValue} грн");
            Console.WriteLine($"Найдорожчий товар: {mostExpensive} ({maxPrice} грн)");
            Console.WriteLine($"Мінімальна ціна: {minPrice} грн");
        }

        static void CalculatePurchase()
        {
            Console.WriteLine("\n--- Розрахунок покупки (Lab 1-2) ---");
            double priceTv = 15000.00;
            double priceFridge = 22000.00;
            double priceWasher = 18500.00;

            try
            {
                Console.Write("Кількість ТВ: ");
                int cTv = int.Parse(Console.ReadLine());
                Console.Write("Кількість Холодильників: ");
                int cFr = int.Parse(Console.ReadLine());
                Console.Write("Кількість Пральних машин: ");
                int cWs = int.Parse(Console.ReadLine());

                double total = (priceTv * cTv) + (priceFridge * cFr) + (priceWasher * cWs);
                Console.WriteLine($"Сума: {total} грн");
            }
            catch
            {
                Console.WriteLine("Помилка вводу.");
            }
        }
    }
}