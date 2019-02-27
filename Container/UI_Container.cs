using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Container : MonoBehaviour {

    public GameObject itemPrefab;
    private static List<UI_Container> instances = new List<UI_Container>();
    private static List<GameObject> inventoryList;
    private static List<GameObject> containerList;
    private static Behaviour_Container inventory;
    private static Behaviour_Container container;
    public bool IsInventory{ get; private set; }
    public static Behaviour_Container Container {set { container = value; } }
    private static GameObject selectedItem;
    public static Info_Item SelectedItemInfo{ get; private set; }
    private static List<GameObject> selectedList;
    private static int selectedListID;

    private void Awake()
    {
        selectedListID = 0;
        instances.Add(this);
        IsInventory = name == ("Inventory") ? true : false;
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Behaviour_Container>();

    }
    private void OnEnable()
    {
        Refresh();
    }

    private void OnDestroy()
    {
        instances.Remove(this);
    }

    private void OnDisable()
    {
        Clear();
        selectedListID = 0;
    }

    public static void Scroll(float direction)
    {
        if(direction!=0 && selectedItem != null)
        {
            selectedItem.GetComponent<TMP_Text>().color = Color.white;
        }
        if (direction > 0 && selectedItem != selectedList[0])
        {
            selectedItem = selectedList[selectedList.FindIndex(x => x == selectedItem) - 1];
        }
        else if (direction < 0 && selectedItem != selectedList[selectedList.Count - 1])
        {
            selectedItem = selectedList[selectedList.FindIndex(x => x == selectedItem) + 1];
        }
        selectedItem.GetComponent<TMP_Text>().color = Color.red;
        UpdateInfo();
    }

    public static void ChooseContainer(float direction)
    {
        if (direction < 0 && inventoryList.Count!=0)
        {
            selectedList = inventoryList;
        }
        else if (direction > 0 && containerList.Count != 0)
        {
            selectedList = containerList;
        }
        selectedItem.GetComponent<TMP_Text>().color = Color.white;
        selectedItem = selectedList[0];
        selectedItem.GetComponent<TMP_Text>().color = Color.red;
        UpdateInfo();
    }

    public static void TossItem()
    {
        if (selectedList == inventoryList)
        {
            Behaviour_Container.TransferItem(inventory, container, selectedList.IndexOf(selectedItem));
            selectedListID = 1;
        }
        else
        {
            Behaviour_Container.TransferItem(container, inventory, selectedList.IndexOf(selectedItem));
            selectedListID = 0;
        }
        foreach (UI_Container instance in instances)
        {
            instance.Clear();
            instance.Refresh();
        }
                    
    }

    private void Refresh()
    {
        List<Info_Item> items;
        if (IsInventory)
        {
            inventoryList = new List<GameObject>();
            items = inventory.GetItemInfo();
        }
        else
        {
            containerList = new List<GameObject>();
            items = container.GetItemInfo();
        }
        for (int i = 0; i < items.Count; i++)
        {
            RectTransform Rect = GetComponent<RectTransform>();
            var obj = Instantiate(itemPrefab, Rect.position + new Vector3(0, -30 * i, 0), Quaternion.identity, transform.Find("Viewport").Find("Content"));
            obj.GetComponent<TMP_Text>().text = items[i].Name;
            obj.GetComponent<UI_ContainerItem>().item = items[i];
            if (IsInventory)
            {
                inventoryList.Add(obj);
            }
            else
                containerList.Add(obj);
        }
        if (containerList != null)
        {
            if (containerList.Count != 0 && selectedListID == 0)
            {
                selectedList = containerList;
                selectedItem = selectedList[0];
                selectedItem.GetComponent<TMP_Text>().color = Color.red;
            }
            else if (inventoryList.Count != 0)
            {
                selectedList = inventoryList;
                selectedItem = selectedList[0];
                selectedItem.GetComponent<TMP_Text>().color = Color.red;
            }
            else
                selectedListID = 0;
            UpdateInfo();
        }
    }

    private static void UpdateInfo()
    {
        SelectedItemInfo = selectedItem.GetComponent<UI_ContainerItem>().item;
        UI_Phone.Instance.ShowItemScreen(SelectedItemInfo);
    }

    private void Clear()
    {
        if (IsInventory)
        {
            foreach (GameObject item in inventoryList)
            {
                Destroy(item);
            }
            inventoryList = null;
        }
        else
        {
            foreach (GameObject item in containerList)
            {
                Destroy(item);
            }
            containerList = null;
        }
    }
}
