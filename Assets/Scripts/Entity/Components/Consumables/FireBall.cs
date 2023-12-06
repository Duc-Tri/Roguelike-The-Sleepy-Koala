using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Consumable
{
    [SerializeField] private int damage = 12;
    [SerializeField] private int radius = 3;
    public int Damage => damage;
    public int Radius => radius;

    public override bool Activate(Actor consumer)
    {
        consumer.GetComponent<Inventory>().SelectedConsumable = this;
        consumer.GetComponent<Player>().ToggleTargetMode(this, radius);
        UIManager.instance.AddMessage($"Select a location to throw a firewall", "#63ffff");
        return false;
    }

    public override bool Cast(Actor consumer, List<Actor> targets)
    {
        //Debug.Log($"FireBall Cast ■ {targets.Count}");

        foreach (Actor target in targets)
        {
            UIManager.instance.AddMessage($"The {target.name} is engulfed in a fiery explosion, taking {damage} damage", "#ff0000");
            target.GetComponent<Fighter>().Hp -= damage;
        }

        Consume(consumer);
        consumer.GetComponent<Player>().ToggleTargetMode();
        return true;
    }

}
