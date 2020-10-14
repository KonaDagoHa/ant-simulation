using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to a circlecollider2d to be used to detect other objects
public class Detection : MonoBehaviour
{
    // USE PHYSICS2D.OVERLAPCIRCLEALL() INSTEAD OF USING OVERLAPCOLLIDER()
        // another option is circlecast, but overlapcircle makes more sense and will probably have better performance

    /*
    // Use overlapcollider() instead of trigger
        // Check every time interval using invokerepeating()

    private Ant ant;
    private CircleCollider2D detectionCollider;
    private Collider2D[] colliders;
    private ContactFilter2D contactFilter;
    private int colliderCount;
    private int maxColliders; // For performance, limit the maximum amount of colliders a single ant can detect

    private void Awake()
    {
        ant = GetComponent<Ant>();
        detectionCollider = GetComponent<CircleCollider2D>();
        colliders = new Collider2D[]
        contactFilter = new ContactFilter2D();
    }

    private void Start()
    {
        //InvokeRepeating("InteractAnts", )
    }

    private void Update()
    {
        
    }

    private void AvoidObstacle()
    {
        colliderCount = detectionCollider.OverlapCollider(contactFilter.NoFilter(), colliders);
    }
    */

    
}
