using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Inventory : MonoBehaviour
{
    [SerializeField] private int capacity = 0;
    [SerializeField] private Consumable selectedConsumable = null;
    [SerializeField] private List<Item> items = new List<Item>();

    public int Capacity { get => capacity; }
    public Consumable SelectedConsumable { get; set; }
    public List<Item> Items { get => items; }

    public void Drop(Item item)
    {
        items.Remove(item);
        item.transform.SetParent(null);
        item.GetComponent<SpriteRenderer>().enabled = true;
        item.AddToGameMAnager();
        UIManager.instance.AddMessage($"You dropped the {item.name}", "#ff0000");
    }

}