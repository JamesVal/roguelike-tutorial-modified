using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [System.Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count(int min, int max)
        {
            this.minimum = min;
            this.maximum = max;
        }
    }

    public int columns = 24;
    public int rows = 24;
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    void InitializeList()
    {
        gridPositions.Clear();

        for (int x = 1; x < columns-1; x++)
            for (int y = 1; y < rows-1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }       
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns + 1; x++)
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                if ((x == -1) || (y == -1) || (x == columns) || (y == rows)) {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
    }

    Vector3 RandomPosition()
    {
        int randomIdx = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIdx];
        gridPositions.RemoveAt(randomIdx);
        return randomPosition;
    }

    void LayoutDebugging(GameObject newObject, Vector3 curPos)
    {
        Instantiate(newObject, curPos, Quaternion.identity);
    }

    void LayoutObjectAtRandom(GameObject[] objectArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for (int i = 0; i < objectCount; i++)
        {
            Vector3 curPos = RandomPosition();
            GameObject curObj = objectArray[Random.Range(0, objectArray.Length)];
            Instantiate(curObj, curPos, Quaternion.identity);
        }
    }

    public void SetupScene(int Level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(Level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
        /*
        // DEVELOPING 
        LayoutDebugging(enemyTiles[0], new Vector3(8f, 8f, 0f));
        LayoutDebugging(enemyTiles[0], new Vector3(7f, 8f, 0f));
        LayoutDebugging(enemyTiles[0], new Vector3(6f, 8f, 0f));
        LayoutDebugging(enemyTiles[0], new Vector3(5f, 8f, 0f));
        LayoutDebugging(enemyTiles[0], new Vector3(4f, 8f, 0f));
        LayoutDebugging(wallTiles[0], new Vector3(0f, 1f, 0f));
        LayoutDebugging(wallTiles[0], new Vector3(2f, 1f, 0f));
        LayoutDebugging(wallTiles[0], new Vector3(0f, 2f, 0f));
        LayoutDebugging(wallTiles[0], new Vector3(2f, 2f, 0f));
        LayoutDebugging(wallTiles[0], new Vector3(0f, 3f, 0f));
        LayoutDebugging(wallTiles[0], new Vector3(0f, 4f, 0f));
        LayoutDebugging(wallTiles[0], new Vector3(1f, 4f, 0f));
        LayoutDebugging(wallTiles[0], new Vector3(2f, 4f, 0f));
        */
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
