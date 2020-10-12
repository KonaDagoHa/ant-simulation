using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private SimulationManager manager;

    void Start()
    {
        manager = GetComponentInParent<SimulationManager>();
        // Center the camera on the tile grid
        transform.position = new Vector3(manager.maxColumns / 2f, manager.maxRows / 2f, -10);
        GetComponent<Camera>().orthographicSize = manager.maxRows / 2f;
    }

}
