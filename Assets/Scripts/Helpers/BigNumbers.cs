using System;
using System.Diagnostics;

[Serializable]
public struct BigNumber
{
    public double number;
    public int exponent;

public BigNumber(double number, int exponent)
{
    if (number == 0)
    {
        this.number = 0;
        this.exponent = 0;
    }
    else
    {
        Normalize(ref number, ref exponent);
        this.number = number;
        this.exponent = exponent;
    }
}
    public static BigNumber operator +(BigNumber a, BigNumber b)
    {
        if (a.number == 0) return b;
        if (b.number == 0) return a;

        if (a.exponent > b.exponent + 10) return a; // a much bigger
        if (b.exponent > a.exponent + 10) return b; // b much bigger

        int diff = a.exponent - b.exponent;
        if (diff > 0)
            return new BigNumber(a.number + b.number / Math.Pow(10, diff), a.exponent);
        else
            return new BigNumber(b.number + a.number / Math.Pow(10, -diff), b.exponent);
    }

    public static BigNumber operator *(BigNumber a, BigNumber b)
    {
        double newnumber = a.number * b.number;
        int newExponent = a.exponent + b.exponent;
        return new BigNumber(newnumber, newExponent);
    }

    public static BigNumber operator /(BigNumber a, BigNumber b)
    {
        double newnumber = a.number / b.number;
        int newExponent = a.exponent - b.exponent;
        return new BigNumber(newnumber, newExponent);
    }

    public override string ToString()
    {
        if (exponent < 6)
            return (number * Math.Pow(10, exponent)).ToString("N0");
        else
            return $"{number:F2}e{exponent}";
    }

    private static void Normalize(ref double number, ref int exponent)
    {
        while (number >= 10)
        {
            number /= 10;
            exponent++;
        }
        while (number < 1 && number > 0)
        {
            number *= 10;
            exponent--;
        }
    }
    public BigNumber Normalized()
{
    double num = this.number;
    int exp = this.exponent;
    Normalize(ref num, ref exp);
    return new BigNumber(num, exp);
}
    public static implicit operator BigNumber(double value)
    {
        return new BigNumber(value, 0);
    }
public static bool operator <(BigNumber a, BigNumber b)
{
    a = a.Normalized();
    b = b.Normalized();

    if (a.exponent != b.exponent)
        return a.exponent < b.exponent;
    return a.number < b.number;
}

public static bool operator >(BigNumber a, BigNumber b)
{
    a = a.Normalized();
    b = b.Normalized();

    if (a.exponent != b.exponent)
        return a.exponent > b.exponent;
    return a.number > b.number;
}

public static bool operator >=(BigNumber a, BigNumber b)
{
    a = a.Normalized();
    b = b.Normalized();
    return a > b || a == b;
}

public static bool operator <=(BigNumber a, BigNumber b)
{
    a = a.Normalized();
    b = b.Normalized();
    return a < b || a == b;
}

public static bool operator ==(BigNumber a, BigNumber b)
{
    a = a.Normalized();
    b = b.Normalized();

    return a.exponent == b.exponent && Math.Abs(a.number - b.number) < 1e-9;
}

    public static bool operator !=(BigNumber a, BigNumber b)
    {
        return !(a == b);
    }

    // Also override Equals and GetHashCode when you overload == and !=
    public override bool Equals(object obj)
    {
        if (obj is BigNumber other)
            return this == other;
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(number, exponent);
    }

    public static BigNumber operator -(BigNumber a, BigNumber b)
{
    if (b.number == 0) return a;

    // Align exponents before subtracting
    int diff = a.exponent - b.exponent;

    if (diff > 10) return a;      // a much bigger
    if (diff < -10) return new BigNumber(-b.number, b.exponent); // b much bigger

    if (diff > 0)
        return new BigNumber(a.number - b.number / Math.Pow(10, diff), a.exponent);
    else
        return new BigNumber(a.number / Math.Pow(10, -diff) - b.number, b.exponent);
}
    public static BigNumber Parse(string input)
{
    if (string.IsNullOrEmpty(input))
        return new BigNumber(0, 0);

    input = input.Trim();

    // --- Case 1: scientific notation e.g. "1.23e12"
    if (input.Contains("e", StringComparison.OrdinalIgnoreCase))
    {
        var parts = input.Split('e', 'E');
        if (parts.Length == 2 &&
            double.TryParse(parts[0], out double num) &&
            int.TryParse(parts[1], out int exp))
        {
            return new BigNumber(num, exp);
        }
    }

    // --- Case 2: plain numeric string e.g. "1000000"
    if (double.TryParse(input, out double value))
    {
        return new BigNumber(value, 0);
    }

    // --- Case 3: suffixed format e.g. "1.23K", "4.56M", "7B", etc.
    char lastChar = input[input.Length - 1];
    double multiplier = 1;
    switch (char.ToUpper(lastChar))
    {
        case 'K': multiplier = 1e3; break;
        case 'M': multiplier = 1e6; break;
        case 'B': multiplier = 1e9; break;
        case 'T': multiplier = 1e12; break;
        case 'Q': multiplier = 1e15; break; // Quadrillion-ish
    }

    string numericPart = multiplier == 1 ? input : input.Substring(0, input.Length - 1);
    if (double.TryParse(numericPart, out double numValue))
    {
        return new BigNumber(numValue * multiplier, 0);
    }

    return new BigNumber(0, 0);
}
public static bool TryParse(string input, out BigNumber result)
{
    try
    {
        result = Parse(input);
        return true;
    }
    catch
    {
        result = new BigNumber(0, 0);
        return false;
    }
}
public static BigNumber Min(BigNumber a, BigNumber b)
{
    UnityEngine.Debug.Log($"Comparing: a = {a}, b = {b}, a < b = {a < b}");
    return a < b ? a : b;
}
public static BigNumber Max(BigNumber a, BigNumber b)
{
    return a > b ? a : b;
}
public int CompareTo(BigNumber other)
{
    if (this > other) return 1;
    if (this < other) return -1;
    return 0;
}
}
