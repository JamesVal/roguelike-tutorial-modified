using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public List<Node> possiblePaths = new List<Node>();
    public Vector2 m_origin;
    public int levelFromStart;

    float boxCollisionSize = 1f;
    bool isObstacle = false;

    public Node(float x, float y)
    {
        m_origin = new Vector2(x, y);
    }

    public Vector2 getOrigin()
    {
        return m_origin;
    }

    public void addPath(Node nodeToAdd)
    {
        if (!possiblePaths.Contains(nodeToAdd))
            possiblePaths.Add(nodeToAdd);
    }

    public void setObstacle(bool state)
    {
        isObstacle = state;
    }

    public bool getObstacle()
    {
        return isObstacle;
    }

    public void printPaths()
    {
        String printedStr = "";

        Debug.Log("Origin: " + m_origin.x + "," + m_origin.y);
        Debug.Log("Connected to: ");
        for (int i = 0; i < possiblePaths.Count; i++)
        {
            Vector2 connectedOrigin = possiblePaths[i].getOrigin();
            printedStr += "(" + connectedOrigin.x + "," + connectedOrigin.y + ")";
            
        }
        Debug.Log(printedStr);
    }
}

public class PathFinder : MonoBehaviour
{
    private List<Node> allNodes = new List<Node>();
    private List<Node> traversedNodes = new List<Node>();
    private int pathLength = 0;
    private Node startNode;
    private Node endNode;
    private Transform target;
    private LineRenderer pathRenderer;
    private SpriteRenderer spriteRenderer;

    public void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pathRenderer = gameObject.AddComponent<LineRenderer>();
                        
        pathRenderer.material = new Material(Shader.Find("Sprites/Default"));
        pathRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        pathRenderer.positionCount = 2;
        pathRenderer.widthMultiplier = 0.05f;

        GenerateGrid(8, 8);
        ConnectNodes();
    }

    void GenerateGrid(int rows, int cols)
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Node newNode = new Node(x, y);
                /* Simulate walls */
                //if ((x == 1 && y == 1) || (x == 2 && y == 1)) newNode.setObstacle(true);
                //if ((x == 0 && y == 1)) newNode.setObstacle(true);
                allNodes.Add(newNode);
            }
        }
    }

    void ConnectNodes()
    {
        for (int eachNodeIdx = 0; eachNodeIdx < allNodes.Count; eachNodeIdx++)
        {
            Node curNode = allNodes[eachNodeIdx];
            Vector2 curOrigin = allNodes[eachNodeIdx].getOrigin();
            for (int eachInnerNodeIdx = (eachNodeIdx + 1); eachInnerNodeIdx < allNodes.Count; eachInnerNodeIdx++)
            {
                if (eachNodeIdx != eachInnerNodeIdx)
                {
                    Node innerNode = allNodes[eachInnerNodeIdx];
                    Vector2 innerOrigin = allNodes[eachInnerNodeIdx].getOrigin();
                    if (((Math.Abs(curOrigin.x - innerOrigin.x) == 1f) && (Math.Abs(curOrigin.y - innerOrigin.y) <= 1f)) ||
                      ((Math.Abs(curOrigin.y - innerOrigin.y) == 1f) && (Math.Abs(curOrigin.x - innerOrigin.x) <= 1f)))
                    {
                        curNode.addPath(innerNode);
                        innerNode.addPath(curNode);
                    }
                }
            }
        }

        for (int eachNodeIdx = 0; eachNodeIdx < allNodes.Count; eachNodeIdx++)
        {
            Node curNode = allNodes[eachNodeIdx];
            curNode.printPaths();
        }
    }

    bool PositionInNode(Vector2 position)
    {

        return false;
    }

    void SetStartEnd(Vector2 startPosition, Vector2 endPosition)
    {
        for (int eachNodeIdx = 0; eachNodeIdx < allNodes.Count; eachNodeIdx++)
        {
            Node curNode = allNodes[eachNodeIdx];

            if (((startPosition.x >= curNode.m_origin.x) && (startPosition.x <= (curNode.m_origin.x + 1f))) &&
                ((startPosition.y >= curNode.m_origin.y) && (startPosition.y <= (curNode.m_origin.y + 1f))))
            {
                startNode = curNode;
            }
            if (((endPosition.x >= curNode.m_origin.x) && (endPosition.x <= (curNode.m_origin.x + 1f))) &&
                ((endPosition.y >= curNode.m_origin.y) && (endPosition.y <= (curNode.m_origin.y + 1f))))
            {
                endNode = curNode;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        SetStartEnd(transform.position, target.position);
        Debug.Log(target.position);
        pathRenderer.SetPosition(0, startNode.m_origin);
        pathRenderer.SetPosition(1, endNode.m_origin);
    }
}
