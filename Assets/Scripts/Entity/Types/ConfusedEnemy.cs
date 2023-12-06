using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A confused enemy will stumble around aimlessly for a given number of turns, then revert back to its previous AI.
// If an actor occupues a tile it is randomly moving into, it will attack.
[RequireComponent(typeof(Actor))]
public class ConfusedEnemy : AI
{
    [SerializeField] private AI previousAI;
    [SerializeField] private int turnRemaining;
    public AI PreviousAI { get; set; }

    public int TurnRemaining { get; set; }

    public override void RunAI()
    {
        // Revert the AI back to the original state if the effect has run its course
        if (turnRemaining <= 0)
        {
            UIManager.instance.AddMessage($"The {gameObject.name} is no longer confused.", "#ff0000");
            GetComponent<Actor>().AI = previousAI;
            GetComponent<Actor>().AI.RunAI();
            Destroy(this);
        }
        else
        {
            // move randomly
            Vector2Int direction = Random.Range(0, 8) switch
            {
                0 => new Vector2Int(0, 1), // north-west
                1 => new Vector2Int(0, -1),
                2 => new Vector2Int(1, 0),
                3 => new Vector2Int(-1, 0),
                4 => new Vector2Int(1, 1),
                5 => new Vector2Int(1, -1),
                6 => new Vector2Int(-1, 1),
                7 => new Vector2Int(-1, -1),
                _ => new Vector2Int(0, 0)
            };
            // will either try to move or attack in the chosen direction
            // it's possible it ll just bump into the wall, wasting a turn
            Action.BumpAction(GetComponent<Actor>(), direction);
            turnRemaining--;
        }
    }

}
