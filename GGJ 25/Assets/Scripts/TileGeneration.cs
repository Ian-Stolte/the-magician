using System.Collections;
using System.Collections.Generic;
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
    //private TileBase base;
    void Awake()
    {
        TileClear();
        Generate();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(RandomDir());
    }

    public void Generate()
    {
        TileClear();
        Vector2Int currentPos = new Vector2Int(10, 10);
        for (int i = 0; i < 80; i++)
        {
            currentPos += RandomDir();
            Vector3Int tempV3 = new Vector3Int(currentPos.x, currentPos.y, 0);
            //bigger by having each set tile as for the nearest 3
            
            int x = Random.Range(2, 4);
            int y = Random.Range(2, 4);
            for(int j = x; j >= 0; j--)
            {
                for(int k = y; k >= 0; k--)
                map.SetTile(new Vector3Int(currentPos.x+j, currentPos.y+k, 0), null);
            }
            //currentPos = new Vector2Int(currentPos.x + x, currentPos.y + y);
            
        }
        //contains tile func
    }


    public Vector2Int RandomDir()
    {
        return directions[Random.Range(0, directions.Length)];
    }

    private void TileClear()
    {
        map.ClearAllTiles();
        map.FloodFill(new Vector3Int(20, 20, 0), ruleTile);
        
    }
}
