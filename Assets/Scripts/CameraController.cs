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
        transform.position = new Vector3(manager.GetWidth() / 2f - manager.GetTileSize() / 2f, manager.GetHeight() / 2f - manager.GetTileSize() / 2f, -10);
    }

}
