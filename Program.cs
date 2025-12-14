// Головнич Вікторія КН-21

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace FarmerMarketApp
{
    class Program
    {
        private const string PRODUCTS_FILE = "products.csv";
        private const string USERS_FILE = "users.csv";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            CheckFiles();
            if (AuthMenu())
            {
                ShowMainMenu();
            }
        }

        static void CheckFiles()
        {
            if (!File.Exists(USERS_FILE))
            {
                File.WriteAllText(USERS_FILE, "Id,Email,Password\n");
                File.AppendAllText(USERS_FILE, "1,admin@gmail.com,12345\n");
            }
            if (!File.Exists(PRODUCTS_FILE))
            {
                File.WriteAllText(PRODUCTS_FILE, "Id,Name,Price,Quantity\n");
            }
        }

        static int GenerateNewId(string filePath)
        {
            if (!File.Exists(filePath)) return 1;
            string[] lines = File.ReadAllLines(filePath);
            if (lines.Length <= 1) return 1;
            int maxId = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    string line = lines[i];
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    string[] parts = line.Split(',');
                    if (int.TryParse(parts[0], out int currentId))
                    {
                        if (currentId > maxId) maxId = currentId;
                    }
                }
                catch
                {
                    continue;
                }
            }

            return maxId + 1;
        }
        
        static bool AuthMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("--- ВХІД У СИСТЕМУ ---");
                Console.WriteLine("1. Вхід (Логін)");
                Console.WriteLine("2. Реєстрація");
                Console.WriteLine("0. Вихід");
                Console.Write("Вибір: ");
                
                string choice = Console.ReadLine();

                if (choice == "1")
                {
                    if (Login()) return true;
                }
                else if (choice == "2")
                {
                    Register();
                }
                else if (choice == "0")
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Невірний вибір.");
                    Pause();
                }
            }
        }

        static bool Login()
        {
            Console.Write("Введіть Email: ");
            string email = Console.ReadLine();
            Console.Write("Введіть Пароль: ");
            string password = Console.ReadLine();
            string[] lines = File.ReadAllLines(USERS_FILE);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length >= 3)
                {
                    if (parts[1] == email && parts[2] == password)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Вхід успішний!");
                        Console.ResetColor();
                        Pause();
                        return true;
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Невірний логін або пароль.");
            Console.ResetColor();
            Pause();
            return false;
        }

        static void Register()
        {
            Console.WriteLine("\n--- РЕЄСТРАЦІЯ ---");
            Console.Write("Введіть новий Email: ");
            string email = Console.ReadLine();
            string[] lines = File.ReadAllLines(USERS_FILE);
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length >= 2 && parts[1] == email)
                {
                    Console.WriteLine("Користувач з таким Email вже існує!");
                    Pause();
                    return;
                }
            }

            Console.Write("Введіть пароль: ");
            string password = Console.ReadLine();
            int newId = GenerateNewId(USERS_FILE);
            string newUserLine = $"{newId},{email},{password}\n";
            File.AppendAllText(USERS_FILE, newUserLine);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Реєстрація успішна! Тепер увійдіть.");
            Console.ResetColor();
            Pause();
        }

        static void ShowMainMenu()
        {
            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("================================================");
                Console.WriteLine(" ГОЛОВНЕ МЕНЮ (Робота з CSV) ");
                Console.WriteLine("================================================");
                Console.WriteLine("1. Додати товар (запис у файл)");
                Console.WriteLine("2. Переглянути всі товари (читання з файлу)");
                Console.WriteLine("3. Видалити товар (перезапис файлу)");
                Console.WriteLine("4. Пошук товару (у файлі)");
                Console.WriteLine("5. Статистика (з файлу)");
                Console.WriteLine("6. Сортування (читання -> сортування -> перезапис)");
                Console.WriteLine("0. Вихід");
                Console.Write("Ваш вибір: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": AddProductToFile(); break;
                    case "2": ReadAndShowProducts(); break;
                    case "3": DeleteProductFromFile(); break;
                    case "4": SearchInFile(); break;
                    case "5": FileStatistics(); break;
                    case "6": SortFile(); break;
                    case "0": exit = true; break;
                    default: Console.WriteLine("Невірний вибір."); Pause(); break;
                }
            }
        }

        static void AddProductToFile()
        {
            Console.Clear();
            Console.WriteLine("--- Додавання товару ---");
            int id = GenerateNewId(PRODUCTS_FILE);
            Console.WriteLine($"Присвоєно ID: {id}");
            Console.Write("Назва: ");
            string name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name)) name = "NoName";
            double price;
            while (true)
            {
                Console.Write("Ціна: ");
                if (double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out price) && price >= 0) break;
                Console.WriteLine("Помилка ціни.");
            }
            int quantity;
            while (true)
            {
                Console.Write("Кількість: ");
                if (int.TryParse(Console.ReadLine(), out quantity) && quantity >= 0) break;
                Console.WriteLine("Помилка кількості.");
            }
            string line = $"{id},{name},{price.ToString(CultureInfo.InvariantCulture)},{quantity}\n";
            File.AppendAllText(PRODUCTS_FILE, line);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Товар записано у файл!");
            Console.ResetColor();
            Pause();
        }

        static void ReadAndShowProducts()
        {
            Console.Clear();
            Console.WriteLine("--- ТОВАРИ З ФАЙЛУ ---");
            Console.WriteLine("------------------------------------------------------");
            Console.WriteLine("| ID   | Назва           | Ціна       | Кількість |");
            Console.WriteLine("------------------------------------------------------");

            if (File.Exists(PRODUCTS_FILE))
            {
                string[] lines = File.ReadAllLines(PRODUCTS_FILE);
                for (int i = 1; i < lines.Length; i++)
                {
                    try
                    {
                        string[] parts = lines[i].Split(',');
                        if (parts.Length < 4) continue;

                        string id = parts[0];
                        string name = parts[1];
                        string price = parts[2];
                        string qty = parts[3];

                        Console.WriteLine($"| {id,-4} | {name,-15} | {price,10} | {qty,9} |");
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            else
            {
                Console.WriteLine("Файл відсутній.");
            }
            Console.WriteLine("------------------------------------------------------");
            Pause();
        }

        static void DeleteProductFromFile()
        {
            Console.Clear();
            Console.WriteLine("--- Видалення товару ---");
            Console.Write("Введіть ID для видалення: ");
            string idToDelete = Console.ReadLine();
            string[] lines = File.ReadAllLines(PRODUCTS_FILE);
            List<string> newLines = new List<string>();
            newLines.Add(lines[0]);

            bool found = false;

            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts[0] != idToDelete)
                {
                    newLines.Add(lines[i]);
                }
                else
                {
                    found = true;
                }
            }

            if (found)
            {
                File.WriteAllLines(PRODUCTS_FILE, newLines);
                Console.WriteLine("Товар видалено.");
            }
            else
            {
                Console.WriteLine("ID не знайдено.");
            }
            Pause();
        }

        static void SearchInFile()
        {
            Console.Clear();
            Console.Write("Введіть назву для пошуку: ");
            string searchName = Console.ReadLine().ToLower();

            string[] lines = File.ReadAllLines(PRODUCTS_FILE);
            
            Console.WriteLine("Результати:");
            for (int i = 1; i < lines.Length; i++)
            {
                string[] parts = lines[i].Split(',');
                if (parts.Length >= 2)
                {
                    if (parts[1].ToLower().Contains(searchName))
                    {
                        Console.WriteLine(lines[i]);
                    }
                }
            }
            Pause();
        }

        static void FileStatistics()
        {
            Console.Clear();
            string[] lines = File.ReadAllLines(PRODUCTS_FILE);
            
            double totalSum = 0;
            int count = 0;
            double maxPrice = 0;
            double minPrice = double.MaxValue;

            for (int i = 1; i < lines.Length; i++)
            {
                try
                {
                    string[] parts = lines[i].Split(',');
                    double price = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    int qty = int.Parse(parts[3]);

                    totalSum += price * qty;
                    count++;

                    if (price > maxPrice) maxPrice = price;
                    if (price < minPrice) minPrice = price;
                }
                catch { continue; }
            }

            if (minPrice == double.MaxValue) minPrice = 0;

            Console.WriteLine($"Всього товарів: {count}");
            Console.WriteLine($"Загальна вартість складу: {totalSum}");
            Console.WriteLine($"Мін. ціна: {minPrice}");
            Console.WriteLine($"Макс. ціна: {maxPrice}");
            Pause();
        }

        static void SortFile()
        {
            Console.WriteLine("Сортування за ціною...");
            string[] lines = File.ReadAllLines(PRODUCTS_FILE);
            List<string> dataLines = new List<string>();
            for(int i = 1; i < lines.Length; i++)
            {
                if(!string.IsNullOrWhiteSpace(lines[i]))
                    dataLines.Add(lines[i]);
            }
            for (int i = 0; i < dataLines.Count - 1; i++)
            {
                for (int j = 0; j < dataLines.Count - i - 1; j++)
                {
                    double price1 = double.Parse(dataLines[j].Split(',')[2], CultureInfo.InvariantCulture);
                    double price2 = double.Parse(dataLines[j+1].Split(',')[2], CultureInfo.InvariantCulture);

                    if (price1 > price2)
                    {
                        string temp = dataLines[j];
                        dataLines[j] = dataLines[j+1];
                        dataLines[j+1] = temp;
                    }
                }
            }
            List<string> finalLines = new List<string>();
            finalLines.Add(lines[0]);
            finalLines.AddRange(dataLines);

            File.WriteAllLines(PRODUCTS_FILE, finalLines);
            Console.WriteLine("Файл відсортовано і перезаписано!");
            Pause();
        }

        static void Pause()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}