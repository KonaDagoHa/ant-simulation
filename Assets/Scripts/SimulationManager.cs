using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SimulationManager : MonoBehaviour
{
    // Prefabs
    public GameObject tilePrefab, nestPrefab, foodPrefab, obstaclePrefab, antPrefab, boundaryWallPrefab;

    // Tiles
    private Tile[,] tiles;
    public int maxRows, maxColumns;

    // Ants
    private Ant[] ants;
    public int antPopulation;

    private void Awake()
    {
        tiles = new Tile[maxColumns, maxRows];
        ants = new Ant[antPopulation];

        CreateGrid();
        CreateBoundary();
        CreateNest();
        CreateFood();
        CreateObstacles();
        CreateAnts();
    }

    // Create a grid of tiles
    private void CreateGrid()
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

    // Create an invisible boundary around the tile grid (prevents ants from moving off grid)
    private void CreateBoundary()
    {
        GameObject topWall = Instantiate(boundaryWallPrefab, transform);
        topWall.transform.localScale = new Vector2(maxColumns + 2, 1);
        topWall.transform.position = new Vector2(-1, maxRows);

        GameObject rightWall = Instantiate(boundaryWallPrefab, transform);
        rightWall.transform.localScale = new Vector2(1, maxRows + 2);
        rightWall.transform.position = new Vector2(maxColumns, -1);

        GameObject bottomWall = Instantiate(boundaryWallPrefab, transform);
        bottomWall.transform.localScale = new Vector2(maxColumns + 2, 1);
        bottomWall.transform.position = new Vector2(-1, -1);

        GameObject leftWall = Instantiate(boundaryWallPrefab, transform);
        leftWall.transform.localScale = new Vector2(1, maxRows + 2);
        leftWall.transform.position = new Vector2(-1, -1);
    }

    private void CreateNest()
    {
        Instantiate(nestPrefab, transform);

    }

    private void CreateFood()
    {
        Instantiate(foodPrefab, transform);
    }

    private void CreateObstacles()
    {
        Instantiate(obstaclePrefab, transform);
    }

    private void CreateAnts()
    {
        for (int i = 0; i < antPopulation; i++)
        {
            GameObject newAnt = Instantiate(antPrefab, transform);
            ants[i] = newAnt.GetComponent<Ant>();
        }
    }

    // Returns reference to tile given floating point position coordinates
    public Tile GetTile(Vector2 pos)
    {
        Vector2Int tilePos = GetTilePosition(pos);

        return tiles[tilePos.x, tilePos.y];
    }

    // Returns integer tile position coordinates given floating point position coordinates
    public Vector2Int GetTilePosition(Vector2 pos)
    {
        // "0.5f" offset is used to prevent indexOutOfBounds error in case ants go slightly over boundary
        int x = (int)Mathf.Clamp(pos.x, 0.5f, maxColumns - 0.5f);
        int y = (int)Mathf.Clamp(pos.y, 0.5f, maxRows - 0.5f);

        return new Vector2Int(x, y);
    }

    // *** Static Methods ***

    // Takes in a rotation vector (transform.eulerAngles) and converts it into a 2D unit vector in same direction on xy-plane
        // Only rotVector.z (transform.eulerAngles.z) component matters
    public static Vector2 RotationToOrientation(Vector3 rotVector)
    {
        return RotationZToOrientation(rotVector.z);
    }
    
    // Takes in a orientation vector with (x, y) coordinates and converts to a rotation vector to be assigned to transform.eulerAngles
    public static Vector3 OrientationToRotation(Vector2 oriVector)
    {
        return new Vector3(0, 0, OrientationToRotationZ(oriVector));
    }

    public static Vector2 RotationZToOrientation(float rotZ)
    {
        return new Vector2(Mathf.Cos((rotZ + 90f) * Mathf.Deg2Rad), Mathf.Sin((rotZ + 90f) * Mathf.Deg2Rad));
    }

    // Takes in orientation vector and only returns z component of rotation vector
    public static float OrientationToRotationZ(Vector2 oriVector)
    {
        return Mathf.Atan2(oriVector.y, oriVector.x) * Mathf.Rad2Deg - 90f;
    }
    
}