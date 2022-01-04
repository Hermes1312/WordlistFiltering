using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace WordlistFiltering
{
    class Program
    {
        private static int LinesRead = 1, LinesCount = 0;
        private static string LastMatch = default;
        private static bool catchNumeric = false;

        static void Main(string[] args)
        {
            string wordlistPath, filteredWordlistPath, cachNumericChoice;
            int minLength, maxLength;

            Console.Write("Path to wordlist: ");            wordlistPath            = Console.ReadLine();
            Console.Write("Path to filtered wordlist: ");   filteredWordlistPath    = Console.ReadLine();
            Console.Write("Min. password length: ");        minLength               = Convert.ToInt32(Console.ReadLine());
            Console.Write("Max. password length: ");        maxLength               = Convert.ToInt32(Console.ReadLine());
            Console.Write("Catch numeric (y/n): ");         cachNumericChoice       = Console.ReadLine();

            if (cachNumericChoice.ToLower() == "y")
                catchNumeric = true;

            else if (cachNumericChoice.ToLower() == "n")
                catchNumeric = false;

            Console.WriteLine("Counting lines, be patient...");
            LinesCount = File.ReadLines(wordlistPath).Count();

            Thread proccessingThread = new Thread(th => ProccessFile(wordlistPath, filteredWordlistPath, minLength, maxLength));
            proccessingThread.Start();

            while (true)
            {
                var percent = (double)Math.Round((double)(100 * LinesRead) / LinesCount, 2);

                if (percent < 100)
                {
                    Console.Clear();
                    Console.WriteLine($"Progress: {percent}% ({LinesRead}/{LinesCount})");
                    Console.WriteLine($"Last match: {LastMatch}");
                    Thread.Sleep(250);
                }

                else
                    break;
            }

            Console.Clear();
            Console.WriteLine("\nTask completed!");
            Console.ReadKey();
        }

        private static bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }

            return true;
        }

        private static void ProccessFile(string wordlistPath, string filteredWordlistPath, int minLength, int maxLength)
        {
            using (StreamReader reader = new StreamReader(wordlistPath))
            {
                string password;

                while ((password = reader.ReadLine()) != null)
                {
                    if (password.Length >= minLength && password.Length <= maxLength)
                    {
                        if (catchNumeric) 
                        {
                            using (TextWriter writer = new StreamWriter(filteredWordlistPath, true))
                            {
                                LastMatch = password;
                                writer.WriteLineAsync(password);
                            }
                        }
                        else if(!catchNumeric && !IsDigitsOnly(password))
                        {
                            using (TextWriter writer = new StreamWriter(filteredWordlistPath, true))
                            {
                                LastMatch = password;
                                writer.WriteLineAsync(password);
                            }
                        }
                    }

                    LinesRead++;
                }
            }
        }
    }
}
