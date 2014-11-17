using System.Linq;
using System.Runtime.CompilerServices;

namespace SubComposite
{
    public static class Luhn
    {
        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        public static bool LuhnCheck(this string cardNumber)
        {
            return LuhnCheck(cardNumber.Select(c => c - '0').ToArray());
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private static bool LuhnCheck(this int[] digits)
        {
            return GetCheckValue(digits) == 0;
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private static int GetCheckValue(int[] digits)
        {
            return digits.Select((d, i) => i % 2 == digits.Length % 2 ? ((2 * d) % 10) + d / 5 : d).Sum() % 10;
        }
    }
}
