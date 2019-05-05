using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float levelStartDelay;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints;
    [HideInInspector] public bool playersTurn = true;

    private int level = 1;
    private Text levelText;
    private GameObject levelImage;
    private bool doingSetup;
    private List<Enemy> enemyList = new List<Enemy>();
    private List<Player> playerList = new List<Player>();

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();

        InitGame();
    }

    void InitGame()
    {
        Debug.Log("InitGame");
        doingSetup = true;
        playerList.Clear();
        enemyList.Clear();
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Day " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);
        boardScript.SetupScene(level);
    }

    private void HideLevelImage()
    {
        Debug.Log("HideImage");
        levelImage.SetActive(false);
        doingSetup = false;
    }

    //this is called only once, and the paramter tell it to be called only after the scene was loaded
    //(otherwise, our Scene Load callback would be called the very first load, and we don't want that)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static public void CallbackInitialization()
    {
        Debug.Log("Scene Loaded");
        //register the callback to be called everytime the scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    //This is called each time a scene is loaded.
    static private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Debug.Log("Scene Loaded");
        instance.level++;
        instance.InitGame();
    }

    public void GameOver()
    {
        levelText.text = "LOL Loss";
        levelImage.SetActive(true);
        enabled = false;
    }

    public void RegisterPlayer(Player player)
    {
        playerList.Add(player);
    }

    public void RegisterEnemy(Enemy enemy)
    {
        enemyList.Add(enemy);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!doingSetup)
        {
            playerList.ForEach((player) =>
            {
                player.MovePlayer();
            });

            enemyList.ForEach((enemy) =>
            {
                enemy.MoveEnemy();
            });
        }
    }
}
