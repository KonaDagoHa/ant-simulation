using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attach this script to ant prefab
[RequireComponent(typeof(Collider2D))]
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
        stopping,
        moving,
    }

    private SimulationManager manager;
    private Collider2D collider2d;



    //[SerializeField]
    private float maxDeviationAngle = 5f; // Maximum turning angle of new orientations
    //[SerializeField]
    private float moveSpeed = 0.5f; // Movement speed
    //[SerializeField]
    private float stopTimeLength = 1f; // in seconds




    private Info info = Info.nothing;
    private Motion motion = Motion.moving;
    private Tile currentTile; // Reference to the current tile the ant is on
    private Tile[] neighborTiles;

    // These two variables may not be needed
    private Vector2 currentPosition; // Use this to cache transform.position in Start() if referred to repeatedly
    private Vector2 currentOrientation; // Use this to cache transform.eulerAngles if referred to repeatedly

    private bool stopTimerIsRunning = false; // used to avoid the stopTimer coroutine stacking per frame
    private bool interactingWithFoodOrNest = false; // used to reassign motion state after interaction
    private float orientationWeight = 1f; // used to modify target orientation
    private float repulsionWeight = 1f; // used to affect the orientation of other ants in range

    private void Start()
    {
        manager = GetComponentInParent<SimulationManager>();
        collider = GetComponent<Collider2D>();
        Debug.Log(collider);
        // Initialize position
        currentPosition = new Vector2(Random.Range(0.5f, manager.maxColumns - 0.5f), Random.Range(0.5f, manager.maxRows - 0.5f));
        transform.position = currentPosition;

        // Initialize orientation by setting its z-rotation to random number between 0 and 360 degrees
        transform.eulerAngles = new Vector3(0, 0, Random.Range(0f, 360f));
        // Add ant to tile
        currentTile = manager.GetTile(currentPosition);
        currentTile.AddAnt(this);
    }

    private void Update()
    {
        UpdateTile();
        currentPosition = transform.position;
        currentOrientation = transform.eulerAngles;
        DetermineMotion();
        ExecuteMotion();

        
    }

    // Updates the ant's current tile
    private void UpdateTile()
    {
        // If ant is on a different tile than the one in the previous timestep
        if (manager.GetTilePosition(currentPosition) != manager.GetTilePosition(transform.position)) 
        {
            // currentPosition is the previous position, and transform.position is the current position
            currentTile.RemoveAnt(this); // Remove ant from previous tile
            currentTile = manager.GetTile(currentPosition); // Update currentTile to the current tile
            currentTile.AddAnt(this); // Add ant to the current tile
            neighborTiles = currentTile.GetNeighbors();
        }
        
    }

    // Called first at beginning of each timestep; determines if ant will stop or move and sets orientation accordingly
    private void DetermineMotion()
    {
        if (motion == Motion.moving) // if ant is moving
        {
            if (Random.value <= -0.005) // if small percentage chance, stop
            {
                motion = Motion.stopping;
            } else // keep moving, adjust orientation
            {


                transform.eulerAngles += new Vector3(0, 0, Random.Range(-maxDeviationAngle, maxDeviationAngle));
            }
            // motion = Motion.stop;
            // else
            // motion = Motion.move;
            // Add to ant's orientation by angle within -maxDeviationAngle and +maxDeviationAngle
            // Call CheckStop()
            // Call CheckInteractions()
            // Call CheckCollisions()
        }
    }

    // Execute ant's motion based on motion state
    private void ExecuteMotion()
    {

        if (motion == Motion.stopping && !stopTimerIsRunning)
        {
            stopTimerIsRunning = true;
            StartCoroutine(StopTimer());
        } else if (motion == Motion.moving)
        {


            // vector that points in ant's forward direction; the "+ 90" is needed to reorient ant's sprite to point in same direction as direction of movement
            Vector2 moveVector = new Vector2(Mathf.Cos((transform.eulerAngles.z + 90) * Mathf.Deg2Rad), Mathf.Sin((transform.eulerAngles.z + 90) * Mathf.Deg2Rad));
            moveVector = Vector2.ClampMagnitude(moveVector, moveSpeed * Time.deltaTime);
            currentPosition += moveVector; // Move forward
        }

        transform.position = currentPosition;

    }

    // Stop for certain time period
    private IEnumerator StopTimer()
    {
        yield return new WaitForSeconds(stopTimeLength);
        motion = Motion.moving;
        stopTimerIsRunning = false;
        // For stopping to interact with food/nest
        if (interactingWithFoodOrNest)
        {
            interactingWithFoodOrNest = false;
            if (info == Info.nest || info == Info.nothing)
            {
                info = Info.food;
            } else if (info == Info.food)
            {
                info = Info.nest;
            }
        }
    }


    // Check current and neighboring tiles for possible interactions and adjust orientation accordingly
    private void CheckInteractions()
    {
        float repulsion = 0; // sum of all orientational repulsions from environment/neighbors
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
        

            //Tile[] neighborTiles = currentTile.GetNeighbors();
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
    private void InteractNest(Nest nest)
    {
        if (collider2d.IsTouching(nest.GetCollider()) && info == Info.food)
        {
            motion = Motion.stopping;
            interactingWithFoodOrNest = true;
        }
    }

    private void InteractFood(Food food)
    {
        if (collider2d.IsTouching(food.GetCollider()) && (info == Info.nest || info == Info.nothing))
        {
            motion = Motion.stopping;
            interactingWithFoodOrNest = true;
        }
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
    // To make ants collide, add Collider2D and Rigidbody2D components to Ant prefab
    // Add Collider2D to Food and Nest prefab, but make sure to check "is trigger" since ants will need to go through food and nest
        // Maybe experiment on whether ants can go through food or nest
}
