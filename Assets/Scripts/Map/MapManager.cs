using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("Map Settings")]
    [SerializeField] private int width = 80;
    [SerializeField] private int height = 45;

    [SerializeField] private int roomMinSize = 6;
    [SerializeField] private int roomMaxSize = 10;

    [SerializeField] private int maxRooms = 30;
    [SerializeField] private int maxMonstersPerRoom = 2;
    [SerializeField] private int maxItemsPerRoom = 2;

    [Header("Tiles")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase fogTile;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap obstacleMap;
    [SerializeField] private Tilemap fogMap;

    [Header("Features")]
    [SerializeField] private List<RectangularRoom> rooms = new List<RectangularRoom>();
    [SerializeField] private List<Vector3Int> visibleTiles = new List<Vector3Int>();
    [SerializeField] private Dictionary<Vector3Int, TileData> tiles = new Dictionary<Vector3Int, TileData>();


    internal Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();
    private int numEntities = 0;

    public int Width { get => width; }
    public int Height { get => height; }
    public TileBase FloorTile { get => floorTile; }
    public TileBase WallTile { get => wallTile; }
    public TileBase FogTile { get => fogTile; }
    public Tilemap FloorMap { get => floorMap; }
    public Tilemap ObstacleMap { get => obstacleMap; }
    public Tilemap FogMap { get => fogMap; }

    public List<RectangularRoom> Rooms { get => rooms; }
    public Dictionary<Vector2Int, Node> Nodes { get => nodes; set => nodes = value; }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ProcGen procGen = new ProcGen();
        procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, maxMonstersPerRoom, maxItemsPerRoom, rooms);

        AddTileMapToDictionary(floorMap);
        AddTileMapToDictionary(obstacleMap);

        SetupFogMap();

        Camera.main.transform.position = new Vector3(40, 20.25f, -10);
        //Camera.main.orthographicSize = 22; // 27;
    }

    public bool InBounds(int x, int y) =>
        0 <= x && x < width &&
        0 <= y && y <= height;

    internal void CreateEntity(string name, Vector2Int pos)
    {
        Instantiate(Resources.Load<GameObject>(name), new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), Quaternion.identity).name = name.ToUpper() + numEntities++;

        Debug.Log($"CreateEntity {name} at {pos}.");
    }

    public void UpdateFogMap(List<Vector3Int> playerFOV)
    {
        foreach (Vector3Int pos in visibleTiles)
        {
            if (!tiles[pos].IsExplored)
            {
                tiles[pos].IsExplored = true;
            }
            tiles[pos].IsVisible = false;
            fogMap.SetColor(pos, new Color(1, 1, 1, 1));
        }

        visibleTiles.Clear();

        foreach (Vector3Int pos in playerFOV)
        {
            tiles[pos].IsVisible = true;
            fogMap.SetColor(pos, Color.clear);
            visibleTiles.Add(pos);
        }
    }

    public void SetEntitiesVisibilities()
    {
        foreach (Entity entity in GameManager.instance.Entitites)
        {
            if (entity.GetComponent<Player>())
            {
                continue;
            }

            Vector3Int entityPos = floorMap.WorldToCell(entity.transform.position);

            entity.GetComponent<SpriteRenderer>().enabled = visibleTiles.Contains(entityPos);
        }
    }

    private void AddTileMapToDictionary(Tilemap tilemap)
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos))
            {
                continue;
            }

            TileData tile = new TileData();
            tiles.Add(pos, tile);
        }
    }

    private void SetupFogMap()
    {
        foreach (Vector3Int pos in tiles.Keys)
        {
            fogMap.SetTile(pos, fogTile);
            fogMap.SetTileFlags(pos, TileFlags.None);
        }
    }

    public bool IsValidPosition(Vector3 futurePosition)
    {
        Vector3Int gridPosition = floorMap.WorldToCell(futurePosition);

        if (!InBounds(gridPosition.x, gridPosition.y) || obstacleMap.HasTile(gridPosition))
            return false;

        return true;
    }

}
