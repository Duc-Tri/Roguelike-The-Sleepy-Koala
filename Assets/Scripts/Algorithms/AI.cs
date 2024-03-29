﻿using UnityEngine;

[RequireComponent(typeof(Actor), typeof(AStar))]
public class AI : MonoBehaviour
{
    [SerializeField] private AStar aStar;
    public AStar AStar { get => aStar; set => aStar = value; }

    private void OnValidate() => aStar = GetComponent<AStar>();

    public virtual void RunAI()
    {

    }

    public void MoveAlongPath(Vector3Int targetPos)
    {
        Vector3Int gridPos = MapManager.instance.FloorMap.WorldToCell(transform.position);
        Vector2 direction = aStar.Compute((Vector2Int)gridPos, (Vector2Int)targetPos);

        Action.MovementAction(GetComponent<Actor>(), direction);
    }

}