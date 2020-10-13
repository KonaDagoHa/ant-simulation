using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class SimulationManager : MonoBehaviour
{
    // Prefabs
    [SerializeField] private GameObject tilePrefab, nestPrefab, foodPrefab, obstaclePrefab, antPrefab, boundaryWallPrefab;

    // Grid
    private Tile[,] tileGrid;
    [SerializeField] [Range(4, 10)] private int rowCount = 10;
    [SerializeField] [Range(4, 10)] private int columnCount = 10;

    // Ants
    [SerializeField] [Range(0, 500)] private int antCount;

    // Getters
    public int RowCount => rowCount;
    public int ColumnCount => columnCount;
    public int AntCount => antCount;

    private void Awake()
    {
        tileGrid = new Tile[ColumnCount, RowCount];

        InitTiles();
        InitBoundaries();
        InitNests();
        InitFoods();
        InitObstacles();
        InitAnts();
    }

    // Initialize grid with tiles
    private void InitTiles()
    {
        for (int x = 0; x < ColumnCount; x++)
        {
            for (int y = 0; y < RowCount; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, transform);
                newTile.transform.position = new Vector2(x, y);
                tileGrid[x, y] = newTile.GetComponent<Tile>();
            }
        }
    }

    // Create an invisible boundary around the tile grid (prevents ants from moving off grid)
    private void InitBoundaries()
    {
        GameObject topWall = Instantiate(boundaryWallPrefab, transform);
        topWall.transform.localScale = new Vector2(ColumnCount + 2, 1);
        topWall.transform.position = new Vector2(-1, RowCount);

        GameObject rightWall = Instantiate(boundaryWallPrefab, transform);
        rightWall.transform.localScale = new Vector2(1, RowCount + 2);
        rightWall.transform.position = new Vector2(ColumnCount, -1);

        GameObject bottomWall = Instantiate(boundaryWallPrefab, transform);
        bottomWall.transform.localScale = new Vector2(ColumnCount + 2, 1);
        bottomWall.transform.position = new Vector2(-1, -1);

        GameObject leftWall = Instantiate(boundaryWallPrefab, transform);
        leftWall.transform.localScale = new Vector2(1, RowCount + 2);
        leftWall.transform.position = new Vector2(-1, -1);

        // Corner pieces (prevents ants from getting stuck in corner)
        GameObject topLeft = Instantiate(boundaryWallPrefab, transform);
        topLeft.transform.eulerAngles = new Vector3(0, 0, 45);
        topLeft.transform.position = new Vector2(0f, RowCount - 0.05f);

        GameObject topRight = Instantiate(boundaryWallPrefab, transform);
        topRight.transform.eulerAngles = new Vector3(0, 0, -45);
        topRight.transform.position = new Vector2(ColumnCount - 0.05f, RowCount);

        GameObject bottomRight = Instantiate(boundaryWallPrefab, transform);
        bottomRight.transform.eulerAngles = new Vector3(0, 0, -135);
        bottomRight.transform.position = new Vector2(ColumnCount, 0.05f);

        GameObject bottomLeft = Instantiate(boundaryWallPrefab, transform);
        bottomLeft.transform.eulerAngles = new Vector3(0, 0, 135);
        bottomLeft.transform.position = new Vector2(0.05f, 0f);
    }

    private void InitNests()
    {
        Instantiate(nestPrefab, transform);

    }

    private void InitFoods()
    {
        Instantiate(foodPrefab, transform);
    }

    private void InitObstacles()
    {
        Instantiate(obstaclePrefab, transform);
        Instantiate(obstaclePrefab, transform);
        Instantiate(obstaclePrefab, transform);
    }

    private void InitAnts()
    {
        for (int i = 0; i < AntCount; i++)
        {
            GameObject newAnt = Instantiate(antPrefab, transform);
        }
    }

    // Returns reference to tile given floating point position coordinates
    public Tile GetTile(Vector2 pos)
    {
        Vector2Int tilePos = GetTilePosition(pos);

        return tileGrid[tilePos.x, tilePos.y];
    }

    // Returns integer tile position coordinates given floating point position coordinates
    public Vector2Int GetTilePosition(Vector2 pos)
    {
        // "0.5f" offset is used to prevent indexOutOfBounds error in case ants go slightly over boundary
        int x = (int)Mathf.Clamp(pos.x, 0.5f, ColumnCount - 0.5f);
        int y = (int)Mathf.Clamp(pos.y, 0.5f, RowCount - 0.5f);
        
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