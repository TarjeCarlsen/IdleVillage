#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class UIPageTools
{
    [MenuItem("UI/Show Main Scene")]
    static void ShowMain()
    {
        ToggleCanvas("MainSceneCanvas");
    }

    [MenuItem("UI/Show Farm Page")]
    static void ShowFarm()
    {
        ToggleCanvas("FarmPageCanvas");
    }

    static void ToggleCanvas(string name)
    {
        Canvas[] canvases = Resources.FindObjectsOfTypeAll<Canvas>();

        // Find the target canvas first
        Canvas target = null;
        foreach (Canvas c in canvases)
        {
            if (c.hideFlags == HideFlags.None && c.name == name)
            {
                target = c;
                break;
            }
        }

        if (target == null)
        {
            Debug.LogWarning($"[UIPageTools] No canvas found with name '{name}'");
            return;
        }

        bool shouldShow = !target.gameObject.activeSelf; // toggle behavior

        // Hide all canvases
        foreach (Canvas c in canvases)
        {
            if (c.hideFlags == HideFlags.None)
                c.gameObject.SetActive(false);
        }

        // Show the selected one if it was hidden
        if (shouldShow)
        {
            target.gameObject.SetActive(true);
            Debug.Log($"[UIPageTools] Showing canvas: {name}");
        }
        else
        {
            Debug.Log($"[UIPageTools] Hiding canvas: {name}");
        }
    }
}

#endif