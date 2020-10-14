using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(CapsuleCollider2D), typeof(Rigidbody2D))]
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
    private CapsuleCollider2D antCollider; // main collider to be detected by other ants
    private Rigidbody2D antBody; // Kinematic rigidbody for moving ants

    private float maxDeviationAngle = 5f; // Maximum turning angle of new orientations
    private float moveSpeed = 0.5f; // Movement speed
    private float stopTimeLength = 1f; // in seconds

    private Info info = Info.nothing;
    private Motion motion = Motion.moving;

    // Guide:
        // Position is ant's position in Vector2(x, y) corresponding with transform.position
        // Rotation is ant's rotation in Vector3(x, y, z) corresponding with transform.eulerAngles (x and y are always 0)
        // Orientation is ant's forward facing unit vector in Vector2(x, y) calculated based on its rotation

    private Vector2 previousPosition; // position at the beginning of the previous frame
    public Vector2 CurrentPosition { get; set; } // assign to transform.position at end of frame
    private float CurrentRotationZ { get; set; } // assign to transform.eulerAngles.z at end of frame

    private bool stopTimerIsRunning = false; // used to avoid the stopTimer coroutine stacking per frame
    private bool interactingWithFoodOrNest = false; // used to reassign motion state after interaction
    private float orientationWeight = 1f; // used to modify target orientation
    private float repulsionWeight = 1f; // used to affect the orientation of other ants in range

    private void Awake()
    {
        // Cache components
        manager = GetComponentInParent<SimulationManager>();
        antCollider = GetComponent<CapsuleCollider2D>();
        antBody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Initialize position
        transform.position = new Vector2(Random.Range(0.5f, manager.ColumnCount - 0.5f), Random.Range(0.5f, manager.RowCount - 0.5f));
        previousPosition = CurrentPosition = transform.position;

        // Initialize rotation by setting its component to random number between -180 and 180 degrees
        transform.eulerAngles = new Vector3(0, 0, Random.Range(-180f, 180f));
        currentRotationZ = transform.eulerAngles.z;
    }

    private void Update()
    {
        
        previousPosition = CurrentPosition = transform.position;
        currentRotationZ = transform.eulerAngles.z;
        DetermineMotion();
        ExecuteMotion();

        
    }

    // Called first at beginning of each timestep; determines if ant will stop or move and sets orientation accordingly
    private void DetermineMotion()
    {
        if (motion == Motion.moving) // if ant is moving
        {
            if (Random.value <= 0.003) // if small percentage chance
            {
                motion = Motion.stopping; // stop
            }
            else // keep moving
            {
                currentRotationZ += Random.Range(-maxDeviationAngle, maxDeviationAngle); // adjust rotation
                CheckInteractions();
                // Call CheckCollisions()
            }
        }
    }

    // Execute ant's motion based on motion state
    private void ExecuteMotion()
    {

        if (motion == Motion.stopping && !stopTimerIsRunning)
        {
            stopTimerIsRunning = true;
            StartCoroutine(StopTimer());
        }
        else if (motion == Motion.moving)
        {
            // TODO: use rigidbody2d's moveposition and rotation functions for optimal performance
            Vector2 velocity = SimulationManager.RotationZToOrientation(currentRotationZ);
            velocity = Vector2.ClampMagnitude(velocity, moveSpeed);
            CurrentPosition += velocity * Time.deltaTime; // Move forward
        }

        transform.position = CurrentPosition;
        transform.eulerAngles = new Vector3(0, 0, currentRotationZ); 
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
            }
            else if (info == Info.food)
            {
                info = Info.nest;
            }
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

        // TODO: make a Vector2 orientationVector instead of using transform.eulerAngles
            // Convert orientationVector to transform.eulerAngles at end of frame

        if (motion == Motion.moving)
        {


            // Interaction priorities: environment > neighbors
            // Environment priorities: obstacles > disturbances > food = nest
            // Therefore: obstacles > disturbances > food = nest > neighbor ants

            // *** Check for environmental interactions ***

            //InteractObstacles();
            
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

            // if current tile has neighbor ants (not neighboring tile because these ants communicate through physical contact)
                // for each neighbor ant in current or neighboring tiles (if there are any)
                    // if info == Info.nest or info == Info.nothing
                        // if neighborAnt.info == Info.food
                            // Change orientation to move toward food
                                // Do this by pointing the orientation toward the neighborAnt's previous
                                // This means we have to calculate and store previous positions for each ant
                    // else if int == info.food
                        // if neighborAnt.info == Info.nest
                            // Change orientation to move toward nest
        }

        // else (Ant is currently stopping)

            // if current or neighboring tiles have a disturbance
                // motion = Motion.move;
                // for each disturbance in current or neighboring tiles
                    //


        

    }

    // Ants change their orientation to be perpendicular to the vector pointing from ant to obstacle (50/50 chance for left/right)
    // TODO: Make it so ant's only check for obstacles within a certain circular range
    private void InteractObstacles()
    {
        /*
        List<Obstacle> totalObstacles = currentTile.GetObstacles(); // add obstacles of current tile to totalObstacles
        
        if (totalObstacles.Count > 0) // if there are 1 or more obstacles
        {
            Obstacle closestObstacle = totalObstacles[0];
            Vector2 vectorAntToObstacle = closestObstacle.transform.position - transform.position;

            if (totalObstacles.Count > 1) // if there are 2 or more obstacles
            {
                float distanceToClosestObstacle = vectorAntToObstacle.magnitude;
                float distance; // Temporary distance for Vector2 calculation

                for (int i = 1; i < totalObstacles.Count; i++) // find and assign closest obstacle to closestObstacle
                {
                    distance = Vector2.Distance(transform.position, totalObstacles[i].transform.position);

                    if (distance < distanceToClosestObstacle)
                    {
                        distanceToClosestObstacle = distance;
                        closestObstacle = totalObstacles[i];
                    }
                }

                vectorAntToObstacle = transform.position - closestObstacle.transform.position;
            }

            // Ant will move perpendicular to the vector from ant to obstacle
            // Ant will move in the perpendicular direction closest to its previous rotation

            Vector2 normalVectorObstacleToAnt = Vector2.Perpendicular(vectorAntToObstacle);
            Vector2 normalVectorObstacleToAntOpposite = normalVectorObstacleToAnt * -1;
            Vector2 orientation = SimulationManager.RotationZToOrientation(currentRotationZ);

            if (Vector2.Distance(orientation, normalVectorObstacleToAnt) < Vector2.Distance(orientation, normalVectorObstacleToAntOpposite))
            {
                currentRotationZ = SimulationManager.OrientationToRotationZ(normalVectorObstacleToAnt);
            }
            else
            {
                currentRotationZ = SimulationManager.OrientationToRotationZ(normalVectorObstacleToAntOpposite);
            }

        }
        */
    }

    // TODO: ant should orient itself toward the food/nest before stopping

    // Checks if ant's collider2d is touching a food or nest's collider2d and sets motion state accordingly
    private void InteractNest(Nest nest)
    {
        if (antCollider.IsTouching(nest.GetCollider()) && info == Info.food)
        {
            motion = Motion.stopping;
            interactingWithFoodOrNest = true;
        }
    }

    private void InteractFood(Food food)
    {
        if (antCollider.IsTouching(food.GetCollider()) && (info == Info.nest || info == Info.nothing))
        {
            motion = Motion.stopping;
            interactingWithFoodOrNest = true;
        }
    }

    // Interact with neighboring ants by adjusting orientation to avoid collision
    private void InteractNeighbors()
    {

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
