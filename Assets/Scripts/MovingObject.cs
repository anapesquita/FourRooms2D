using UnityEngine;
using System.Collections;

// The abstract keyword enables you to create classes and class members that are incomplete and must be implemented in a derived class.
// This code has been downloaded from the 2D game Unity tutorial:  https://unity3d.com/learn/tutorials/projects/2d-roguelike-tutorial/moving-object-script?playlist=17150
// Date downloaded: 13/05/2019

public abstract class MovingObject : MonoBehaviour
{
    private float oneSquareMoveTime;
    public LayerMask blockingLayer;
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;
    private Coroutine moveCoroutine;        // Added to track the current movement coroutine

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        oneSquareMoveTime = GameController.control.oneSquareMoveTime;
        inverseMoveTime = 1f / oneSquareMoveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            // Stop any existing movement before starting a new one
            if (moveCoroutine != null)
            {
                StopCoroutine(moveCoroutine);
            }
            moveCoroutine = StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > 0.000001f)
        {
            Vector3 newPostion = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPostion);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }

        GameController.control.MoveCamera(transform.position);
        moveCoroutine = null;
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir) where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);
        }
    }

    protected abstract void OnCantMove<T>(T component) where T : Component;
}