using UnityEngine;
using System;

public class ConvertNumbers : MonoBehaviour
{
    public static ConvertNumbers Instance { get; private set; }

    private static readonly string[] suffixes =
    {
        "", "K", "M", "B", "T", "Qa", "Qi", "Sx", "Sp", "Oc", "No",
        "Dc", "Ud", "Dd", "Td", "Qad", "Qid", "Sxd", "Spd", "Ocd", "Nod"
    };
    // Up to 10^63, but we’ll switch to scientific after 10^21 per your request.

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

    /// <summary>
    /// Formats a regular double into short notation (1K, 1M, etc.)
    /// </summary>
    public string FormatNumber(double value)
    {
        if (value < 1000)
            return value.ToString("0.##");

        int exp = (int)(Math.Floor(Math.Log10(value) / 3)); // how many 1000s
        if (exp < suffixes.Length && exp <= 7) // up to T (10^12) or Qa/Qi if you want more
        {
            double scaled = value / Math.Pow(1000, exp);
            return $"{scaled:0.##}{suffixes[exp]}";
        }
        else
        {
            // past 10^21, fallback to scientific notation
            return value.ToString("0.##e0");
        }
    }

    /// <summary>
    /// Formats a BigNumber instance into short notation (1K, 1M, etc.)
    /// </summary>
    public string FormatNumber(BigNumber value)
    {
        // Convert exponent from base-10 to thousands
        int expGroup = value.exponent / 3;
        if (expGroup < suffixes.Length && value.exponent <= 21)
        {
            double scaled = value.number * Math.Pow(10, value.exponent % 3);
            return $"{scaled:0.##}{suffixes[expGroup]}";
        }
        else
        {
            return $"{value.number:0.##}e{value.exponent}";
        }
    }

}
