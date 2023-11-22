using System;
using UnityEngine;

[Serializable]
public class TileData
{
    [SerializeField] bool isExplored, isVisible;

    public bool IsExplored { get => isExplored; set => isExplored = value; }
    public bool IsVisible { get => isVisible; set => isVisible = value; }
}
