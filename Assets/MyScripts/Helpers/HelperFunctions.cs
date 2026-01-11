
using UnityEngine;
using System;
using LargeNumbers;
using LargeNumbers.Example;
public class HelperFunctions : MonoBehaviour
{

    public static HelperFunctions Instance { get; set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public AlphabeticNotation GetLeftover(AlphabeticNotation amount, AlphabeticNotation currentlyInStorage)
    { // returns leftover so that values dont reach minus values
        AlphabeticNotation leftOver = new LargeNumber(0);
        leftOver = AlphabeticNotationUtils.Min(amount, currentlyInStorage);
        return leftOver;
    }

    public AlphabeticNotation GetMaxPossible(AlphabeticNotation amount, AlphabeticNotation maxStorage)
    { // returns max to add without going over storage cap
        return AlphabeticNotationUtils.Min(amount, maxStorage);
    }

    //Generates a unique id for objects. Send it the object name to get "name" + uniqueid as the new unique id
    public string GenerateUniqueId()
    {
        return System.Guid.NewGuid().ToString();
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


    public AlphabeticNotation CalculateOutputPerMin(AlphabeticNotation amount, float time)
    {
        if (time <= 0f)
        {
            Debug.LogWarning("Time must be greater than zero to calculate output per minute.");
            return new AlphabeticNotation(0);
        }

        // amount per second = amount / time
        // per minute = * 60
        return (amount / time) * 60f;
    }


    public static TextMesh CreateWorldText(
        string text,
        Transform parent = null,
        Vector3 localPosition = default,
        int fontSize = 40,
        Color? color = null,
        TextAnchor textAnchor = TextAnchor.MiddleCenter,
        TextAlignment textAlignment = TextAlignment.Center,
        int sortingOrder = 0)
    {
        if (color == null) color = Color.white;

        return CreateWorldText(
            parent,
            text,
            localPosition,
            fontSize,
            (Color)color,
            textAnchor,
            textAlignment,
            sortingOrder
        );
    }

    // Create Text in the World (full version)
    public static TextMesh CreateWorldText(
        Transform parent,
        string text,
        Vector3 localPosition,
        int fontSize,
        Color color,
        TextAnchor textAnchor,
        TextAlignment textAlignment,
        int sortingOrder)
    {
        GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
        Transform transform = gameObject.transform;

        transform.SetParent(parent, false);
        transform.localPosition = localPosition;

        TextMesh textMesh = gameObject.GetComponent<TextMesh>();
        textMesh.anchor = textAnchor;
        textMesh.alignment = textAlignment;
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;

        MeshRenderer meshRenderer = textMesh.GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = sortingOrder;

        return textMesh;
    }
}
