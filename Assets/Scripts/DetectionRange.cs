using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionRange : MonoBehaviour
{
    private Ant ant;

    private void Awake()
    {
        ant = GetComponentInParent<Ant>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TriggerCollision = collision;
        Debug.Log(collision.gameObject.name);
    }

    // Manipulate ant's current position and orientation 
}
