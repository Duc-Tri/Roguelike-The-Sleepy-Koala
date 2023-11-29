using System;
using UnityEngine;

static public class Action
{
    static public void EscapeAction()
    {
        Debug.Log("Quit");
    }

    static public bool BumpAction(Actor actor, Vector2 direction)
    {
        Entity target = GameManager.instance.GetBlockingActorAtLocation(actor.transform.position + (Vector3)direction);

        if (target)
        {
            MeleeAction(actor, target);
            return false;
        }
        else
        {
            MovementAction(actor, direction);
            return true;
        }
    }

    static public void MeleeAction(Actor actor, Entity target)
    {
        int damage = actor.GetComponent<Fighter>().Power - target.GetComponent<Fighter>().Defense;
        string attackDesc = $"{actor.name} attacks {target.name}";

        if (damage > 0)
        {
            Debug.Log($"{attackDesc} for {damage} hit points.");
            target.GetComponent<Fighter>().Hp -= damage;
        }
        else
        {
            Debug.Log($"{attackDesc} but does no damge.");
        }

        GameManager.instance.EndTurn();
    }

    static public void MovementAction(Actor actor, Vector2 direction)
    {
        //Debug.Log($"{actor.name} => {direction}");
        actor.Move(direction);
        actor.UpdateFieldOfView();
        GameManager.instance.EndTurn();
    }

    static public void SkipAction()
    {
        GameManager.instance.EndTurn();
    }
}