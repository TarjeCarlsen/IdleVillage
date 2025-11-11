using TMPro;
using UnityEngine;

public class CreateTmpText : MonoBehaviour
{
    [SerializeField] private Transform parentToSpawnUnder;
    [SerializeField] private TMP_FontAsset fontAsset;
    [SerializeField] private float fontSize = 36f;

    private static CreateTmpText instance;

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Creates a TMP_Text under the assigned parent with the given text.
    /// Returns the TMP_Text reference for further modification.
    /// </summary>
    public static TMP_Text CreateText(string message)
    {
        if (instance == null)
        {
            Debug.LogError("CreateTmpText: No instance found in scene!");
            return null;
        }

        GameObject textObj = new GameObject("Generated TMP Text");
        textObj.transform.SetParent(instance.parentToSpawnUnder, false);

        TMP_Text tmpText = textObj.AddComponent<TextMeshProUGUI>();
        tmpText.text = message;
        tmpText.font = instance.fontAsset;
        tmpText.fontSize = instance.fontSize;
        tmpText.alignment = TextAlignmentOptions.Center;

        return tmpText;
    }
}
