using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private SimulationManager manager;
    private Collider2D collider2d;
    private Tile currentTile;

    private ContactFilter2D contactFilter = new ContactFilter2D();
    private Collider2D[] collisions = new Collider2D[10];

    private void Start()
    {
        manager = GetComponentInParent<SimulationManager>();
        collider2d = GetComponent<Collider2D>();
        transform.position = new Vector2(Random.Range(1f, manager.ColumnCount - 1f), Random.Range(1f, manager.RowCount - 1f));
        currentTile = manager.GetTile(transform.position);
        currentTile.AddObstacle(this);
    }
}
