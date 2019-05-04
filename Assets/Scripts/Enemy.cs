using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    private PathFinder pathFinder;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        /* Add path finder / renderer */
        pathFinder = gameObject.AddComponent<PathFinder>();

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        MoveEnemy();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);
        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        if (Mathf.Abs(target.position.x - transform.position.x) <= .45f)
        {
            if (Mathf.Abs(target.position.y - transform.position.y) > .45f)
                yDir = target.position.y > transform.position.y ? 1 : -1;
            else
                xDir = target.position.x > transform.position.x ? 1 : -1;
            Debug.Log("Y");
        }
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
            Debug.Log("X");
        }

        //AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        hitPlayer.LoseFood(playerDamage);

        animator.SetTrigger("EnemyAttack");
    }
}
