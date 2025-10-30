using UnityEngine;
using System;
using System.Globalization;
public class ConvertNumbers : MonoBehaviour
{
    //UNUUSED DELETE
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
    double realValue = value.number * Math.Pow(10, value.exponent);

    if (realValue < 1000)
    {
        return Math.Floor(realValue).ToString(CultureInfo.InvariantCulture);
    }

    int exp = (int)(Math.Floor(Math.Log10(realValue) / 3));
    double scaled = realValue / Math.Pow(1000, exp);

    if (exp < suffixes.Length)
    {
        return scaled.ToString("0.##", CultureInfo.InvariantCulture) + suffixes[exp];
    }
    else
    {
        return realValue.ToString("0.##e0", CultureInfo.InvariantCulture);
    }
}

}
