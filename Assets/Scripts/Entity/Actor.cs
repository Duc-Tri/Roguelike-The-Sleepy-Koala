using System.Collections.Generic;
using UnityEngine;

public class Actor : Entity
{
    [SerializeField] private bool isAlive = true;

    [SerializeField] private int fieldOfViewRange = 8;

    [SerializeField] private List<Vector3Int> fieldOfView;

    [SerializeField] private AI aI;

    [SerializeField] private Inventory inventory;

    AdamMilVisibility algorithm;

    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public List<Vector3Int> FieldOfView { get => fieldOfView; set => fieldOfView = value; }
    public Inventory Inventory { get => inventory; }

    private void OnValidate()
    {
        if (GetComponent<AI>())
            aI = GetComponent<AI>();

        if (GetComponent<Inventory>())
            inventory = GetComponent<Inventory>();
    }

    void Start()
    {
        AddToGameMAnager();
        if (GetComponent<Player>())
            GameManager.instance.InsertActor(this, 0);
        else if (IsAlive)
            GameManager.instance.AddActor(this);

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

}