using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script to ant prefab
public class Ant : MonoBehaviour
{
    private SimulationManager manager;

    private float maxDeviationAngle = 30; // Maximum turning angle of new orientations
    private float speed; // Movement speed

    private enum Info // Info to transferred to other ants
    {
        nothing, // No recent encounters
        food, // Recent encounter with food source
        nest // Recent encounter with nest
    }
    private enum Interaction // interaction states
    {
        moving, // default wandering movement
        collecting, // ant stops for a certain time period to collect or deposit food
        depositing,
        avoiding, // ant orients away from obstacles
        escaping, // ant orients away from disturbances at angle closer to maxDeviationAngle
    }

    private Tile currentTile; // Reference to the current tile the ant is on

    private void Start()
    {
        manager = GetComponentInParent<SimulationManager>();
        transform.forward = Random.insideUnitCircle;
        currentTile = manager.GetTile(transform.position);
        currentTile.AddAnt(this);
    }


    // Check current and neighboring tiles for possible interactions
    // Returns the nearest possible interaction if any exist
    private void CheckInteractions()
    {

    }

    // Check for collisions/boundaries to avoid
    private void CheckCollisions()
    {

    }

    // Calculate new orientation and new position to move toward
    private void CalculateMovement()
    {
        // Randomly choose whether to stop or to move (0.5% chance to stop)
        // If stop, ants stay at current position and orientation for certain period

        // If move, calculate new orientation

        // Call CheckInteractions()
        // If there are interactions, enter corresponding interaction state and recalculate new orientation

        // Call CheckCollisions()
        // If there are collisions, enter corresponding interact state and recalculate new orientation

        // Ants will always stop to collect or deposit food
    }
}
