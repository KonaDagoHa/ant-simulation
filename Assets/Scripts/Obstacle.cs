using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private SimulationManager manager;
    private Collider2D collider2d;
    private Tile currentTile;

    void Start()
    {
        manager = GetComponentInParent<SimulationManager>();
        collider2d = GetComponent<Collider2D>();
        transform.position = new Vector3(Random.Range(1f, manager.maxColumns - 1f), Random.Range(1f, manager.maxRows - 1f));
        currentTile = manager.GetTile(transform.position);
        currentTile.AddObstacle(this);
    }

    public Collider2D GetCollider() { return collider2d; }
}
