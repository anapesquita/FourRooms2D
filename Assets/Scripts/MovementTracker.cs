using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTracker : MonoBehaviour
{
    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = transform.position;
        GameController.control.characterSpawnLocation = transform.position;
    }

    void Update()
    {
        if (transform.position != previousPosition)
        {
            GameController.control.UpdateTravelDistance(transform.position);
            previousPosition = transform.position;
        }
    }
}