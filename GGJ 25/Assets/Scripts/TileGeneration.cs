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

    [SerializeField]
    private BoolGroup[] spikeChecks;
    
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

        /*for(int x = 0; x <= gridSize; x++)
        {
            for(int y = 0; y <= gridSize; y++)
            {
                Vector3Int tempV3 = new Vector3Int(x, y, 0);
                OpenSpot(tempV3);
            }
        }
        EntranceGenerate();
        ExitGenerate();*/
    }

    private int NumNeighbors(Vector3Int currentPos)
    {
        int neighbors = 0;
        foreach (Vector2Int dir in directions)
        {
            if (map.HasTile(new Vector3Int(currentPos.x + dir.x, currentPos.y + dir.y, 0)))
                neighbors++;
        }
        /*
        for (int x = -1; x <= 1; x++)
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
        //3x3 bool groups, check if have space for 3 spikes, and then spawn all 3 at once
        bool[] tiles = new bool[9];
        tiles[0] = map.HasTile(v3);
        tiles[1] = map.HasTile(new Vector3Int(v3.x+1, v3.y, 0));
        tiles[2] = map.HasTile(new Vector3Int(v3.x+2, v3.y, 0));
        tiles[3] = map.HasTile(new Vector3Int(v3.x, v3.y+1, 0));
        tiles[4] = map.HasTile(new Vector3Int(v3.x+1, v3.y+1, 0));
        tiles[5] = map.HasTile(new Vector3Int(v3.x+2, v3.y+1, 0));
        tiles[6] = map.HasTile(new Vector3Int(v3.x, v3.y+2, 0));
        tiles[7] = map.HasTile(new Vector3Int(v3.x+1, v3.y+2, 0));
        tiles[8] = map.HasTile(new Vector3Int(v3.x+2, v3.y+2, 0));
        
        foreach(BoolGroup boolGroup in spikeChecks)
        {
            if(boolGroup.CompareGroup(tiles))
            {
                GenerateSpike(new Vector3Int(v3.x + boolGroup.spawnPlace.x, v3.y + boolGroup.spawnPlace.y, 0));
            }
        }
    }

    private void EntranceGenerate()
    {
        bool spawned = false;
        //check for all open spot tiles and replace? then delete all unreplaced open spot tiles?
        //check for spike tile and replace
        //either way, replace any spike tiles within 3 radius

    }
    private void ExitGenerate()
    {
        bool spawned = false;

    }

    public void GenerateSpike(Vector3Int v3)
    {
        int temp = Random.Range(0, 9);
        if(temp > 7)
        {
            spikeMap.SetTile(v3, spikeTile);
            generatingSpikes = true;
        }
        else if(generatingSpikes && spikesGenerated < 3)
        {
            spikeMap.SetTile(v3, spikeTile);
            spikesGenerated++;
        }
        else
        {
            generatingSpikes = false;
            spikesGenerated = 0;
        }
    }

}
