using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("{0,-30} {1,-15} {2,-15} {3,-10} {4,-15}",
            "Имя процесса", "PID", "Память (МБ)", "ЦП (%)", "Приоритет");
        Console.WriteLine(new string('-', 85));

        Process[] processes = Process.GetProcesses();
        var processList = processes.Select(p => {
            try
            {
                string user = GetProcessOwnerSimple(p);
                float memoryMB = p.WorkingSet64 / 1024f / 1024f;
                float cpu = GetCpuUsage(p);
                string priority = p.BasePriority.ToString();
                return new { Process = p, User = user, MemoryMB = memoryMB, CPU = cpu, Priority = priority };
            }
            catch
            {
                return null;
            }
        }).Where(p => p != null).ToList();

        // Вывод всех процессов
        foreach (var proc in processList)
        {
            Console.WriteLine("{0,-30} {1,-15} {2,-15:F2} {3,-10:F2} {4,-15}",
                proc.Process.ProcessName, proc.Process.Id, proc.MemoryMB, proc.CPU, proc.Priority);
        }

        // Поиск самого ресурсоёмкого и наименее ресурсоёмкого процессов
        if (processList.Any())
        {
            var mostResourceIntensive = processList.OrderByDescending(p => p.MemoryMB).First();
            var leastResourceIntensive = processList.Where(p => p.MemoryMB > 0).OrderBy(p => p.MemoryMB).First();

            Console.WriteLine("\n--- Анализ ресурсов ---");
            Console.WriteLine($"Самый ресурсоёмкий процесс: {mostResourceIntensive.Process.ProcessName} (PID: {mostResourceIntensive.Process.Id})");
            Console.WriteLine($"  Память: {mostResourceIntensive.MemoryMB:F2} МБ, ЦП: {mostResourceIntensive.CPU:F2}%, Приоритет: {mostResourceIntensive.Priority}");

            Console.WriteLine($"Процесс с наименьшим количеством ресурсов: {leastResourceIntensive.Process.ProcessName} (PID: {leastResourceIntensive.Process.Id})");
            Console.WriteLine($"  Память: {leastResourceIntensive.MemoryMB:F2} МБ, ЦП: {leastResourceIntensive.CPU:F2}%, Приоритет: {leastResourceIntensive.Priority}");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }

    static string GetProcessOwnerSimple(Process process)
    {
        try
        {
            string processName = Process.GetCurrentProcess().ProcessName;
            if (processName == "svchost") return "SYSTEM";

            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                return identity.Name.Split('\\').LastOrDefault() ?? "Unknown";
            }
        }
        catch
        {
            return "N/A";
        }
    }

    static float GetCpuUsage(Process process)
    {
        try
        {
            using (PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName))
            {
                cpuCounter.NextValue();
                System.Threading.Thread.Sleep(500);
                return cpuCounter.NextValue() / Environment.ProcessorCount;
            }
        }
        catch
        {
            return 0;
        }
    }
}