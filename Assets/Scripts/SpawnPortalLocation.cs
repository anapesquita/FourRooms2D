using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPortalLocation : MonoBehaviour
{
    void Start()
    {
        transform.position = GameController.control.portalSpawnLocation;
    }
}