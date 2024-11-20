namespace File
{
    internal class Program
    {
        static readonly string numbersFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "numbers.txt");
        static readonly string sumFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sums.txt");
        static readonly string productFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "products.txt");

        static List<Tuple<int, int>> numberPairs = new List<Tuple<int, int>>();
        static AutoResetEvent generationCompleted = new AutoResetEvent(false);

        static void Main()
        {
            Thread generatorThread = new Thread(GenerateNumbers);
            Thread sumThread = new Thread(ComputeSums);
            Thread productThread = new Thread(ComputeProducts);

            generatorThread.Start();
            sumThread.Start();
            productThread.Start();

            generatorThread.Join();
            generationCompleted.Set();

            sumThread.Join();
            productThread.Join();

            Console.WriteLine("Программа завершена.");
        }

        static void GenerateNumbers()
        {
            try
            {
                Random random = new Random();
                using (StreamWriter writer = new StreamWriter(numbersFile))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int num1 = random.Next(1, 100);
                        int num2 = random.Next(1, 100);
                        numberPairs.Add(new Tuple<int, int>(num1, num2));
                        writer.WriteLine($"{num1},{num2}");
                    }
                }

                Console.WriteLine("Генерация чисел завершена.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при генерации чисел: {ex.Message}");
            }
        }

        static void ComputeSums()
        {
            try
            {
                generationCompleted.WaitOne();

                List<int> sums = new List<int>();
                foreach (var pair in numberPairs)
                {
                    sums.Add(pair.Item1 + pair.Item2);
                }

                using (StreamWriter writer = new StreamWriter(sumFile))
                {
                    foreach (var sum in sums)
                    {
                        writer.WriteLine(sum);
                    }
                }

                Console.WriteLine("Подсчет сумм завершен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при подсчете сумм: {ex.Message}");
            }
        }

        static void ComputeProducts()
        {
            try
            {
                generationCompleted.WaitOne();

                List<int> products = new List<int>();
                foreach (var pair in numberPairs)
                {
                    products.Add(pair.Item1 * pair.Item2);
                }

                using (StreamWriter writer = new StreamWriter(productFile))
                {
                    foreach (var product in products)
                    {
                        writer.WriteLine(product);
                    }
                }

                Console.WriteLine("Подсчет произведений завершен.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при подсчете произведений: {ex.Message}");
            }
        }
    }
}