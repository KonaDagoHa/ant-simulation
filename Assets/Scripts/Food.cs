﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    private SimulationManager manager;
    private Collider2D collider2d;
    private Tile currentTile;

    void Start()
    {
        manager = GetComponentInParent<SimulationManager>();
        collider2d = GetComponent<Collider2D>();
        transform.position = new Vector2(Random.Range(1f, manager.ColumnCount - 1f), Random.Range(1f, manager.RowCount - 1f));
        currentTile = manager.GetTile(transform.position);
        currentTile.AddFood(this);
    }

    public Collider2D GetCollider() { return collider2d; }
}
