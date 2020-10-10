using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script to ant prefab
public class Ant : MonoBehaviour
{
    private enum Info // Info to transferred to other ants
    {
        nothing, // No recent encounters
        food, // Recent encounter with food source
        nest // Recent encounter with nest
    }
    private enum Motion // motion states
    {
        // Change to stop, move, collect, deposit, avoid, escape if you want a different animation for each state

        stop,
        move,

        // get rid of these two
        //interactEnvironment, // involves stopping for certain time period
        //interactNeighbors, // does NOT involve stopping for certain time period
        
    }

    private SimulationManager manager;

    private float maxDeviationAngle = 30f; // Maximum turning angle of new orientations
    private float speed = 1f; // Movement speed

    private Info info = Info.nothing;
    private Motion motion = Motion.move;
    

    private Tile currentTile; // Reference to the current tile the ant is on

    private void Start()
    {
        manager = GetComponentInParent<SimulationManager>();
        // Assign ant random orientation by setting its z-rotation number between 0 and 360 degrees
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
        currentTile = manager.GetTile(transform.position);
        currentTile.AddAnt(this);
    }

    private void Update()
    {
        DetermineMotion();
        ExecuteMotion();

        
    }

    // Called first at beginning of each timestep; determines if ant will stop or move and sets orientation accordingly
    private void DetermineMotion()
    {
        // if 0.5% chance
        if (Random.value <= 0.05f)
        {
            motion = Motion.stop;
        } else
        {
            motion = Motion.move;
            //transform.eulerAngles += new Vector3(0, 0, Random.Range(-maxDeviationAngle, maxDeviationAngle));
        }
            // motion = Motion.stop;
        // else
            // motion = Motion.move;
            // Add to ant's orientation by angle within -maxDeviationAngle and +maxDeviationAngle
            // Call CheckStop()
            // Call CheckInteractions()
            // Call CheckCollisions()
    }

    private void ExecuteMotion()
    {
        // ants move perpendicular to their body; offset eulerAngles by 90 degrees because a eulerAngle of 0 is at the +y axis, not the +x axis

        if (motion == Motion.stop)
        {
            // Stop for certain time period (maybe through coroutine)
        } else if (motion == Motion.move)
        {
            // Move forward
            // vector that points in ant's forward direction; the "+ 90" is needed to reorient ant's sprite to point in same direction as direction of movement
            Vector2 moveVector = new Vector2(Mathf.Cos((transform.eulerAngles.z + 90) * Mathf.Deg2Rad), Mathf.Sin((transform.eulerAngles.z + 90) * Mathf.Deg2Rad));
            moveVector = Vector2.ClampMagnitude(moveVector, speed * Time.deltaTime);
            transform.position += (Vector3) moveVector;
        }
    }


    // Check current and neighboring tiles for possible interactions and adjust orientation accordingly
    private void CheckInteractions()
    {
        // Only disturbances can interrupt ant stopping
        // TODO: Also need to check if the closest entity for each type of interaction (obstacles, disturbances, nests, food, neighboring ants) ...
            // if within a certain distance range
            // Ex: Ant has detectionRange of 0.5, so even if there was an obstacle inside an eastern neighboring tile, ant cannot detect it unless ...
                // the obstacle's position is within a circle with radius 0.5 centered on the ant
            // If this overcomplicates things, don't do it

        // TODO: make variables to store previous position of each ant (needed for finding the new orientation of ant after information transform between ants
            // See *** Check for neighbor interactions *** for details

        // when ant stops at food/nest, the ant's info only changes to food/nest respectively AFTER the timer for stopping runs out (when the ant is no longer stopping)


        // If motion != Motion.stop (assumes that motion == Motion.move)

            Tile[] neighborTiles = currentTile.GetNeighbors();
            // Interaction priorities: environment > neighbors
            // Environment priorities: obstacles > disturbances > food = nest

            // *** Check for environmental interactions ***

            // if current or neighbor tiles have an obstacle
                // for each obstacle
                    // find the obstacle that is closest to ant's position
                    // set ant's orientation to be perpendicular to the vector pointing from ant to obstacle (50/50 chance for left/right)

            // else if current or neighbor tiles have a disturbance
                // for each disturbance in current or neighboring tiles
                    // find the closest disturbance
                    // set ant's orientation to be opposite to the vector pointing from ant to disturbance

            // else if current or neighboring tiles have a nest
                // for each nest
                    // find closest nest
                    // if info == Info.food
                        // Adjust orientation to point toward nest
                        // info = Info.nest
                    // else if info == Info.nothing
                        // info = Info.nest (ant does not stop to interact)

            // else if current or neighboring tiles have a food source
                // for each food source
                    // find closest food source
                    // if info == Info.nest or info == Info.nothing
                        // adjust orientation to point toward food
                        // info = Info.food
               
            // *** Check for neighbor interactions ***

            // else if current tile has neighbor ants (not neighboring tile because these ants communicate through physical contact)
                // for each neighbor ant in current or neighboring tiles (if there are any)
                    // if info == Info.nest or info == Info.nothing
                        // if neighborAnt.info == Info.food
                            // Change orientation to move toward food
                                // Do this by pointing the orientation toward the neighborAnt's previous
                                // This means we have to calculate and store previous positions for each ant
                    // else if int == info.food
                        // if neighborAnt.info == Info.nest
                            // Change orientation to move toward nest

        // else (Ant is currently stopping)

            // if current or neighboring tiles have a disturbance
                // motion = Motion.move;
                // for each disturbance in current or neighboring tiles
                    // 

        

    }

    // Checks if ant's collider2d is touching a food or nest's collider2d and sets motion state accordingly
    private void CheckStop()
    {
        // Collider2d between ants and food/nest cannot have physics for this to work (ants can overlap food/nest)

        // If touching food and (info == Info.nest or info == Info.nothing)
            // motion = Motion.stop;
        // else if touching nest and info == Info.food
            // motion = Motion.stop;
    }

    // Check for collisions with neighbor ants and grid boundaries
    private void CheckCollisions()
    {
        // collider2d ants and obstacles should have physics (they cannot pass through each other)
    }

    // No physics collisions: ants and food/nest, ants and disturbance
    // Physics collisions: ants and ants, ants and obstacles

    // Consider making it so ants can overlap each other when within certain range of nest/food (this will fix bugs with ants getting stuck)
    // OR just make it so ants can't collide with other ants (Plan B)
}
