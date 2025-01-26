using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

public class TileGeneration : MonoBehaviour
{
    [SerializeField] private Tilemap map;
    [SerializeField] private int gridSize;
    private bool generating;

    [SerializeField] private Vector2Int[] directions = new Vector2Int[4];
    [SerializeField] private RuleTile ruleTile;

    //Spikes
    [SerializeField] private BoolGroup[] spikeChecks;
    [SerializeField] private float spikePct;
    [SerializeField] private Tilemap spikeMap;
    [SerializeField] private Tilemap testMap;
    [SerializeField] private Tile spikeTile;
    private bool generatingSpikes;
    private int spikesGenerated;

    //Enemies
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int minEnemies;
    [SerializeField] private int maxEnemies;
    private List<Vector3Int> emptySpaces = new List<Vector3Int>();

    [SerializeField] private GameObject fader;
    
    public float levelNum;

    [SerializeField] private Transform player;
    private bool playerSet;
    [SerializeField] private Transform enemies;



    private void Awake()
    {
        NewGame();
    }

    private void Update()
    {
        if (enemies.childCount == 0 && !generating)
            StartCoroutine(Generate());
    }

    public void NewGame()
    {
        levelNum = 0;
        //score = 0;
        StartCoroutine(Generate());
    }

    public void Gen()
    {
        StartCoroutine(Generate());
    }

    public IEnumerator Generate()
    {
        generating = true;
        for (float i = 0; i < 1; i += 0.01f)
        {
            fader.GetComponent<CanvasGroup>().alpha = i;
            yield return new WaitForSeconds(0.01f);
        }
        Random.InitState(System.Environment.TickCount);
        map.ClearAllTiles();
        spikeMap.ClearAllTiles();
        testMap.ClearAllTiles();
        emptySpaces.Clear();
        foreach (Transform child in enemies)
            Destroy(child.gameObject);
        playerSet = false;

        map.FloodFill(new Vector3Int(gridSize*2, gridSize*2, 0), ruleTile);
        Vector2Int currentPos = new Vector2Int(gridSize, gridSize);
        for(int i = 0; i < 120; i++)
        {
            Vector2Int multiplier = directions[Random.Range(0, directions.Length)];
            if (currentPos.x + multiplier.x > gridSize*2-2 || currentPos.x + multiplier.x < 2 || currentPos.y + multiplier.y > gridSize*2-2 || currentPos.y + multiplier.y < 2)
            {
                multiplier *= -1;
            }
            for(int j = 1; j <= 4; j++)
            {
                for(int k = 1; k <= 4; k++) 
                {
                    Vector3Int loc = new Vector3Int(currentPos.x+multiplier.x+j, currentPos.y+multiplier.y+k, 0);
                    map.SetTile(loc, null);
                    emptySpaces.Add(loc);
                }
            }
            currentPos += multiplier*3;
        }

        //Destroy overhanging pieces
        for(int x = 0; x <= gridSize*2 -3; x++)
        {
            for(int y = 0; y <= gridSize*2 -3; y++)
            {
                if (NumNeighbors(new Vector3Int(x, y, 0), map) < 2 && map.HasTile(new Vector3Int(x, y, 0)))
                {
                    //Debug.Log("Destroying tile at (" + x + ", " + y + ")!");
                    map.SetTile(new Vector3Int(x, y, 0), null);
                    x = 0;
                    y = 0;
                }
                if (!playerSet)
                {
                    bool validSpawn = true;
                    for (int dx = -2; dx <= 2; dx++)
                    {
                        for (int dy = -2; dy <= 2; dy++)
                        {
                            if (map.HasTile(new Vector3Int(x + dx, y + dy, 0)) || spikeMap.HasTile(new Vector3Int(x + dx, y + dy, 0)))
                            {
                                validSpawn = false;
                                break;
                            }
                        }
                    }
                    if (validSpawn)
                    {
                        playerSet = true;
                        player.position = new Vector3(x, y, 0);
                        RemoveEmpty(new Vector3Int(x, y, 0));
                    }
                }
            }
        }

        //Create border
        for (int x = -10; x < gridSize*2+10; x++)
        {
            for (int y = -10; y < gridSize*2+10; y++)
            {
                if (x < 0 || x > gridSize*2 || y < 0 || y > gridSize*2)
                    map.SetTile(new Vector3Int(x, y, 0), ruleTile);
            }
        }

        //Generate spikes
        for(int x = 0; x <= gridSize*2; x++)
        {
            for(int y = 0; y <= gridSize*2; y++)
            {
                OpenSpot(new Vector3Int(x, y, 0));
            }
        }

        //Spawn enemies
        int offset = 0;
        if (emptySpaces.Count > 600)
            offset = 2;
        else if (emptySpaces.Count > 400)
            offset = 1;
        int numEnemies = Random.Range(minEnemies, maxEnemies-1 + offset);
        int enemiesSpawned = 0;
        int timesAttempted = 0;
        while (enemiesSpawned < numEnemies && timesAttempted < 30)
        {
            Vector3Int randomTile = emptySpaces[Random.Range(0, emptySpaces.Count)];
            timesAttempted++;
            if (NumNeighbors(randomTile, map) == 0 && NumNeighbors(randomTile, spikeMap) == 0 && randomTile.x > 0 && randomTile.y > 0 && randomTile.x < gridSize*2 && randomTile.y < gridSize*2)
            {
                GameObject enemy = Instantiate(enemyPrefab, randomTile, Quaternion.identity, enemies);
                enemy.GetComponent<EnemyMovement>().mode = "IDLE";
                enemiesSpawned++;
                RemoveEmpty(randomTile);
            }
        }

        for (float i = 1; i >= 0; i -= 0.01f)
        {
            fader.GetComponent<CanvasGroup>().alpha = i;
            yield return new WaitForSeconds(0.01f);
        }

        foreach (Transform child in enemies)
            child.GetComponent<EnemyMovement>().mode = "MOVE";

        generating = false;
    }

    private void RemoveEmpty(Vector3Int pos)
    {
        HashSet<Vector3Int> emptySpacesHash = new HashSet<Vector3Int>(emptySpaces);
        for (int dx = -4; dx <= 4; dx++)
        {
            for (int dy = -4; dy <= 4; dy++)
            {
                emptySpacesHash.Remove(pos + new Vector3Int(dx, dy, 0));
            }
        }
        emptySpaces = new List<Vector3Int>(emptySpacesHash);
    }


    private int NumNeighbors(Vector3Int currentPos, Tilemap tilemap)
    {
        int neighbors = 0;
        /*foreach (Vector2Int dir in directions)
        {
            if (tilemap.HasTile(new Vector3Int(currentPos.x + dir.x, currentPos.y + dir.y, 0)))
                neighbors++;
        }*/
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (map.HasTile(new Vector3Int(currentPos.x + x, currentPos.y + y, 0)) && !(x == 0 && y == 0))
                    neighbors++;
            }
        }
        return neighbors;
    }


    private void OpenSpot(Vector3Int v3)
    {
        //3x2 bool groups, check if have space for 3 spikes, and then spawn all 3 at once        
        CheckArea(new bool[]{false, false, false, true, true, true}, new Vector3Int(0, 2, 0), 3, v3);
        CheckArea(new bool[]{true, true, true, false, false, false}, new Vector3Int(0, -2, 0), 3, v3);
        CheckArea(new bool[]{true, false, true, false, true, false}, new Vector3Int(2, 0, 0), 2, v3);
        CheckArea(new bool[]{false, true, false, true, false, true}, new Vector3Int(-2, 0, 0), 2, v3);
    }

    private void CheckArea(bool[] pattern, Vector3Int direction, int cols, Vector3Int v3, int fillDir = 0)
    {
        bool match = true;
        for (int i = 0; i < 6; i++)
        {
            Vector3Int loc = new Vector3Int(v3.x + i%cols, v3.y + i/cols, 0);
            if (map.HasTile(loc) != pattern[i] || spikeMap.HasTile(loc + direction))
            {
                if (spikeMap.HasTile(loc + direction))
                {
                    testMap.SetTile(loc, ruleTile);
                }
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
                    CheckArea(pattern, direction, cols, v3 + offset, 1);
                if (fillDir == 0 || fillDir == -1)
                    CheckArea(pattern, direction, cols, v3 + offset*-1, 1);
            }
        }
    }

    public void GenerateSpike(Vector3Int v3)
    {
        //pass direction and spawn spike in correct orientation
        spikeMap.SetTile(v3, spikeTile);
    }

}
