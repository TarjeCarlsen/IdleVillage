using UnityEngine;

public class SetHouseType : MonoBehaviour
{
    public HouesTypes houesType;
    public HouesTypes GetHouseType() => houesType;

    [SerializeField] CardInfo cardInfo;
    [SerializeField] InventoryHandler inventoryHandler;

    private void OnEnable(){
        cardInfo.OnBought += AddToInventory;
    }
    private void OnDisable(){
        cardInfo.OnBought -= AddToInventory;
    }
    private void AddToInventory(){
        inventoryHandler.AddToInv(houesType);
    }
}
