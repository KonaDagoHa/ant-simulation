using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    // Prefabs
    public GameObject tilePrefab, nestPrefab, antPrefab;

    // Tiles
    private Tile[,] tiles;
    public int maxRows, maxColumns;
    // tileSize, gridWidth, gridHeight are only here to make math cleaner; they are redundant due to maxRows and maxColumns
    private int tileSize = 1; // Keep this at 1 for simplicity
    private float gridWidth, gridHeight;

    // Nest
    private Nest nest;

    // Ants
    private Ant[] ants;
    public int antPopulation;

    // Getters
    public float GetWidth() { return gridWidth; }
    public float GetHeight() { return gridHeight; }
    public int GetTileSize() { return tileSize; }

    private void Awake()
    {
        gridWidth = maxColumns * tileSize;
        gridHeight = maxRows * tileSize;

        tiles = new Tile[maxColumns, maxRows];
        ants = new Ant[antPopulation];

        CreateTiles();
        CreateNest();
        CreateAnts();
    }

    // Create a grid of tiles
    private void CreateTiles()
    {
        for (int x = 0; x < maxColumns; x++)
        {
            for (int y = 0; y < maxRows; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, transform);
                newTile.transform.position = new Vector2(x, y);
                tiles[x, y] = newTile.GetComponent<Tile>();
            }
        }
    }

    // Create ant nest
    private void CreateNest()
    {
        GameObject newNest = Instantiate(nestPrefab, transform);
        newNest.transform.position = new Vector3(Random.Range(1, maxColumns - 1), Random.Range(1, maxRows - 1));
        nest = newNest.GetComponent<Nest>();

    }

    // Create and populate ants
    private void CreateAnts()
    {
        for (int i = 0; i < antPopulation; i++)
        {
            GameObject newAnt = Instantiate(antPrefab, transform);
            newAnt.transform.position = nest.transform.position; // Ants all start at nest
            ants[i] = newAnt.GetComponent<Ant>();
        }
    }

    // Returns reference to tile given position coordinates
    public Tile GetTile(Vector2 pos)
    {
        // pos can have floats for coordinates (useful for ants)
        return tiles[(int) pos.x, (int) pos.y];
    }
}
