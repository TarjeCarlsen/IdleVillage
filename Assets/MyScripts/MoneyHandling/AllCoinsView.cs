using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LargeNumbers;

public class AllCoinsView : MonoBehaviour
{
    [SerializeField] private List<Sprite> currencyIcons;
    [SerializeField] private Sprite backgroundSprite;

    private List<Image> currencyIcon_list = new();
    [SerializeField] private List<TextMeshProUGUI> currencyText_list = new();
    private List<GameObject> parentObjects = new();

    private int counter = 0;

    private void Start()
    {
        CreateCurrencyObject();

        foreach (CurrencyTypes type in System.Enum.GetValues(typeof(CurrencyTypes)))
            UpdateCurrency(type);
    }

    private void OnEnable()
    {
        if (MoneyManager.Instance != null)
            MoneyManager.Instance.OnCurrencyChanged += UpdateCurrency;

        if (StorageManager.Instance != null)
        {
            StorageManager.Instance.OnStorageChange += UpdateCurrency;
            foreach (CurrencyTypes type in System.Enum.GetValues(typeof(CurrencyTypes)))
                UpdateCurrency(type);
        }
    }

    private void OnDisable()
    {
        if (MoneyManager.Instance != null)
            MoneyManager.Instance.OnCurrencyChanged -= UpdateCurrency;

        if (StorageManager.Instance != null)
            StorageManager.Instance.OnStorageChange -= UpdateCurrency;
    }

    public void CreateCurrencyObject()
    {
        for (int i = 0; i < MoneyManager.Instance.currency.Count; i++)
        {
            MakeCurrencyRow(i);
        }
    }

    // ✅ NEW: One function builds a complete "row" with background, icon, and text
private void MakeCurrencyRow(int index)
{
    // === Row container ===
    GameObject rowObj = new GameObject("CurrencyRow_" + index);
    rowObj.transform.SetParent(this.transform, false);

    RectTransform rowRect = rowObj.AddComponent<RectTransform>();
    rowRect.anchorMin = new Vector2(0, 1);
    rowRect.anchorMax = new Vector2(1, 1);
    rowRect.pivot = new Vector2(0.5f, 1);
    rowRect.sizeDelta = new Vector2(85 , 25); // 👈 defines how wide & tall each row is

    // === Background ===
    GameObject bgObj = new GameObject("Background_" + index);
    bgObj.transform.SetParent(rowObj.transform, false);

    RectTransform bgRect = bgObj.AddComponent<RectTransform>();
    bgRect.anchorMin = new Vector2(0, 0);
    bgRect.anchorMax = new Vector2(1, 1);
    bgRect.offsetMin = Vector2.zero;
    bgRect.offsetMax = Vector2.zero;
    bgRect.sizeDelta = new Vector2(0, 45); // 👈 matches row height

    Image bgImage = bgObj.AddComponent<Image>();
    bgImage.sprite = backgroundSprite;
    bgImage.type = Image.Type.Sliced;
    bgImage.color = Color.white;

    // === Grid layout on background ===
    GridLayoutGroup grid = bgObj.AddComponent<GridLayoutGroup>();
    grid.padding = new RectOffset(-133, 0, 0, 0);
    grid.cellSize = new Vector2(95, 50);
    grid.spacing = new Vector2(55, 0);
    grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
    grid.startAxis = GridLayoutGroup.Axis.Horizontal;
    grid.childAlignment = TextAnchor.MiddleCenter;
    grid.constraint = GridLayoutGroup.Constraint.FixedRowCount;
    grid.constraintCount = 1;

    // === Icon ===
    GameObject iconObj = new GameObject("Icon_" + index);
    iconObj.transform.SetParent(bgObj.transform, false);

    Image iconImg = iconObj.AddComponent<Image>();
    iconImg.sprite = currencyIcons[index];
    iconImg.preserveAspect = true;

    RectTransform iconRect = iconObj.GetComponent<RectTransform>();
    iconRect.sizeDelta = new Vector2(0, 0); // 👈 controlled icon size

    currencyIcon_list.Add(iconImg);

    // === Text ===
    GameObject textObj = new GameObject("CurrencyText_" + index);
    textObj.transform.SetParent(bgObj.transform, false);

    TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
    text.text = MoneyManager.Instance.GetCurrency((CurrencyTypes)index).ToString();
    text.alignment = TextAlignmentOptions.Center;
    text.color = Color.black;
    text.fontSize = 28;
    text.fontStyle = FontStyles.Bold;
    text.textWrappingMode = TextWrappingModes.NoWrap;

    RectTransform textRect = text.GetComponent<RectTransform>();
    textRect.sizeDelta = new Vector2(150, 100);
    currencyText_list.Add(text);

    parentObjects.Add(rowObj);
    counter++;
}


    private void UpdateCurrency(CurrencyTypes type)
    {
        int index = (int)type;
        if (index < currencyText_list.Count)
        {
            AlphabeticNotation value = MoneyManager.Instance.GetCurrency(type);
            AlphabeticNotation max = StorageManager.Instance.GetMaxStorage(type);
            currencyText_list[index].text = $"{ value.ToStringSmart(1)}{" "} / {" "}{max.ToStringSmart(1)}";
        }
    }
}
