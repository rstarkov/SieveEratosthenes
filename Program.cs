using System;
using System.Runtime.InteropServices;

namespace SieveEratosthenes
{
    class Program
    {
        static void Main(string[] args)
        {
            var start = DateTime.UtcNow;
            var map = new PrimeMap();
            Console.WriteLine($"{(DateTime.UtcNow - start).TotalMilliseconds:#,0} ms");

            map.Dispose(); // comment this out and watch the speed change

            start = DateTime.UtcNow;
            map = new PrimeMap();
            Console.WriteLine($"{(DateTime.UtcNow - start).TotalMilliseconds:#,0} ms");
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
    }
}
