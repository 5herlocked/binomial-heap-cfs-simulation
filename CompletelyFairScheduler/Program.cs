using System;
using System.Collections.Generic;
using System.Timers;
using System.Threading;
using Timer = System.Timers.Timer;
using System.Text;

namespace CompletelyFairScheduler
{
    enum SchedulingBias 
    {
        Energy,
        Performance,
        UserFocus,
        Default
    }

    class Program
    {
        public static SchedulingBias currentBias;
        static Mutex mutex = new Mutex();
        static bool running = false;
        static Stack<Task> processReadyQueue = new Stack<Task>();
        static List<Task> completedList = new List<Task>();

        static void CPUThread(Task currentTask)
        {
            if (mutex.WaitOne(1000))
            {
                Console.WriteLine("Task: " + "\t" + currentTask.ProcessID + "\t\t" + currentTask.Priority + "\t\t" + currentTask.Vruntime.ToString(@"ss\.ffff") + "\t\t" + currentTask.PowerConsumption);
                Thread.Sleep(currentTask.Vruntime);

                completedList.Add(currentTask);
                mutex.ReleaseMutex();
                running = false;
            }
        }

        private static void GenerateTasks(object sender, ElapsedEventArgs e)
        {
            Random random = new Random();
            Task generatedTask = new Task(random.Next(100000), random.Next(2000),
                TimeSpan.FromMilliseconds(random.Next(30000)), random.Next(20));

            processReadyQueue.Push(generatedTask);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Completely Fair Scheduler Simulator");
            Console.WriteLine("Do you want to introduce a scheduling bias? (Y/n):");
            String wantBias = Console.ReadLine().Trim().ToLower();
            int schedulingBias;
            if (wantBias.Equals("y"))
            {
                Console.WriteLine("Pick your bias: ");
                Console.WriteLine("1. Energy");
                Console.WriteLine("2. Performance");
                Console.WriteLine("3. UserFocus");

                schedulingBias = int.Parse(Console.ReadLine());
                switch(schedulingBias)
                {
                    case 1:
                        currentBias = SchedulingBias.Energy;
                        break;
                    case 2:
                        currentBias = SchedulingBias.Performance;
                        break;
                    case 3:
                        currentBias = SchedulingBias.UserFocus;
                        break;
                    default:
                        currentBias = SchedulingBias.Default;
                        break;
                }
            }


            Timer taskTimer = new Timer
            {
                Interval = new Random().Next(1000),
                Enabled = true,
                AutoReset = true,
            };
            taskTimer.Elapsed += GenerateTasks;

            Console.Clear();
            Console.WriteLine("\tProcessID\tPriority\tvRuntime\tPower Consumption\n");

            BinomialHeap<Task> schedulingHeap = new BinomialHeap<Task>();

            Console.CancelKeyPress += new ConsoleCancelEventHandler(myHandler);

            while (true)
            {
                Task readyTask = null;
                processReadyQueue.TryPop(out readyTask);

                if (readyTask != null) 
                {
                    schedulingHeap.Insert(readyTask);

                    if (!running)
                    {
                        running = true;
                        Task currentTask = schedulingHeap.ExtractMin();
                        Thread cpuLoop = new Thread(() => CPUThread(currentTask));
                        cpuLoop.Start();
                    }
                }
            }
        }

        private static void myHandler(object sender, ConsoleCancelEventArgs e)
        {
            Console.WriteLine("\n The Simulation has been interrupted");
            Console.WriteLine("Quitting...");
            Console.WriteLine("Printing Completed Tasks: ");
            StringBuilder completedTasksString = new StringBuilder();
            completedTasksString.Append("\tProcessID\tPriority\tvRuntime\tPower Consumption\n");
            completedList.ForEach((e) => completedTasksString.Append(e.ToString() + "\n"));
            Console.WriteLine(completedTasksString);
        }
    }
}