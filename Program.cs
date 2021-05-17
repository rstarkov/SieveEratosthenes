using System;
using System.Runtime.InteropServices;
using RT.Util;

namespace SieveEratosthenes
{
    unsafe class Program
    {
        static void Main(string[] args)
        {
            double minTime = double.MaxValue;
            long minCycles = long.MaxValue;
            while (true)
            {
                var c = new TicTocCycles();
                Ut.Tic();
                c.Tic();
                var map = new PrimeMap();
                minCycles = Math.Min(minCycles, c.Toc());
                minTime = Math.Min(minTime, Ut.Toc());
                Console.WriteLine($"{minTime * 1000:#,0} ms, {minCycles:#,0} cycles");
            }
        }
    }

    unsafe class PrimeMap
    {
        ulong* _composite;

        public PrimeMap()
        {
            _composite = (ulong*) Marshal.AllocHGlobal(134_217_728); // 2^31 / 8 bits per byte / 2 to remove all even numbers

            for (int step = 3; step <= 46340 /* sqrt of 2^31 */; step += 2)
            {
                if (get(step))
                    continue;
                for (uint x = (uint) (step * step); x < int.MaxValue; x += (uint) step)
                    set((int) x);
            }
        }

        private bool get(int value)
        {
            if ((value & 1) == 0)
                return true;
            ulong c = _composite[value >> 7]; // 1 for even numbers + 6 for one of the 64 bits per ulong
            return ((c >> ((value >> 1) & 63)) & 1) == 1;
        }

        private void set(int value)
        {
            if ((value & 1) == 0)
                return;
            ulong* cell = _composite + (value >> 7);
            ulong c = *cell;
            c |= 1ul << ((value >> 1) & 63);
            *cell = c;
        }
    }
}
