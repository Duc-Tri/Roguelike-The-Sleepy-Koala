using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Lighting : Consumable
{

    [SerializeField] private int damage = 20;
    [SerializeField] private int maxiumRange = 5;
    public int Damage => damage;
    public int MaxiumRange => maxiumRange;

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode();
        UIManager.instance.AddMessage($"Select a target to strike", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, Actor target)
    {
        UIManager.instance.AddMessage($"A lighting bolt strikes the {target.name} with a loud thunder, for {damage} damage", "#ffffff");
        target.GetComponent<Fighter>().Hp -= damage;
        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }

}
