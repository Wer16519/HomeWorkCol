using System;
using MPI;

class Program
{
    static void Main(string[] args)
    {
        using (new MPI.Environment(ref args))
        {
            Intracommunicator comm = Communicator.world;
            int rank = comm.Rank;
            int size = comm.Size;
            int arraySize = 10;
            double[] array = new double[arraySize];

            if (rank == 0)
            {
                Random rand = new Random();
                for (int i = 0; i < arraySize; i++)
                {
                    array[i] = Math.Cos((i + 1) * rand.NextDouble());
                }
            }

            // Рассылка массива всем процессам
            comm.Broadcast(ref array, 0);

            // Умножение элементов на номер процесса
            for (int i = 0; i < arraySize; i++)
            {
                array[i] *= rank;
            }

            // Поиск локального максимума
            double localMax = array[0];
            for (int i = 1; i < arraySize; i++)
            {
                if (array[i] > localMax)
                    localMax = array[i];
            }

            // Поиск глобального максимума - ИСПРАВЛЕННАЯ ВЕРСИЯ
            double globalMax = comm.Reduce<double>(localMax, Operation<double>.Max, 0);

            if (rank == 0)
            {
                Console.WriteLine($"Global Max = {globalMax}");
            }
        }
    }
}