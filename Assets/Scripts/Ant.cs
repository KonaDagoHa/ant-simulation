using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: get rid of dynamic rigidbodies, control ant behavior through overlapcircle and adjust collisions at end of frame


[RequireComponent(typeof(CapsuleCollider2D))]
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

    private float maxDeviationAngle = 30f; // Maximum turning angle of new orientations
    private float moveSpeed = 0.1f; // Movement speed

    // Timers for coroutines (in seconds)
    private float stopTimeLength = 1f; // how long ants should stop for
    private float determineMotionInterval = 0.2f; // time interval for running DetermineMotion() (increase for better performance)

    private Info info = Info.nothing;
    private Motion motion = Motion.moving;

    // Guide:
        // Position is ant's position in Vector2(x, y) corresponding with transform.position
        // Rotation is ant's rotation in Vector3(x, y, z) corresponding with transform.eulerAngles (x and y are always 0)
        // Orientation is ant's forward facing unit vector in Vector2(x, y) calculated based on its rotation

    public Vector2 currentPosition { get; set; } // assign to transform.position at end of frame
    public float currentRotationZ { get; set; } // assign to transform.eulerAngles.z at end of frame

    private Vector2 previousPosition; // position at the beginning of the previous frame
    private float stopChance = -1;
    private bool stopTimerIsRunning = false; // used to avoid the stopTimer coroutine stacking per frame
    private bool interactingWithFoodOrNest = false; // used to reassign motion state after interaction
    private float orientationWeight = 1f; // used to modify target orientation
    private float repulsionWeight = 0.1f; // used to affect the orientation of other ants in range
    private float detectionRadius = 0.04f;

    private void Awake()
    {
        // Cache components
        manager = GetComponentInParent<SimulationManager>();
        antCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        // Initialize position
        transform.position = new Vector2(Random.Range(0.5f, manager.ColumnCount - 0.5f), Random.Range(0.5f, manager.RowCount - 0.5f));
        previousPosition = currentPosition = transform.position;

        // Initialize rotation by setting its component to random number between -180 and 180 degrees
        transform.eulerAngles = new Vector3(0, 0, Random.Range(-180f, 180f));
        currentRotationZ = transform.eulerAngles.z;

        // Start motion determination coroutine
        StartCoroutine(DetermineMotion());

        // Start stopping coroutine
        StartCoroutine(StopTimer());

        
    }

    private void Update()
    {
        // ExecuteMotion should logically be at the end of Update(), but since coroutines run after Update(), it has to be called first
        ExecuteMotion();
        previousPosition = transform.position;
        // Coroutines run after Update()
    }
    
    // Execute ant's motion based on motion state
    private void ExecuteMotion()
    {

        if (motion == Motion.stopping && !stopTimerIsRunning)
        {
            stopTimerIsRunning = true; // start stop timer coroutine by setting this to true
        }
        else if (motion == Motion.moving)
        {
            Vector2 velocity = SimulationManager.RotationZToOrientation(currentRotationZ);
            velocity = Vector2.ClampMagnitude(velocity, moveSpeed);

            currentPosition += velocity * Time.deltaTime; // Move forward
            // Clamping at boundaries
            float clampMargins = transform.localScale.x; // used to prevent ants from getting stuck at boundaries
            float clampX = Mathf.Clamp(currentPosition.x, clampMargins, manager.ColumnCount - clampMargins);
            float clampY = Mathf.Clamp(currentPosition.y, clampMargins, manager.RowCount - clampMargins);
            currentPosition = new Vector2(clampX, clampY);
        }

        transform.eulerAngles = new Vector3(0, 0, currentRotationZ);
        transform.position = currentPosition;

    }

    // Called first at beginning of each timestep; determines if ant will stop or move and sets orientation accordingly
    private IEnumerator DetermineMotion()
    {
        while (true)
        {
            yield return new WaitForSeconds(determineMotionInterval); // Runs every determineMotionInterval seconds

            if (motion == Motion.moving) // if ant is moving
            {
                if (Random.value <= stopChance) // if small percentage chance
                {
                    motion = Motion.stopping; // stop
                }
                else // keep moving
                {
                    currentRotationZ += Random.Range(-maxDeviationAngle, maxDeviationAngle); // adjust rotation
                    CheckInteractions();
                }
            }
        }
    }

    // Stop for certain time period
    private IEnumerator StopTimer()
    {
        while (true)
        {
            // check if ant should be stopping
            if (motion == Motion.stopping && stopTimerIsRunning)
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
            yield return null; // Runs every frame
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
            Collider2D[] collisions = Physics2D.OverlapCircleAll(currentPosition, detectionRadius);

            for (int i = 0; i < collisions.Length; i++)
            {
                int collisionLayer = collisions[i].gameObject.layer;
                if (collisionLayer == LayerMask.NameToLayer("Obstacles"))
                {
                    AvoidObstacle(collisions[i]);
                }
                else if (collisionLayer == LayerMask.NameToLayer("Boundaries"))
                {
                    AvoidBoundary(collisions[i]);
                }
                else if (collisionLayer == LayerMask.NameToLayer("Nests"))
                {
                    InteractNest(collisions[i]);
                }
                else if (collisionLayer == LayerMask.NameToLayer("Foods"))
                {
                    InteractFood(collisions[i]);
                }
            }

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

    private void AvoidObstacle(Collider2D obstacleCollider)
    {
        // Ant will move perpendicular to the vector from ant to obstacle
        // Ant will move in the direction closest to its previous orientation
        Vector2 antToObstacle = (Vector2)obstacleCollider.transform.position - currentPosition;
        Vector2 antToObstaclePerp = Vector2.Perpendicular(antToObstacle);
        Vector2 antToObstaclePerpOpposite = antToObstaclePerp * -1;
        Vector2 currentOrientation = SimulationManager.RotationZToOrientation(currentRotationZ);

        if (Vector2.Distance(currentOrientation, antToObstaclePerp) < Vector2.Distance(currentOrientation, antToObstaclePerpOpposite))
        {
            currentRotationZ = Mathf.MoveTowardsAngle(currentRotationZ, SimulationManager.OrientationToRotationZ(antToObstaclePerp) + 10f, 20f);
        }
        else
        {
            currentRotationZ = Mathf.MoveTowardsAngle(currentRotationZ, SimulationManager.OrientationToRotationZ(antToObstaclePerpOpposite) - 10f, 20f);
        }
    }


    private void AvoidBoundary(Collider2D boundaryCollider)
    {
        Vector2 boundaryToAnt = currentPosition - boundaryCollider.ClosestPoint(currentPosition);
        currentRotationZ = Mathf.MoveTowardsAngle(currentRotationZ, SimulationManager.OrientationToRotationZ(boundaryToAnt), 40f);
    }

    // Checks if ant's collider2d is touching a food or nest's collider2d and sets motion state accordingly
    private void InteractNest(Collider2D nestCollider)
    {
        if (info == Info.food)
        {
            if (Vector2.Distance(currentPosition, nestCollider.ClosestPoint(currentPosition)) <= 0)
            {
                motion = Motion.stopping;
                interactingWithFoodOrNest = true;
            }
        }
    }

    private void InteractFood(Collider2D foodCollider)
    {
        if (info == Info.nest || info == Info.nothing)
        {
            if (Vector2.Distance(currentPosition, foodCollider.ClosestPoint(currentPosition)) <= 0)
            {
                motion = Motion.stopping;
                interactingWithFoodOrNest = true;
            }
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
