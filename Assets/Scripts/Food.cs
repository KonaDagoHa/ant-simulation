using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private SimulationManager manager;

    private Tile currentTile;

    void Start()
    {
        manager = GetComponentInParent<SimulationManager>();
        transform.position = new Vector3(Random.Range(1f, manager.maxColumns - 1f), Random.Range(1f, manager.maxRows - 1f));
        currentTile = manager.GetTile(transform.position);
        currentTile.AddFood(this);
    }
}
