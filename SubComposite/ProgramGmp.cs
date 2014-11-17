using Emil.GMP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace SubComposite
{
    class ProgramGmp
    {
        static void Main(string[] args)
        {
            var parities = new[] { 0, 0, 0, 0, 0, 1, 1, 1, 0, 1, 0, 0, 1, 1, 1, 0, };
            var intermediateDigitOptions = Enumerable.Range(0, 10).Select(x => (BigInt)x).ToArray();
            var evenIntermediateDigitOptions = intermediateDigitOptions.Where(x => x % 2 == 0).ToArray();
            var oddIntermediateDigitOptions = intermediateDigitOptions.Where(x => x % 2 == 1).ToArray();
            var finalDigitOptions = new BigInt[] { 4, 6, 8, 9 };
            var evenFinalDigitOptions = finalDigitOptions.Where(x => x % 2 == 0).ToArray();
            //var oddFinalDigitOptions = finalDigitOptions.Where(x => x % 2 == 1).ToArray();
            int maxSize = 16;
            for (int size = 6; size <= maxSize; size++)
            {
                var sw = Stopwatch.StartNew();
                int finalCount = 0;

                // leftmost digits must be composite
                for (int i = 2; i <= size; i++)
                {
                    var parity = parities[15 - size + i];
                    var digitOptions = size == i ? evenFinalDigitOptions : (parity == 1 ? oddIntermediateDigitOptions : evenIntermediateDigitOptions);
                    using (var newList = File.Open("numbers" + (i + 1) + ".txt", FileMode.Create))
                    using (var newStream = new StreamWriter(newList, Encoding.UTF8))
                    {
                        foreach (var startingValue in GetValues(i, parities[16 - size]))
                        {
                            var start10 = startingValue * 10;
                            foreach (var append in digitOptions)
                            {
                                BigInt newVal = start10 + append;
                                if (!newVal.IsProbablyPrimeRabinMiller(1))
                                {
                                    var newValStr = newVal.ToString();
                                    if (size == i)
                                    {
                                        // rightmost digits must be composite, but this is already covered by the ending digit being even
                                        // also, three instances of leading zeroes
                                        if (GetZeroes(newValStr) == 3 && Luhn.LuhnCheck(newValStr))
                                        {
                                            finalCount++;
                                            newStream.WriteLine(newValStr);
                                        }
                                    }
                                    else
                                    {
                                        if (newValStr.Length + size - i >= 4)
                                        {
                                            var zeroCount = GetZeroes(newValStr);
                                            // if we're close enough to the end and don't have enough zereos, we can skip the number
                                            if (zeroCount + size - i >= 4 && zeroCount <= 3)
                                                newStream.WriteLine(newValStr);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                sw.Stop();
                Console.WriteLine("size {0}, count {1}, time {2}", size, finalCount, sw.Elapsed);
            }
            Console.ReadLine();
        }
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private static int GetZeroes(string str)
        {
            int count = 0;
            for (int i = 1; i < str.Length; i++)
            {
                if (str[i] == '0')
                    count++;
            }
            return count;
        }
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<BigInt> GetValues(int size, int parity)
        {
            if (size == 2)
                return parity == 0 ? new BigInt[] { 4, 6, 8 } : new BigInt[] { 9 };
            else
                return GetValuesFromFile(size);
        }
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private static IEnumerable<BigInt> GetValuesFromFile(int size)
        {
            using (var reader = File.OpenText("numbers" + size + ".txt"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    yield return new BigInt(line);
            }
            File.Delete("numbers" + size + ".txt");
        }
    }
}
