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
    private int tileSize;

    [SerializeField]
    private Vector2Int[] directions = new Vector2Int[4];
    [SerializeField]
    private RuleTile ruleTile;



    [SerializeField]
    private BoolGroup[] spikeChecks;
    
    void Awake()
    {
        Generate();
    }

    public void Generate()
    {
        map.ClearAllTiles();
        spikeMap.ClearAllTiles();
        map.FloodFill(new Vector3Int(30, 30, 0), ruleTile);
        Vector2Int currentPos = new Vector2Int(15, 15);
        for(int i = 0; i < 80; i++)
        {

            //bigger by having each set tile as for the nearest 3
            Vector2Int multiplier = RandomDir();
            for(int j = 1; j <= 3; j++)
            {
                for(int k = 1; k <= 3; k++) 
                {
                    map.SetTile(new Vector3Int(currentPos.x+multiplier.x+j, currentPos.y+multiplier.y+k, 0), null);
                }
            }
            currentPos += multiplier*3;
            
        }

        // for(int i = -2; i <= 22; i++)
        // {
        //     foreach(Vector2Int dir in directions)
        //     {
        //         map.SetTile(new Vector3Int(i*dir.x, i*dir.y, 0), ruleTile);
        //         //creates a plus thing because the walls are all starting from the same origin. 2 of the walls need to start from the opposite corner
        //     }
        // }

        for(int x = 0; x <= 20; x++)
        {
            for(int y = 0; y <= 20; y++)
            {
                Vector3Int tempV3 = new Vector3Int(x, y, 0);
                OpenSpot(tempV3);
            }
        }
        EntranceGenerate();
        ExitGenerate();



        // for (int i = 0; i < 80; i++)
        // {
        //     currentPos += RandomDir();
        //     Vector3Int tempV3 = new Vector3Int(currentPos.x, currentPos.y, 0);
        //     //bigger by having each set tile as for the nearest 3
            
        //     for(int j = 0; j <= 2; j++)
        //     {
        //         for(int k = 0; k <= 2; k++)
        //         {
        //             map.SetTile(new Vector3Int(currentPos.x+j, currentPos.y+k, 0), null);
        //         }

        //     }
        //     currentPos = new Vector2Int(currentPos.x, currentPos.y);
            
        // }
        //contains tile func
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
    public Vector2Int RandomDir()
    {
        return directions[Random.Range(0, directions.Length)];
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
