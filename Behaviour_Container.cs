using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Behaviour_Container : MonoBehaviour {

    [SerializeField] private new string name; // Container name
    public string Name
    {
        get { return name; }
    }
    [SerializeField] private List<GameObject> items; // List of items inside
    [StringInList(typeof(Global_ItemRandomizer), "ItemGroups")] public string itemGroup; // Contains items of group, used by randomizer
    private int health; // Container health
    private bool isBroken; // If health depleted status

    [SerializeField] private Sprite fullSprite; // Visual clue that container has items
    [SerializeField] private Sprite emptySprite;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        if (items.Count == 0)
        {
            spriteRenderer.sprite = emptySprite;
        }
        else
            spriteRenderer.sprite = fullSprite;
	}

    public static void TransferItem(Behaviour_Container from , Behaviour_Container to , int id) //Used to transfer items between containers
    {
        to.items.Add(from.items[id]);
        from.items.RemoveAt(id);
    }

    public void AddItem(GameObject item) // Add item to container
    {
        items.Add(item);
    }

    public List<Info_Item> GetItemInfo() // Lists all info of contained items
    {
        List<Info_Item> list = new List<Info_Item>();
        foreach(GameObject item in items)
        {
            list.Add(item.GetComponent<Stats_Item>().GetItemInfo());
            list[list.Count - 1].Sprite = item.GetComponent<SpriteRenderer>().sprite;
        }
        return list;
    }

    public void Search() // Player selected action in menu
    {
        UI_Container.Container = this;
        GameObject.FindWithTag("Canvas").GetComponent<Controls_GameUI>().SearchContainer();
    }
}
