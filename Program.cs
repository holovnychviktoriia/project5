// Головнич Вікторія КН-21

using System;
using System.Collections.Generic;
using System.Globalization;

namespace FarmerMarketApp
{
    struct Product
    {
        public int Id;
        public string Name;
        public double Price;
        public int Quantity;

        public Product(int id, string name, double price, int quantity)
        {
            this.Id = id;
            this.Name = name;
            this.Price = price;
            this.Quantity = quantity;
        }

        public void Display()
        {
            Console.WriteLine($"| {Id,-4} | {Name,-15} | {Price,10:F2} | {Quantity,10} |");
        }
    }

    class Program
    {
        private static List<Product> products = new List<Product>(); 
        private const string CORRECT_LOGIN = "user";
        private const string CORRECT_PASSWORD = "123";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            
            if (LoginSystem())
            {
                ShowMainMenu();
            }
            else
            {
                Console.WriteLine("\nСпроби вичерпано. Програму завершено.");
            }
        }
        
        static bool LoginSystem()
        {
            Console.Clear();
            Console.WriteLine("--- СИСТЕМА ВХОДУ ---");

            for (int attempts = 1; attempts <= 3; attempts++)
            {
                Console.WriteLine($"\nСпроба {attempts} з 3.");
                Console.Write("Введіть логін: ");
                string login = Console.ReadLine();
                Console.Write("Введіть пароль: ");
                string password = Console.ReadLine();

                if (login == CORRECT_LOGIN && password == CORRECT_PASSWORD)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nВхід успішний! Доступ надано.");
                    Console.ResetColor();
                    Pause();
                    return true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n❌ Неправильний логін або пароль!");
                    Console.ResetColor();
                    
                    if (attempts == 3) 
                    {
                        Console.WriteLine("Остання спроба невдала.");
                        break; 
                    }
                }
            }
            return false;
        }

        static void ShowMainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("================================================");
                Console.WriteLine(" ГОЛОВНЕ МЕНЮ 'Фермерського магазину' ");
                Console.WriteLine("================================================");
                Console.WriteLine("1. Додати 5 товарів (Початкове заповнення)");
                Console.WriteLine("2. Переглянути список товарів");
                Console.WriteLine("3. Обчислити статистику");
                Console.WriteLine("4. Операції з колекцією (Додати/Пошук/Видалення/Сортування)");
                Console.WriteLine("0. Вихід з програми");
                Console.Write("Ваш вибір: ");

                string input = Console.ReadLine();
                if (!int.TryParse(input, out int choice))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Помилка! Введіть число.");
                    Console.ResetColor();
                    Pause();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        InputInitialProducts();
                        break;
                    case 2:
                        ShowProducts();
                        break;
                    case 3:
                        CalculateStatistics();
                        break;
                    case 4:
                        ShowCollectionMenu();
                        break;
                    case 0:
                        Console.WriteLine("\nПрограма завершена.");
                        exit = true;
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Невірний вибір!");
                        Console.ResetColor();
                        break;
                }

                if (!exit)
                {
                    Pause();
                }
            }
        }

        static void ShowCollectionMenu()
        {
            bool back = false;
            while (!back)
            {
                Console.Clear();
                Console.WriteLine("--- ОПЕРАЦІЇ З КОЛЕКЦІЄЮ ---");
                Console.WriteLine("1. Додати один новий товар");
                Console.WriteLine("2. Знайти товар за назвою");
                Console.WriteLine("3. Видалити товар за ID");
                Console.WriteLine("4. Сортування (Bubble Sort vs Standard)");
                Console.WriteLine("0. Повернутися назад");
                Console.Write("Ваш вибір: ");

                string input = Console.ReadLine();
                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Невірний ввід.");
                    Pause();
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        AddSingleProduct();
                        break;
                    case 2:
                        SearchProductByName();
                        break;
                    case 3:
                        DeleteProductById();
                        break;
                    case 4:
                        SortMenu();
                        break;
                    case 0:
                        back = true;
                        break;
                    default:
                        Console.WriteLine("Невірний вибір.");
                        break;
                }
                
                if (!back) Pause();
            }
        }

        static void InputInitialProducts()
        {
            Console.Clear();
            Console.WriteLine("--- Введення даних про 5 товарів ---");
            
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"\n>> Введіть дані для товару №{i + 1}:");
                Product p = CreateProductFromInput();
                products.Add(p);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nДані про 5 товарів успішно додано до списку!");
            Console.ResetColor();
        }

        static void AddSingleProduct()
        {
            Console.Clear();
            Console.WriteLine("--- Додавання нового товару ---");
            Product p = CreateProductFromInput();
            products.Add(p);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Товар успішно додано!");
            Console.ResetColor();
        }

        static Product CreateProductFromInput()
        {
            int id = 0;
            double price = 0.0;
            int quantity = 0;

            do 
            {
                Console.Write("ID (ціле число > 0): ");
                string idInput = Console.ReadLine();
                if (!int.TryParse(idInput, out id) || id <= 0)
                {
                    Console.WriteLine("Помилка. ID має бути додатним.");
                }
            } while (id <= 0);

            Console.Write("Назва: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Без назви";
            }

            do 
            {
                Console.Write("Ціна (грн): ");
                string priceInput = Console.ReadLine();
                if (!double.TryParse(priceInput, NumberStyles.Any, CultureInfo.InvariantCulture, out price) || price < 0)
                {
                    Console.WriteLine("Помилка. Ціна має бути числом >= 0.");
                }
            } while (price < 0);

            do 
            {
                Console.Write("Кількість: ");
                string qtyInput = Console.ReadLine();
                if (!int.TryParse(qtyInput, out quantity) || quantity < 0)
                {
                    Console.WriteLine("Помилка. Кількість має бути цілим числом >= 0.");
                }
            } while (quantity < 0);

            return new Product(id, name, price, quantity);
        }

        static void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine("--- СПИСОК ТОВАРІВ ---");

            if (products.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine("| ID   | Назва           | Ціна (грн) | Кількість |");
            Console.WriteLine("-----------------------------------------------------------------");
            
            foreach (var product in products)
            {
                product.Display();
            }
            Console.WriteLine("-----------------------------------------------------------------");
            Console.WriteLine($"Всього елементів: {products.Count}");
        }

        static void SearchProductByName()
        {
            Console.Clear();
            Console.WriteLine("--- Пошук товару ---");
            Console.Write("Введіть назву (або частину) для пошуку: ");
            string searchName = Console.ReadLine();

            bool found = false;
            Console.WriteLine("\nРезультати пошуку:");
            Console.WriteLine("-----------------------------------------------------------------");
            
            for (int i = 0; i < products.Count; i++)
            {
                if (products[i].Name.Contains(searchName))
                {
                    products[i].Display();
                    found = true;
                }
            }

            if (!found)
            {
                Console.WriteLine("Товарів не знайдено.");
            }
            Console.WriteLine("-----------------------------------------------------------------");
        }

        static void DeleteProductById()
        {
            Console.Clear();
            Console.WriteLine("--- Видалення товару ---");
            Console.Write("Введіть ID товару для видалення: ");
            
            if (int.TryParse(Console.ReadLine(), out int searchId))
            {
                int indexToDelete = -1;
                
                for (int i = 0; i < products.Count; i++)
                {
                    if (products[i].Id == searchId)
                    {
                        indexToDelete = i;
                        break;
                    }
                }

                if (indexToDelete != -1)
                {
                    Console.WriteLine($"Видалено товар: {products[indexToDelete].Name}");
                    products.RemoveAt(indexToDelete);
                }
                else
                {
                    Console.WriteLine("Товар з таким ID не знайдено.");
                }
            }
            else
            {
                Console.WriteLine("Некоректний ID.");
            }
        }

        static void SortMenu()
        {
            Console.Clear();
            Console.WriteLine("--- Сортування ---");
            Console.WriteLine("1. Власний алгоритм (Bubble Sort) за ціною");
            Console.WriteLine("2. Вбудований List.Sort за ціною");
            Console.Write("Вибір: ");
            string input = Console.ReadLine();

            if (input == "1")
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
                Console.WriteLine("Відсортовано бульбашкою.");
            }
            else if (input == "2")
            {
                products.Sort((p1, p2) => p1.Price.CompareTo(p2.Price));
                Console.WriteLine("Відсортовано стандартним методом.");
            }
            else
            {
                Console.WriteLine("Невірний вибір.");
            }
        }

        static void CalculateStatistics()
        {
            Console.Clear();
            Console.WriteLine("--- ОБЧИСЛЕННЯ СТАТИСТИКИ ---");

            if (products.Count == 0)
            {
                Console.WriteLine("Список порожній.");
                return;
            }

            double totalSum = 0;
            double sumOfPrices = 0;
            int countValidProducts = 0;
            int countExpensive = 0;
            double minPrice = double.MaxValue;
            double maxPrice = double.MinValue;
            string cheapestProduct = "N/A";
            string expensiveProduct = "N/A";

            for (int i = 0; i < products.Count; i++)
            {
                Product p = products[i];

                if (p.Price <= 0 || p.Quantity <= 0)
                {
                    continue;
                }

                totalSum += p.Price * p.Quantity;
                sumOfPrices += p.Price;
                countValidProducts++;

                if (p.Price > 50.00)
                {
                    countExpensive++;
                }

                if (p.Price < minPrice)
                {
                    minPrice = p.Price;
                    cheapestProduct = p.Name;
                }
                if (p.Price > maxPrice)
                {
                    maxPrice = p.Price;
                    expensiveProduct = p.Name;
                }
            }
            
            double averagePrice = countValidProducts > 0 ? sumOfPrices / countValidProducts : 0;
            
            Console.WriteLine("\n-------------------------------------------------");
            Console.WriteLine("РЕЗУЛЬТАТИ ОБЧИСЛЕНЬ:");
            Console.WriteLine("-------------------------------------------------");
            Console.WriteLine($"Кількість товарів у списку: {products.Count}");
            Console.WriteLine($"Загальна вартість усіх товарів: {totalSum:F2} грн");
            Console.WriteLine($"Середня ціна товару: {averagePrice:F2} грн");
            Console.WriteLine($"Товарів дорожче 50 грн: {countExpensive} шт.");
            Console.WriteLine($"Найдешевший товар: {cheapestProduct} ({minPrice:F2} грн)");
            Console.WriteLine($"Найдорожчий товар: {expensiveProduct} ({maxPrice:F2} грн)");
        }

        static void Pause()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}