using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    private SimulationManager manager;

    void Start()
    {
        manager = GetComponentInParent<SimulationManager>();
        // Center the camera on the tile grid
        transform.position = new Vector3(manager.ColumnCount / 2f, manager.RowCount / 2f, -10);
        GetComponent<Camera>().orthographicSize = manager.RowCount / 2f;
    }

}
