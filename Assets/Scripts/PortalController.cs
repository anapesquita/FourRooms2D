using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    private Vector3 secondPortalLocation = new Vector3(-3f, -3f, 0f);
    public bool isSecondPortal = false;

    // Static variables shared between all portals
    private static bool isTeleporting = false;
    private static GameObject currentTeleportingPlayer = null;
    private static PortalController destinationPortal = null;
    private const float MINIMUM_EXIT_DISTANCE = 0.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isTeleporting)
        {
            // Don't teleport if this is the destination portal of an ongoing teleport
            if (this == destinationPortal)
            {
                return;
            }

            StartCoroutine(TeleportPlayer(other.gameObject));
        }
    }

    private IEnumerator TeleportPlayer(GameObject player)
    {
        isTeleporting = true;
        currentTeleportingPlayer = player;

        // Get the MovingObject component
        MovingObject movingObject = player.GetComponent<MovingObject>();

        // Stop all coroutines on the MovingObject to interrupt any ongoing movement
        if (movingObject != null)
        {
            movingObject.StopAllCoroutines();
        }

        // Determine teleport destination and set destination portal
        Vector3 destination;
        if (isSecondPortal)
        {
            destination = GameController.control.portalSpawnLocation;
            destinationPortal = FindFirstPortal();
        }
        else
        {
            destination = secondPortalLocation;
            destinationPortal = FindSecondPortal();
        }

        // Teleport the player
        player.transform.position = destination;

        // Update the camera
        GameController.control.MoveCamera(destination);

        yield return new WaitForSeconds(0.1f);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Only reset teleport state when the teleporting player exits the destination portal
            if (this == destinationPortal && other.gameObject == currentTeleportingPlayer)
            {
                // Calculate distance between player and portal
                float distance = Vector2.Distance(transform.position, other.transform.position);

                // Only reset if player is far enough away
                if (distance >= MINIMUM_EXIT_DISTANCE)
                {
                    isTeleporting = false;
                    currentTeleportingPlayer = null;
                    destinationPortal = null;
                }
            }
        }
    }

    private PortalController FindFirstPortal()
    {
        PortalController[] portals = FindObjectsOfType<PortalController>();
        foreach (PortalController portal in portals)
        {
            if (!portal.isSecondPortal)
                return portal;
        }
        return null;
    }

    private PortalController FindSecondPortal()
    {
        PortalController[] portals = FindObjectsOfType<PortalController>();
        foreach (PortalController portal in portals)
        {
            if (portal.isSecondPortal)
                return portal;
        }
        return null;
    }
}