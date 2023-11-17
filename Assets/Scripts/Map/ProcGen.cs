using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class ProcGen
{
    public void GenerateDungeon(int mapWidth, int mapHeight, int roomMinSize, int roomMaxSize, int maxRooms, int maxMonstersPerRoom, List<RectangularRoom> rooms)
    {
        RectangularRoom newRoom, oldRoom = null;

        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(roomMinSize, roomMaxSize);
            int roomHeight = Random.Range(roomMinSize, roomMaxSize);
            int roomX = Random.Range(0, mapWidth - roomWidth - 1);
            int roomY = Random.Range(0, mapHeight - roomHeight - 1);

            newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);

            if (newRoom.Overlaps(rooms))
            {
                continue;
            }
            // no intersection => valid room

            //dig out this room inner area & build the walls
            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || y == roomY || x == roomX + roomWidth - 1 || y == roomY + roomHeight - 1)
                    {
                        if (SetWallTileIfEmpty(new Vector3Int(x, y)))
                            continue;
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y));
                    }
                }
            }

            if (rooms.Count > 0)
            {
                // dig out a tunnel between this room and the previous one
                TunnelBetWeen(oldRoom, newRoom);
            }

            rooms.Add(newRoom);
            oldRoom = newRoom;
        }

        // set player starts in first room
        MapManager.instance.CreateEntity("Player", rooms[0].Center());

    } // GenerateDungeon

    private void TunnelBetWeen(RectangularRoom oldRoom, RectangularRoom newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (Random.value < 0.5f)
        {
            // move horizontally, then verically
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        }
        else
        {
            // move verically, then horizontally
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);
        }

        // generate the coordinates for this tunnel
        List<Vector2Int> tunnelCoords = new List<Vector2Int>();
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        // set the tiles for this tunnel
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            Vector3Int pos = new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0);
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            // set the floor tile
            MapManager.instance.FloorMap.SetTile(pos, MapManager.instance.FloorTile);

            // set the wall tiles around this tile to be walls
            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (SetWallTileIfEmpty(new Vector3Int(x, y)))
                        continue;
                }
            }
        }
    }

    private bool SetWallTileIfEmpty(Vector3Int pos)
    {
        if (MapManager.instance.FloorMap.GetTile(pos))
        {
            return true;
        }
        else
        {
            MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.WallTile);
            return false;
        }
    }

    void SetFloorTile(Vector3Int pos)
    {
        if (MapManager.instance.ObstacleMap.GetTile(pos))
        {
            MapManager.instance.ObstacleMap.SetTile(pos, null);
        }
        MapManager.instance.FloorMap.SetTile(pos, MapManager.instance.FloorTile);

    }

    private void PlaceEntities(RectangularRoom newRoom, int maxMonsters)
    {
        int numMonsters = Random.Range(0, maxMonsters + 1);

        for (int monster = 0; monster < numMonsters;)
        {
            int x = Random.Range(newRoom.x, newRoom.x + newRoom.width);
            int y = Random.Range(newRoom.y, newRoom.y + newRoom.height);

            if (x == newRoom.x || x == newRoom.x + newRoom.width - 1 || y == newRoom.y || y == newRoom.y + newRoom.height - 1)
            {
                continue; // retry
            }

            for (int entity = 0; entity < GameManager.instance.Entitites.Count; entity++)
            {
                Vector3Int pos = MapManager.instance.FloorMap.WorldToCell(GameManager.instance.Entitites[entity].transform.position);
                if (pos.x == x && pos.y == y)
                {
                    return;
                }
            }

            if (Random.value < 0.8f)
            {
                MapManager.instance.CreateEntity("Orc", new Vector2Int(x, y));
            }
            else
            {
                MapManager.instance.CreateEntity("Troll", new Vector2Int(x, y));
            }

            monster++;
        }

    }

}
