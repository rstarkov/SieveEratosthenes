using System;
using System.Runtime.CompilerServices;
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

                //for (int i = 0; i < 20000; i++)
                //    if (map.IsPrime(i))
                //        Console.WriteLine(i);

                map.Dispose();
            }
        }
    }

    unsafe class PrimeMap : IDisposable
    {
        ulong* _composite;

        public PrimeMap()
        {
            _composite = (ulong*) Marshal.AllocHGlobal(134_217_728); // 2^31 / 8 bits per byte / 2 to remove all even numbers

            for (int step = 3; step <= 46340 /* sqrt of 2^31 */; step += 2)
            {
                if ((((_composite[step >> 7]) >> (step >> 1)) & 1) == 1)
                    continue;

                for (uint x = (uint) (step * step); x < int.MaxValue; x += (uint) step)
                    if ((x & 1) != 0)
                        _composite[x >> 7] |= 1ul << (((int) x) >> 1);
            }
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal((IntPtr) _composite);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsPrime(int value)
        {
            if (value == 2)
                return true;
            if (value < 2 || (value & 1) == 0)
                return false;
            ulong c = _composite[value >> 7]; // 1 for even numbers + 6 for one of the 64 bits per ulong
            return ((c >> ((value >> 1))) & 1) == 0;
        }
    }
}
