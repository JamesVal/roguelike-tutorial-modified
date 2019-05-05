using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class Player : MovingObject
{
    public int wallDamage;
    public int pointsPerFood;
    public int pointsPerSoda;
    public float restartLevelDelay;
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;
    private Text foodText;

    // Start is called before the first frame update
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        foodText = GameObject.Find("FoodText").GetComponent<Text>();
        food = GameManager.instance.playerFoodPoints;
        InvokeRepeating("PeriodicFoodLoss", 1f, 1f);
        GameManager.instance.RegisterPlayer(this);
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        //If Move returns true, meaning Player was able to move into an empty space.
        if (Move(xDir, yDir, out hit))
        {
            //Call RandomizeSfx of SoundManager to play the move sound, passing in two audio clips to choose from.
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

        //Since the player has moved and lost food points, check if the game has ended.
        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("PlayerChop");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if (other.tag == "Food")
        {
            food += pointsPerFood;
            foodText.text = "Food " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            food += pointsPerSoda;
            foodText.text = "Food " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    private void PeriodicFoodLoss()
    {
        food -= 1;
        foodText.text = "Food " + food;
        CheckIfGameOver();
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("PlayerHit");
        food -= loss;
        foodText.text = "Food " + food;
        CheckIfGameOver();
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    private void CheckIfGameOver()
    {
        if (food <= 0)
        {
            CancelInvoke("PeriodicFoodLoss");
            SoundManager.instance.RandomizeSfx(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }

    public void MovePlayer()
    {
        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (horizontal != 0) vertical = 0;

        if ((horizontal != 0) || (vertical != 0))
        {
            AttemptMove<Wall>(horizontal, vertical);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
