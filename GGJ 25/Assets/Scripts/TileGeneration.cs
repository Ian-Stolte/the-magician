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
    private int tileSize;

    [SerializeField]
    private Vector2Int[] directions = new Vector2Int[4];
    [SerializeField]
    private RuleTile ruleTile;

    [SerializeField]
    private TileBase[] spikeCheck;

    [SerializeField]
    private Tile spikeTile;
    void Awake()
    {
        Generate();
    }

    public void Generate()
    {
        map.ClearAllTiles();
        map.FloodFill(new Vector3Int(20, 20, 0), ruleTile);
        Vector2Int currentPos = new Vector2Int(10, 10);
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

        for(int i = -2; i <= 22; i++)
        {
            foreach(Vector2Int dir in directions)
            {
                map.SetTile(new Vector3Int(i*dir.x, i*dir.y, 0), ruleTile);
                //creates a plus thing because the walls are all starting from the same origin. 2 of the walls need to start from the opposite corner
            }
        }
        for(int x = 0; x <= 20; x++)
        {
            for(int y = 0; y <= 20; y++)
            {
                if(SpikeSpot(new Vector3Int(x, y, 0)));
                    {
                        GenerateSpike(new Vector3Int(x, y, 0));
                    }

            }
        }



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


    private bool SpikeSpot(Vector3Int v3)
    {
        bool check = false;
        bool[] tiles = new bool[4];
        for(int i = 0; i > 2; i++)
        {
            Vector3Int tempv3 = new Vector3Int(v3.x+i, v3.y, 0);
            tiles[i] = map.HasTile(tempv3);
        }
        for(int i = 0; i > 2; i++)
        {
            Vector3Int tempv3 = new Vector3Int(v3.x+1, v3.y+i, 0);
            tiles[i] = map.HasTile(tempv3);
        }
        //if tiles matches a possible shape of bool[] that means we can spawn a spike, then check is true
        return check;
    }
    public Vector2Int RandomDir()
    {
        return directions[Random.Range(0, directions.Length)];
    }

    public void GenerateSpike(Vector3Int v3)
    {
        map.SetTile(v3, spikeTile);
        Debug.Log("SPIKE");
    }

}
