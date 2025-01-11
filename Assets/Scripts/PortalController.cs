using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    private Vector3 exitPosition = new Vector3(-3f, -2f, 0f);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Get the MovingObject component
            MovingObject movingObject = other.gameObject.GetComponent<MovingObject>();

            // Stop all coroutines on the MovingObject to interrupt any ongoing movement
            if (movingObject != null)
            {
                movingObject.StopAllCoroutines();
            }

            // Teleport the player
            other.gameObject.transform.position = exitPosition;

            // Update the camera through GameController's room system
            GameController.control.MoveCamera(exitPosition);
        }
    }
}