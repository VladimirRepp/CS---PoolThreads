using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sample_9
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Старт главного потока!");
            Sample_1();
            //Sample_2();
            //Sample_3();
            //Sample_4();
            //Sample_5();

            Console.WriteLine("Окончание главного потока!\nНажмите ВВОД для выхода...");
            Console.ReadLine();
        }

        #region === Examples of QueueUserWorkItem Without Waiting === 

        /// <summary>
        /// Пример 1: добавление задач в пул потоков. БЕЗ ожидания!
        /// </summary>
        private static void Sample_1()
        {
            // Поставили задачи в очередь рабочих задач
            ThreadPool.QueueUserWorkItem(WorkItem1_1, "Hello ThreadPool!");
            ThreadPool.QueueUserWorkItem(WorkItem2_1, 45);

            //Thread.Sleep(1000);

            // Гавный поток, НЕ ждет окончания работы задач из пула потов
            Console.WriteLine("Окончание главного потока!");
        }

        /// <summary>
        /// Метод рабочей задачи для пула потоков, которая выводит строку в консоль
        /// </summary>
        /// <param name="state"></param>
        private static void WorkItem1_1(object state)
        {
            Console.WriteLine("WorkItem1_1: {0}", state);
        }

        /// <summary>
        /// Метод рабочей задачи для пула потоков, который считает квадрат числа и выводит в консоль
        /// </summary>
        /// <param name="state"></param>
        private static void WorkItem2_1(object state)
        {
            int value = (int)state;
            int squere = value * value;
            Console.WriteLine("WorkItem2_1: {0} * {1} = {2}", value, value, squere);
        }

        #endregion

        #region === Examples of QueueUserWorkItem With Waiting ===

        /// <summary>
        /// Пример 2: добавление задач в пул потоков. С ожиданием! 
        /// </summary>
        private static void Sample_2()
        {
            CountdownEvent countdownEvent = new CountdownEvent(2);

            // Поставили задачи в очередь рабочих задач
            ThreadPool.QueueUserWorkItem(WorkItem1_2, countdownEvent);
            ThreadPool.QueueUserWorkItem(WorkItem2_2, countdownEvent);

            countdownEvent.Wait();
        }

        private static void WorkItem1_2(object countdownEvent)
        {
            Console.WriteLine("WorkItem1_1!");

            CountdownEvent myCountdownEvent = (CountdownEvent)countdownEvent;
            myCountdownEvent.Signal();
        }

        private static void WorkItem2_2(object countdownEvent)
        {
            Console.WriteLine("WorkItem2_1!");

            CountdownEvent myCountdownEvent = (CountdownEvent)countdownEvent;
            myCountdownEvent.Signal();
        }

        #endregion

        #region === Examples of use Task.Run ===

        /// <summary>
        /// Пример 3: постановка задач в очередь пула потоков через Task.Run
        /// </summary>
        private static void Sample_3()
        {
            // Поставка в очередь рабочих задач в пул потоков
            Task<int> task_sum = Task.Run(() => Sum(10, 20));
            Task<int> task_multiply = Task.Run(() => Multiply(15, 10));

            // Дожидаемся результат задачи (т.е. блокируем основной поток)
            int result_task_sum = task_sum.Result;
            int result_task_multiply = task_multiply.Result;

            Console.WriteLine($"result_task_sum = {result_task_sum}");
            Console.WriteLine($"result_task_multiply = {result_task_multiply}");
        }

        private static int Sum(int a, int b)
        {
            return a + b;
        }

        private static int Multiply(int a, int b)
        {
            return a * b;
        }

        #endregion

        #region === Examples of thread safety and ThreadPool === 
        private static int _counter;

        /// <summary>
        /// Пример 4: потокобезопастность и пул потоков
        /// </summary>
        private static void Sample_4()
        {
            int countWorkItems = 10;
            CountdownEvent countdownEvent = new CountdownEvent(countWorkItems);
            for (int i = 0; i < countWorkItems; i++)
            {
                ThreadPool.QueueUserWorkItem(WorkItem_4, countdownEvent);
            }

            countdownEvent.Wait();

            Console.WriteLine($"_counter = {_counter}");
        }

        private static void WorkItem_4(object state)
        {
            CountdownEvent countdownEvent = (CountdownEvent)state;

            // Эмитация работы
            Random rand = new Random();
            Thread.Sleep(rand.Next(1000, 3000));

            int value = Interlocked.Increment(ref _counter); // делаем потоко-безопасную операцию
            Console.WriteLine($"WorkItem: {value}");

            countdownEvent.Signal();
        }

        #endregion

        #region === Examples of print info about ThreadPool ===

        /// <summary>
        /// Пример 5: вывод информации о пуле потоков
        /// </summary>
        private static void Sample_5()
        {
            // PrintInfoThreadPool_Sample1_5();
            PrintInfoThreadPool_Sample2_5();
        }

        private static void PrintInfoThreadPool_Sample1_5()
        {
            int minWorkerThreads, maxWorkerThreads;
            int minCompletionPortThreads, maxCompletionPortThreads;

            ThreadPool.GetMinThreads(out minWorkerThreads, out minCompletionPortThreads);
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);

            Console.WriteLine(" === Начальные значения ===");
            Console.WriteLine($"Минимальное количество работчих потоков: {minWorkerThreads}");
            Console.WriteLine($"Максимальное количество работчих потоков: {maxWorkerThreads}");
            Console.WriteLine($"Минимальное количество потоков портов: {minCompletionPortThreads}");
            Console.WriteLine($"Максимальное количество потоков портов: {maxCompletionPortThreads}");

            // === //

            ThreadPool.SetMinThreads(10, 10);
            ThreadPool.SetMaxThreads(20, 20);

            ThreadPool.GetMinThreads(out minWorkerThreads, out minCompletionPortThreads);
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);

            Console.WriteLine(" === Значения после изменения ===");
            Console.WriteLine($"Минимальное количество работчих потоков: {minWorkerThreads}");
            Console.WriteLine($"Максимальное количество работчих потоков: {maxWorkerThreads}");
            Console.WriteLine($"Минимальное количество потоков портов: {minCompletionPortThreads}");
            Console.WriteLine($"Максимальное количество потоков портов: {maxCompletionPortThreads}");
        }

        private static void PrintInfoThreadPool_Sample2_5()
        {
            int countWorkItems = 10;
            int availableWorkerTheards, availableCompletionPortTheards;

            CountdownEvent countdownEvent = new CountdownEvent(countWorkItems);

            Console.WriteLine("=== Начальные значения === ");
            ThreadPool.GetAvailableThreads(out availableWorkerTheards, out availableCompletionPortTheards);
            Console.WriteLine($"availableWorkerTheards = {availableWorkerTheards}");
            Console.WriteLine($"availableCompletionPortTheards = {availableCompletionPortTheards}");

            for (int i = 0; i < countWorkItems; i++)
            {
                ThreadPool.QueueUserWorkItem(WorkItem, countdownEvent);
            }

            // =========================================================== //

            Console.WriteLine("\n=== Значения после запуска задач === ");
            ThreadPool.GetAvailableThreads(out availableWorkerTheards, out availableCompletionPortTheards);
            Console.WriteLine($"availableWorkerTheards = {availableWorkerTheards}");
            Console.WriteLine($"availableCompletionPortTheards = {availableCompletionPortTheards}");

            // =========================================================== //

            countdownEvent.Wait();

            Console.WriteLine("\n=== Значения после завершения задач === ");
            ThreadPool.GetAvailableThreads(out availableWorkerTheards, out availableCompletionPortTheards);
            Console.WriteLine($"availableWorkerTheards = {availableWorkerTheards}");
            Console.WriteLine($"availableCompletionPortTheards = {availableCompletionPortTheards}");
        }

        private static void WorkItem_5(object state)
        {
            CountdownEvent countdownEvent = (CountdownEvent)state;

            Random rand = new Random();
            Thread.Sleep(rand.Next(1000, 3000));

            Console.WriteLine("WorkItem - completed!");
            countdownEvent.Signal();
        }
        #endregion
    }
}
