using Arithmetic.BigInt.Interfaces;

namespace Arithmetic.BigInt.MultiplyStrategy;

internal class KaratsubaMultiplier : IMultiplier
{
    public BetterBigInteger Multiply(BetterBigInteger a, BetterBigInteger b)
    {
        int n = Math.Max(a.GetDigits().Length, b.GetDigits().Length);

        if (n <= 16)
        {
            return new SimpleMultiplier().Multiply(a, b);
        }

        int m = n / 2;

        BetterBigInteger a0 = GetPart(a, 0, m);
        BetterBigInteger a1 = GetPart(a, m, n);
        
        BetterBigInteger b0 = GetPart(b, 0, m);
        BetterBigInteger b1 = GetPart(b, m, n);

        BetterBigInteger z2 = Multiply(a1, b1);

        BetterBigInteger z0 = Multiply(a0, b0);

        BetterBigInteger z1 = Multiply(a0 + a1, b0 + b1);

        BetterBigInteger middle = z1 - z2 - z0;

        return (z2 << (m * 32 * 2)) + (middle << (m * 32)) + z0;
    }


    private BetterBigInteger GetPart(BetterBigInteger val, int start, int end)
    {
        ReadOnlySpan<uint> digits = val.GetDigits();
        if (start >= digits.Length) 
            return new BetterBigInteger([0], false);

        int count = Math.Min(end, digits.Length) - start;
        if (count <= 0) 
            return new BetterBigInteger([0], false);

        return new BetterBigInteger(digits.Slice(start, count).ToArray(), false);
    }
}
