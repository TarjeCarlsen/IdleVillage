#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

public static class EditorHotkeys
{
    [MenuItem("Tools/Toggle Inspector Lock _F1")]
    private static void ToggleInspectorLock()
    {
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        if (inspectorType == null)
            return;

        var inspectors = Resources.FindObjectsOfTypeAll(inspectorType);
        if (inspectors == null || inspectors.Length == 0)
            return;

        // Toggle the first visible Inspector
        var inspectorWindow = inspectors[0] as EditorWindow;
        if (inspectorWindow == null)
            return;

        var isLockedProperty = inspectorType.GetProperty(
            "isLocked",
            System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.Public |
            System.Reflection.BindingFlags.NonPublic
        );

        if (isLockedProperty == null)
            return;

        bool isLocked = (bool)isLockedProperty.GetValue(inspectorWindow);
        isLockedProperty.SetValue(inspectorWindow, !isLocked);

        inspectorWindow.Repaint();
    }
}
#endif
