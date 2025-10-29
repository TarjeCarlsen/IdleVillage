using UnityEngine;
using System;
using System.Globalization;
public class ConvertNumbers : MonoBehaviour
{
    public static ConvertNumbers Instance { get; private set; }

    private static readonly string[] suffixes =
    {
        "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No",
        "Dc", "Ud", "Dd", "Td", "Qad", "Qid", "Sxd", "Spd", "Ocd", "Nod"
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


public string FormatNumber(double value)
{
    if (value < 1000)
    {
        print("INSIDE HERE "+ value);
        return Math.Floor(value).ToString(); // This shows raw integer like "942"
    }

    int exp = (int)(Math.Floor(Math.Log10(value) / 3));
    if (exp < suffixes.Length && exp <= 7)
    {
        double scaled = value / Math.Pow(1000, exp);
        return $"{scaled:0.##}{suffixes[exp]}";
    }
    else
    {
        return value.ToString("0.##e0");
    }
}

public string FormatNumber(BigNumber value)
{
    double num = value.number;
    int exp = value.exponent;

    // Calculate real-world value
    double realValue = num * Math.Pow(10, exp);

    // ✅ Check if final value is under 1000
    if (realValue < 1000)
    {
        return Math.Floor(realValue).ToString(CultureInfo.InvariantCulture);
    }

    while (num >= 1000)
    {
        num /= 1000;
        exp += 3;
    }
    while (num < 1 && exp > 0)
    {
        num *= 1000;
        exp -= 3;
    }

    int expGroup = exp / 3;

    if (expGroup < suffixes.Length && exp <= 63)
    {
        return num.ToString("0.##", CultureInfo.InvariantCulture) + suffixes[expGroup];
    }
    else
    {
        return num.ToString("0.##", CultureInfo.InvariantCulture) + $"e{exp}";
    }
}

}
