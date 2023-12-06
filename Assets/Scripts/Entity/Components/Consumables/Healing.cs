using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Consumable;
using static UnityEditor.Progress;

public class Healing : Consumable
{
    [SerializeField] private int amount = 0;
    public int Amount { get => amount; }

    public override bool Activate(Actor consumer)
    {
        int amountRecovered = consumer.GetComponent<Fighter>().Heal(amount);

        if (amountRecovered > 0)
        {
            UIManager.instance.AddMessage($"You consume the {name} and recover {amountRecovered} HP", "#00ff00");
            Consume(consumer);
            return true;
        }

        UIManager.instance.AddMessage("Your health is already full !", "#808080");
        return false;
    }

}
