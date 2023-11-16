using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ProcGen
{
    public void GenerateDungeon(int mapWidth, int mapHeight, int roomMinSize, int roomMaxSize, int maxRooms, List<RectangularRoom> rooms)
    {
        RectangularRoom oldRoom = null;

        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(roomMinSize, roomMaxSize);
            int roomHeight = Random.Range(roomMinSize, roomMaxSize);
            int roomX = Random.Range(0, mapWidth - roomWidth - 1);
            int roomY = Random.Range(0, mapHeight - roomHeight - 1);

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);

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
                        if (SetWallTileIfEmpty(new Vector3Int(x, y, 0)))
                            continue;
                    }
                    else
                    {
                        if (MapManager.instance.ObstacleMap.GetTile(new Vector3Int(x, y, 0)))
                        {
                            MapManager.instance.ObstacleMap.SetTile(new Vector3Int(x, y, 0), null);
                        }
                        MapManager.instance.FloorMap.SetTile(new Vector3Int(x, y, 0), MapManager.instance.FloorTile);
                    }
                }
            }

            if (MapManager.instance.Rooms.Count > 0)
            {
                // dig out a tunnel between this room and the previous one
                TunnelBetWeen(oldRoom, newRoom);
            }

            rooms.Add(newRoom);
            oldRoom = newRoom;
        }

        // set player starts in first room
        MapManager.instance.CreatePlayer(rooms[0].Center());

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
        BresenhamLine(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine(tunnelCorner, newRoomCenter, tunnelCoords);

        // set the tiles for this tunnel
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            Vector3Int pos = new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y, 0);
            if (MapManager.instance.ObstacleMap.HasTile(pos))
            {
                MapManager.instance.ObstacleMap.SetTile(pos, null);
            }

            // set the floor tile
            MapManager.instance.FloorMap.SetTile(pos, MapManager.instance.FloorTile);

            // set the wall tiles around this tile to be walls
            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (SetWallTileIfEmpty(new Vector3Int(x, y, 0)))
                        continue;
                }
            }
        }
    }

    private void BresenhamLine(Vector2Int roomCenter, Vector2Int tunnelCorner, List<Vector2Int> tunnelCoords)
    {
        int x = roomCenter.x,
            y = roomCenter.y;
        int dx = Mathf.Abs(tunnelCorner.x - roomCenter.x),
            dy = Mathf.Abs(tunnelCorner.y - roomCenter.y);
        int sx = roomCenter.x < tunnelCorner.x ? 1 : -1,
            sy = roomCenter.y < tunnelCorner.y ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            tunnelCoords.Add(new Vector2Int(x, y));
            if (x == tunnelCorner.x && y == tunnelCorner.y)
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }

            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }
    }

    private bool SetWallTileIfEmpty(Vector3Int pos)
    {
        if (MapManager.instance.FloorMap.GetTile(new Vector3Int(pos.x, pos.y, 0)))
        {
            return true;
        }
        else
        {
            MapManager.instance.ObstacleMap.SetTile(new Vector3Int(pos.x, pos.y, 0), MapManager.instance.WallTile);
            return false;
        }
    }
}
