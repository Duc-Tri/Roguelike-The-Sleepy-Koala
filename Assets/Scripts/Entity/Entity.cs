using System;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField] private bool isSentient = false;
    [SerializeField] private bool blocksMovement = false;

    [SerializeField] private int fieldOfViewRange = 8;
    [SerializeField] private List<Vector3Int> fieldOfView;

    AdamMilVisibility algorithm;

    public bool IsSentient { get => isSentient; }
    public bool BlocksMovement { get => blocksMovement; }

    void Start()
    {
        if (isSentient)
        {
            if (GetComponent<Player>())
                GameManager.instance.InsertEntity(this, 0);
            else if (IsSentient)
                GameManager.instance.AddEntity(this);
        }

        fieldOfView = new List<Vector3Int>();
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();
    }

    public void UpdateFieldOfView()
    {
        Vector3Int gridpos = MapManager.instance.FloorMap.WorldToCell(transform.position);

        fieldOfView.Clear();
        algorithm.Compute(gridpos, fieldOfViewRange, fieldOfView);

        if (GetComponent<Player>())
        {
            MapManager.instance.UpdateFogMap(fieldOfView);
            MapManager.instance.SetEntitiesVisibilities();
        }
    }

    public void Move(Vector2 direction)
    { 
        transform.position += (Vector3)direction;
    }

}
