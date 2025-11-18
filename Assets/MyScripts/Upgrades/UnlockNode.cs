using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UnlockNode : MonoBehaviour
{
    [SerializeField] private CardInfo cardInfo;
    // [SerializeField] private List<CardInfo> cardFromPreNode;
    [SerializeField] private Image nodeImage;
    private Color originalColor;
    [SerializeField] private GameObject buttonObject;
    [SerializeField] private bool defaultUnlocked;
    [SerializeField] private bool useNormalUpgradeNode = true;
    [SerializeField] private bool useMerchantUpgradeNode = false;
    [SerializeField] private bool setObjectInactiveUntilUnlock = false;
    [SerializeField] private GameObject objectToSetInactive;
    [SerializeField] private List<RequirementData> requirementDatas;
    private bool isMovedIntoScene = false;
    private bool isFullyUnlocked = false;
    private int currentlevel;
    private Vector3 originalPosition;


    [System.Serializable]
    public class RequirementData
    {
        public CardInfo cardFromPreNode;
        public MerchantCardHandler merchantCardFromPreNode;
        public int requiredLevel;
        public int moveToSceneLevel;
    }
    private void Start()
    {

        if (setObjectInactiveUntilUnlock)
        {
            originalPosition = objectToSetInactive.transform.position;
        }
        originalColor = nodeImage.color;
        if (defaultUnlocked)
        {
            UnlockMyNode();
        }
        else
        {
            LockMyNode();
        }
    }
    private void OnEnable()
    {
        for (int i = 0; i < requirementDatas.Count; i++)
        {
            if (useNormalUpgradeNode)
            {
                requirementDatas[i].cardFromPreNode.OnBought += CheckRequirements;
            }
            else if (useMerchantUpgradeNode)
            {
                requirementDatas[i].merchantCardFromPreNode.OnBought += CheckRequirements;
            }
        }

    }
    private void OnDisable()
    {
        for (int i = 0; i < requirementDatas.Count; i++)
        {
            if (useNormalUpgradeNode)
            {
                requirementDatas[i].cardFromPreNode.OnBought -= CheckRequirements;
            }
            else if (useMerchantUpgradeNode)
            {
                requirementDatas[i].merchantCardFromPreNode.OnBought -= CheckRequirements;
            }
        }

    }

private void CheckRequirements()
{
    bool allReadyToMove = true;
    bool allReadyToUnlock = true;

    for (int i = 0; i < requirementDatas.Count; i++)
    {
        int level = useNormalUpgradeNode ?
            requirementDatas[i].cardFromPreNode.level :
            requirementDatas[i].merchantCardFromPreNode.upgradeLevel;

        // Check move-to-scene requirement
        if (level < requirementDatas[i].moveToSceneLevel)
            allReadyToMove = false;

        // Check full unlock requirement
        if (level < requirementDatas[i].requiredLevel)
            allReadyToUnlock = false;
    }

    // Only move when ALL requirements are satisfied
    if (!isMovedIntoScene && allReadyToMove)
        MoveIntoScene();

    // Only unlock when ALL requirements are satisfied
    if (!isFullyUnlocked && allReadyToUnlock)
        FullUnlock();
}

    private void MoveIntoScene()
    {
        isMovedIntoScene = true;
        if (setObjectInactiveUntilUnlock)
            objectToSetInactive.transform.position = originalPosition;
    }

    private void FullUnlock()
    {
        isFullyUnlocked = true;
        buttonObject.SetActive(true);
        nodeImage.color = originalColor;
        OnDisable();
    }

    public void UnlockMyNode()
    {
        buttonObject.SetActive(true);
        nodeImage.color = originalColor;
        if (setObjectInactiveUntilUnlock)
        {
            objectToSetInactive.transform.position = originalPosition;
        }
        OnDisable();
    }

    public void LockMyNode()
    {
        if (setObjectInactiveUntilUnlock)
        {
            originalPosition = objectToSetInactive.transform.position;
            objectToSetInactive.transform.position = new Vector3(9999, 9999, 9999);
        }

        buttonObject.SetActive(false);
        nodeImage.color = new Color(0f, 0f, 0f, 0.5f);
    }

}
