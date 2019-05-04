using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rigidBody;
    private float inverseMoveTime;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rigidBody = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        Vector2 boxVector = new Vector2(0f, 0f);

        if (xDir != 0)
        {
            boxVector.x = (xDir < 0) ? .5f * -1f : .5f;
        }

        if (yDir != 0)
        {
            boxVector.y = (yDir < 0) ? .5f * -1f : .5f;
        }


        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start + boxVector, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            Debug.Log("MOVE" + end);
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        else if (hit.distance > inverseMoveTime * Time.deltaTime)
        {
            //Debug.Log("CLOSE MOVE");
            Debug.Log("Pos" + transform.position);
            Debug.Log("Hit Dist:" + hit.distance);
            if (xDir != 0) end = start + new Vector2((xDir < 0) ? hit.distance * -1f : hit.distance, 0);
            else if (yDir != 0) end = start + new Vector2(0, (yDir < 0 ) ? hit.distance * -1f : hit.distance);
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        Debug.Log("NO MOVE");
        return false;
    }   

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        //float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        //while (sqrRemainingDistance > float.Epsilon)
        //{
            Vector3 newPosition = Vector3.MoveTowards(rigidBody.position, end, inverseMoveTime * Time.deltaTime);
            if (newPosition.y < 0) Debug.Log("Negative?" + newPosition.y);
            rigidBody.MovePosition(newPosition);
            //sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        //}
    }

    protected virtual void AttemptMove<T>(int xDir, int yDir)
        where T : Component
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

    protected abstract void OnCantMove<T>(T componenet)
        where T : Component;
}
