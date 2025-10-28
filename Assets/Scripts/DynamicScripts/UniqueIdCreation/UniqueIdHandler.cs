using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using System;

public class UniqueIdHandler : MonoBehaviour
{

    public string uniqueId;
    public string GetUniqueId() => uniqueId;
    public void GenerateUniqueId(string prefix = "")
    {
        // 🕒 1. Get the current date and time as part of the ID
        string datePart = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");

        // 🔢 2. Generate a random 8-character string (using part of a GUID)
        string randomPart = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

        // 🧩 3. Combine the parts into a readable ID
        if (string.IsNullOrEmpty(prefix))
            uniqueId = $"{datePart}_{randomPart}";
        else
            uniqueId = $"{prefix}_{datePart}_{randomPart}";

        // Optional: log it for debugging
        Debug.Log($"[UniqueIdHandler] Generated unique ID: {uniqueId}");
    }
}
