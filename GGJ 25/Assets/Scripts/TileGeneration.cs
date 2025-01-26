using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGeneration : MonoBehaviour
{
    [SerializeField]
    private Tilemap map;

    [SerializeField]
    private Tilemap spikeMap;
    [SerializeField]
    private Tile spikeTile;
    private bool generatingSpikes;
    private int spikesGenerated;

    [SerializeField]
    private Vector2Int[] directions = new Vector2Int[4];
    [SerializeField]
    private RuleTile ruleTile;

    [SerializeField]
    private int gridSize;

    //Spikes
    [SerializeField] private BoolGroup[] spikeChecks;
    [SerializeField] private float spikePct;

    [SerializeField] private Transform player;



    void Awake()
    {
        Generate();
    }

    public void Generate()
    {
        Random.InitState(System.Environment.TickCount);
        map.ClearAllTiles();
        spikeMap.ClearAllTiles();
        map.FloodFill(new Vector3Int(gridSize*2, gridSize*2, 0), ruleTile);
        Vector2Int currentPos = new Vector2Int(gridSize, gridSize);
        for(int i = 0; i < 80; i++)
        {
            Vector2Int multiplier = directions[Random.Range(0, directions.Length)];
            if (currentPos.x + multiplier.x > 78 || currentPos.x + multiplier.x < 2 || currentPos.y + multiplier.y > 78 || currentPos.y + multiplier.y < 2)
            {
                multiplier *= -1;
            }
            for(int j = 1; j <= 4; j++)
            {
                for(int k = 1; k <= 4; k++) 
                {
                    map.SetTile(new Vector3Int(currentPos.x+multiplier.x+j, currentPos.y+multiplier.y+k, 0), null);
                }
            }
            currentPos += multiplier*3;
        }

        for(int x = 0; x <= gridSize*2 -3; x++)
        {
            for(int y = 0; y <= gridSize*2 -3; y++)
            {
                if (NumNeighbors(new Vector3Int(x, y, 0)) < 2 && map.HasTile(new Vector3Int(x, y, 0)))
                {
                    Debug.Log("Destroying tile at (" + x + ", " + y + ")!");
                    map.SetTile(new Vector3Int(x, y, 0), null);
                    x = 0;
                    y = 0;
                }
                else if (NumNeighbors(new Vector3Int(x, y, 0)) == 0)
                {
                    player.position = new Vector3(x, y, 0);
                }
            }
        }

        for (int x = -10; x < gridSize*2+10; x++)
        {
            for (int y = -10; y < gridSize*2+10; y++)
            {
                if (x < 0 || x > gridSize*2 || y < 0 || y > gridSize*2)
                    map.SetTile(new Vector3Int(x, y, 0), ruleTile);
            }
        }

        //get rid of 2x2 squares?

        for(int x = 0; x <= gridSize*2; x++)
        {
            for(int y = 0; y <= gridSize*2; y++)
            {
                OpenSpot(new Vector3Int(x, y, 0));
            }
        }
        //generate door
    }


    private int NumNeighbors(Vector3Int currentPos)
    {
        int neighbors = 0;
        foreach (Vector2Int dir in directions)
        {
            if (map.HasTile(new Vector3Int(currentPos.x + dir.x, currentPos.y + dir.y, 0)))
                neighbors++;
        }
        /*for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (map.HasTile(new Vector3Int(currentPos.x + x, currentPos.y + y, 0)) && !(x == 0 && y == 0))
                    neighbors++;
            }
        }*/
        return neighbors;
    }


    private void OpenSpot(Vector3Int v3)
    {
        //3x2 bool groups, check if have space for 3 spikes, and then spawn all 3 at once        
        bool[][] patterns = new bool[4][];
        CheckArea(new bool[]{false, false, false, true, true, true}, 3, v3);
        CheckArea(new bool[]{true, true, true, false, false, false}, 3, v3);
        CheckArea(new bool[]{true, false, true, false, true, false}, 2, v3);
        CheckArea(new bool[]{false, true, false, true, false, true}, 2, v3);
    }

    private void CheckArea(bool[] pattern, int cols, Vector3Int v3, int fillDir = 0)
    {
        bool match = true;
        for (int i = 0; i < 6; i++)
        {
            if (map.HasTile(new Vector3Int(v3.x + i%cols, v3.y + i/cols, 0)) != pattern[i])
            {
                match = false;
                break;
            }
        }
        if (match)
        {
            if (Random.Range(0f, 1f) < spikePct || fillDir != 0)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (!pattern[i])
                    {
                        GenerateSpike(new Vector3Int(v3.x + i%cols, v3.y + i/cols));
                    }
                }
                Vector3Int offset = (cols == 3) ? new Vector3Int(1, 0, 0) : new Vector3Int(0, 1, 0);
                if (fillDir == 0 || fillDir == 1)
                    CheckArea(pattern, cols, v3 + offset, 1);
                if (fillDir == 0 || fillDir == -1)
                    CheckArea(pattern, cols, v3 + offset*-1, 1);
            }
        }
    }

    public void GenerateSpike(Vector3Int v3)
    {
        //pass direction and spawn spike in correct orientation
        spikeMap.SetTile(v3, spikeTile);
    }

}
