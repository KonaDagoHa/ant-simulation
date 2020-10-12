using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Tile grid could be made as a hashmap like in the paper, but that would get messy as there would ...
    // need to be a separate script to initialize each tile; if performance suffers, consider the hashmap method

// Attach this script to tile prefab
public class Tile : MonoBehaviour
{
    private SimulationManager manager;

    private Tile[] neighbors; // references to neighboring tiles

    // Lists to track occupants currently on tile
    private List<Ant> ants;
    private List<Nest> nests;
    private List<Food> foods;
    private List<Obstacle> obstacles;
    private List<Disturbance> disturbances;

    // Functions for Ant.cs
    public void AddAnt(Ant ant) { ants.Add(ant); }
    public void RemoveAnt(Ant ant) { ants.Remove(ant); }
    public Tile[] GetNeighbors() { return neighbors; }
    // Functions for Nest.cs
    public void AddNest(Nest nest) { nests.Add(nest);  }
    public void RemoveNest(Nest nest) { nests.Remove(nest); }
    // Functions for Food.cs
    public void AddFood(Food food) { foods.Add(food); }
    public void RemoveFood(Food food) { foods.Remove(food); }

    private void Start()
    {
        manager = GetComponentInParent<SimulationManager>();
        ants = new List<Ant>();
        nests = new List<Nest>();
        foods = new List<Food>();
        obstacles = new List<Obstacle>();
        disturbances = new List<Disturbance>();

        neighbors = InitNeighbors();
    }

    // Returns references to all neighboring tiles
    private Tile[] InitNeighbors()
    {
        Vector2 pos = transform.position;
        // World position coordinates of neighboring tiles
        Vector2[] neighborPos = new Vector2[]
        {
            new Vector2(pos.x - 1, pos.y + 1), // northwest
            new Vector2(pos.x, pos.y + 1), // north
            new Vector2(pos.x + 1, pos.y + 1), // northeast
            new Vector2(pos.x - 1, pos.y), // west
            new Vector2(pos.x + 1, pos.y), // east
            new Vector2(pos.x - 1, pos.y - 1), // southwest
            new Vector2(pos.x, pos.y - 1), // south
            new Vector2(pos.x + 1, pos.y - 1), // southeast
        };

        // Tile list to store tile neighbors temporarily
        List<Tile> neighborList = new List<Tile>();

        // Traverse through neighbor positions to see which are valid
        for (int i = 0; i < neighborPos.Length; i++)
        {
            // If neighbor is within grid boundaries, add to neighborList
            if (neighborPos[i].x > -1 && neighborPos[i].x < manager.maxColumns &&
                neighborPos[i].y > -1 && neighborPos[i].y < manager.maxRows)
            {
                neighborList.Add(manager.GetTile(neighborPos[i]));
            }
        }

        return neighborList.ToArray();

    }
    
}
