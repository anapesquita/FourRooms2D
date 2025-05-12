using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalController : MonoBehaviour
{
    private Vector3 secondPortalLocation = new Vector3(-3f, -3f, 0f);
    public bool isSecondPortal = false;

    // Static variables for all portals
    private static bool globalTeleportLock = false;
    private static bool isExitingPortal = false;
    private static float exitTimer = 0f;
    private static float REQUIRED_EXIT_TIME = 0.5f;
    private const float MINIMUM_EXIT_DISTANCE = 1f;

    private void Update()
    {
        if (isExitingPortal)
        {
            exitTimer += Time.deltaTime;
            if (exitTimer >= REQUIRED_EXIT_TIME)
            {
                isExitingPortal = false;
                globalTeleportLock = false;
                exitTimer = 0f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !globalTeleportLock)
        {
            StartCoroutine(TeleportPlayer(other.gameObject));
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // Reset exit timer if player is still in any portal
            exitTimer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            float distance = Vector2.Distance(transform.position, other.transform.position);
            if (distance >= MINIMUM_EXIT_DISTANCE)
            {
                isExitingPortal = true;
                exitTimer = 0f;
            }
        }
    }

    private IEnumerator TeleportPlayer(GameObject player)
    {
        // Lock teleportation globally
        globalTeleportLock = true;
        isExitingPortal = false;

        // Get the MovingObject component
        MovingObject movingObject = player.GetComponent<MovingObject>();
        if (movingObject != null)
        {
            movingObject.StopAllCoroutines();
        }

        // Determine teleport destination
        Vector3 destination;
        if (isSecondPortal)
        {
            destination = GameController.control.portalSpawnLocation;
        }
        else
        {
            destination = secondPortalLocation;
        }

        // Short delay before teleport
        yield return new WaitForSeconds(0.1f);

        // Teleport the player
        Vector3 previousPosition = player.transform.position;
        player.transform.position = destination;

        // Update game state
        GameController.control.MoveCamera(destination);
        GameController.control.portalUsedBeforeTarget = true;
        GameController.control.portalUsedType = isSecondPortal ? "second" : "first";

        // Add only one meter to the total travel distance
        GameController.control.totalTravelDistance += 1f;
        GameController.control.SetPreviousPosition(destination);

        // Force a short delay before allowing exit check
        yield return new WaitForSeconds(0.2f);
    }
}