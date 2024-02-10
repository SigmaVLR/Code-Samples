using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType { Shop, Keeper, Restaurant, Cart }

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public Image[] CurrentItemImage;
    public int newCurrentItem;
    public Items[] ItemSlots;
    public GameObject SlotPrefab;
    public GameObject UseablePrefab;
    public Transform InventoryPanel;
    public List<Items> ItemInventory;
    public List<Items> KeeperInventory;
    public List<Items> RestInventory;
    public List<Items> CartInventory;


    public GameObject KeeperInventoryUI;
    public GameObject RestInventoryUI;
    public GameObject CartInventoryUI;



    void Awake()
    {
        Instance = this;
        //KeeperInventoryUI = GameObject.Find("Keeper Inventory");
        //RestInventoryUI = GameObject.Find("Restuarant Inventory");
        //CartInventoryUI = GameObject.Find("Cart Inventory");
    }

    public void SetImage(Sprite newImage)
    {
        CurrentItemImage[newCurrentItem].sprite = newImage;
    }

    public void addItem(Items newitem)
    {
        if (ItemSlots[newCurrentItem] != null)
        {
            RemoveItem(ItemSlots[newCurrentItem]);
            newitem.Equipped = true;
            ItemSlots[newCurrentItem] = newitem;
            SkillsManager.Instance.AddItemIncrease(newitem.Stat, newitem);
        }
        else {
            newitem.Equipped = true;
            ItemSlots[newCurrentItem] = newitem;
            SkillsManager.Instance.AddItemIncrease(newitem.Stat, newitem);
        }
    }

    public void addInventory( Items newitem)
    {
        switch (newitem.Type)
        {
            case ItemType.Shop:
                ItemInventory.Add(newitem);
                UpdateInventoryUI(newitem);
                break;

            case ItemType.Cart:
                CartInventory.Add(newitem);
                UpdateCartInventory(newitem);
                break;

            case ItemType.Keeper:
                KeeperInventory.Add(newitem);
                UpdateKeepertInventory(newitem);
                break;

            case ItemType.Restaurant:
                RestInventory.Add(newitem);
                UpdateRestInventory(newitem);
                break;
        }
        
    }

    public void UpdateInventoryUI(Items newitem)
    {
        GameObject prefab = Instantiate(SlotPrefab);
        prefab.transform.SetParent(InventoryPanel, false);

        ItemSlot slot = prefab.GetComponent<ItemSlot>();
        slot.SetSlot(newitem);
        Debug.Log("Updating Inventory");
    }

    public void UpdateCartInventory(Items newitem)
    {
        GameObject prefab = Instantiate(UseablePrefab);
        prefab.transform.SetParent(CartInventoryUI.transform, false);

        ItemSlotUsable slot = prefab.GetComponent<ItemSlotUsable>();
        slot.SetSlot(newitem);
    }

    public void UpdateRestInventory(Items newitem)
    {
        GameObject prefab = Instantiate(UseablePrefab);
        prefab.transform.SetParent(RestInventoryUI.transform, false);

        ItemSlotUsable slot = prefab.GetComponent<ItemSlotUsable>();
        slot.SetSlot(newitem);
    }

    public void UpdateKeepertInventory(Items newitem)
    {
        GameObject prefab = Instantiate(UseablePrefab);
        prefab.transform.SetParent(KeeperInventoryUI.transform, false);

        ItemSlotUsable slot = prefab.GetComponent<ItemSlotUsable>();
        slot.SetSlot(newitem);
    }

    public void RemoveItem(Items item)
    {
            item.Equipped = false;
            SkillsManager.Instance.RemoveItemIncrease(item.Stat, item);

    }
    // Update is called once per frame
    void Update()
    {
        
    }


}
