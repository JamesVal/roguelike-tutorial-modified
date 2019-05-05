using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;
    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    private Animator animator;
    private Transform target;
    private bool skipMove;
    private PathFinder pathFinder;

    // Start is called before the first frame update
    protected override void Start()
    {
        GameManager.instance.RegisterEnemy(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;

        /* Add path finder / renderer */
        pathFinder = gameObject.AddComponent<PathFinder>();

        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
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
        Vector2 nextDirection;
        nextDirection = pathFinder.GetNextDirection();

        /*
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
        */

        AttemptMove<Player>((int)nextDirection.x, (int)nextDirection.y);
        //AttemptMove<Player>((int)nextDirection.x, 0);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        Debug.Log("hitplayer");

        hitPlayer.LoseFood(playerDamage);

        animator.SetTrigger("EnemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
    }
}
