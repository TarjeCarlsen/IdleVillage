using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using System;
public class HelperFunctions : MonoBehaviour
{

    public static HelperFunctions Instance {get; set;}

    private void Awake(){
        if(Instance != null && Instance != this){
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public BigNumber GetLeftover(BigNumber amount, BigNumber currentlyInStorage){ // returns leftover so that values dont reach minus values
        BigNumber leftOver = 0;
        leftOver = BigNumber.Min(amount, currentlyInStorage);    
        return leftOver;
    }

    public BigNumber GetMaxPossible(BigNumber amount, BigNumber maxStorage){ // returns max to add without going over storage cap
        return BigNumber.Min(amount, maxStorage);
    }

//Generates a unique id for objects. Send it the object name to get "name" + uniqueid as the new unique id
    public string GenerateUniqueId(string prefix = "")
    {
        // Format: <prefix>_<timestamp>_<GUID>
        string guidPart = Guid.NewGuid().ToString("N").Substring(0, 8); // shorter but still unique
        string timePart = DateTime.UtcNow.Ticks.ToString("X");          // based on time
        return string.IsNullOrEmpty(prefix)
            ? $"{timePart}_{guidPart}"
            : $"{prefix}_{timePart}_{guidPart}";
    }

    public string ConvertSecondsToTime(float totalSeconds)
{
    totalSeconds = Mathf.Max(0, totalSeconds); // prevent negatives
    int hours = Mathf.FloorToInt(totalSeconds / 3600f);
    int minutes = Mathf.FloorToInt((totalSeconds % 3600f) / 60f);
    int seconds = Mathf.FloorToInt(totalSeconds % 60f);

    if (hours > 0)
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    else
        return $"{minutes:D2}:{seconds:D2}";
}
public float ConvertTimeToSeconds(string timeString)
{
    if (string.IsNullOrWhiteSpace(timeString))
        return 0f;

    string[] parts = timeString.Split(':');

    try
    {
        if (parts.Length == 3)
        {
            // Format: HH:MM:SS
            int hours = int.Parse(parts[0]);
            int minutes = int.Parse(parts[1]);
            int seconds = int.Parse(parts[2]);

            return hours * 3600f + minutes * 60f + seconds;
        }
        else if (parts.Length == 2)
        {
            // Format: MM:SS
            int minutes = int.Parse(parts[0]);
            int seconds = int.Parse(parts[1]);

            return minutes * 60f + seconds;
        }
        else if (parts.Length == 1)
        {
            // Just seconds
            return float.Parse(parts[0]);
        }
    }
    catch (FormatException)
    {
        Debug.LogWarning("Invalid time string: " + timeString);
    }

    return 0f; // fallback if input is invalid
}
}
