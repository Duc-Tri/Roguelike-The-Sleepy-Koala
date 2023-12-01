using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private bool blocksMovement = false;

    public bool BlocksMovement { get => blocksMovement; set => blocksMovement = value; }

    public void AddToGameMAnager()
    {
        GameManager.instance.AddEntity(this);
    }

    public void Move(Vector2 direction)
    {
        transform.position += (Vector3)direction;
    }

}
