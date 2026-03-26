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
            int temp = 0;

            if (rank == size - 1)
            {
                Console.Write("Enter a value: ");
                temp = int.Parse(Console.ReadLine());
                for (int i = 0; i < size - 1; i++)
                {
                    comm.Send(temp, i, 0);
                }
            }
            else
            {
                temp = comm.Receive<int>(size - 1, 0);
            }

            if (rank % 2 == 0 && rank < size - 1)
            {
                temp = -temp;
                comm.Send(temp, rank + 1, 0);
            }
            else if (rank % 2 != 0)
            {
                temp = comm.Receive<int>(rank - 1, 0);
            }

            Console.WriteLine($"Process {rank}: temp = {temp}");
        }
    }
}