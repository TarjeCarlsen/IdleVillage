using UnityEditor;
using UnityEngine;
public class EditorHotekeys : MonoBehaviour
{


public static class InspectorLockToggle
{
    [MenuItem("Tools/Toggle Inspector Lock _F1")]
    private static void ToggleInspectorLock()
    {
        var inspectorType = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow");
        var inspectorWindow = EditorWindow.GetWindow(inspectorType);
        var isLockedProperty = inspectorType.GetProperty("isLocked");

        bool isLocked = (bool)isLockedProperty.GetValue(inspectorWindow);
        isLockedProperty.SetValue(inspectorWindow, !isLocked);

        inspectorWindow.Repaint();
    }
}
}
