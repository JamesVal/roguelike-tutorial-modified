using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public List<Node> possiblePaths = new List<Node>();
    public Vector2 m_origin;
    public int levelFromStart = -99;

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

    public void printPaths()
    {
        String printedStr = "";

        //Debug.Log("Origin: " + m_origin.x + "," + m_origin.y);
        //Debug.Log("Connected to: ");
        for (int i = 0; i < possiblePaths.Count; i++)
        {
            Vector2 connectedOrigin = possiblePaths[i].getOrigin();
            printedStr += "(" + connectedOrigin.x + "," + connectedOrigin.y + ")";
            
        }
        //Debug.Log(printedStr);
    }
}

public class PathFinder : MonoBehaviour
{
    private LayerMask blockingLayer;
    private List<Node> allNodes = new List<Node>();
    private List<Node> pathToEnd = new List<Node>();
    private Node startNode;
    private Node endNode;
    private Transform target;
    private LineRenderer pathRenderer;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider;

    public void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        pathRenderer = gameObject.AddComponent<LineRenderer>();
        blockingLayer = LayerMask.GetMask("BlockingLayer");
        boxCollider = GetComponent<BoxCollider2D>();

        pathRenderer.material = new Material(Shader.Find("Sprites/Default"));
        pathRenderer.sortingLayerName = spriteRenderer.sortingLayerName;
        pathRenderer.positionCount = 2;
        pathRenderer.widthMultiplier = 0.2f;

        GenerateGrid(24, 24);
        ConnectNodes();
    }

    void GenerateGrid(int rows, int cols)
    {
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < cols; y++)
            {
                Node newNode = new Node(x, y);
                allNodes.Add(newNode);
            }
        }
    }

    public void ClearNodeLevels()
    {
        for (int i = 0; i < allNodes.Count; i++)
        {
            Node curNode = allNodes[i];
            curNode.levelFromStart = -99;
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
                    if (((Math.Abs(curOrigin.x - innerOrigin.x) == 1) && (Math.Abs(curOrigin.y - innerOrigin.y) == 0)) ||
                      ((Math.Abs(curOrigin.y - innerOrigin.y) == 1) && (Math.Abs(curOrigin.x - innerOrigin.x) == 0)))
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

    void SetStartEnd(Vector2 startPosition, Vector2 endPosition)
    {
        startNode = null;
        endNode = null;
        for (int eachNodeIdx = 0; eachNodeIdx < allNodes.Count; eachNodeIdx++)
        {
            Node curNode = allNodes[eachNodeIdx];

            if ((Math.Abs(startPosition.x - curNode.m_origin.x) <= .5f) &&
                (Math.Abs(startPosition.y - curNode.m_origin.y) <= .5f))
            {
                startNode = curNode;
            }
            
            if ((Math.Abs(endPosition.x - curNode.m_origin.x) <= .5f) &&
                (Math.Abs(endPosition.y - curNode.m_origin.y) <= .5f))
            {
                endNode = curNode;
            }
            if ((startNode != null) && (endNode != null)) break;
        }
    }

    bool TraversePath(List<Node> currentNodes, int curDistance)
    {
        List<Node> nextNodes = new List<Node>();
        RaycastHit2D hit;

        for (int eachNodeIdx = 0; eachNodeIdx < currentNodes.Count; eachNodeIdx++)
        {
            Node curNode = currentNodes[eachNodeIdx];
            curNode.levelFromStart = curDistance;

            if ((curNode.m_origin.x == endNode.m_origin.x) && (curNode.m_origin.y == endNode.m_origin.y))
                return true;

            for (int eachInnerNodeIdx = 0; eachInnerNodeIdx < curNode.possiblePaths.Count; eachInnerNodeIdx++)
            {
                Node innerNode = curNode.possiblePaths[eachInnerNodeIdx];

                if ((innerNode.levelFromStart == -99) && !(nextNodes.Contains(innerNode)))
                {
                    boxCollider.enabled = false;
                    hit = Physics2D.Linecast(innerNode.m_origin, innerNode.m_origin, blockingLayer);
                    boxCollider.enabled = true;
                    if ((hit.transform == null) || hit.transform.GetComponent<Player>())
                    nextNodes.Add(innerNode);
                }
            }
        }

        if (nextNodes.Count > 0) return TraversePath(nextNodes, curDistance + 1);

        return false;
    }

    double CalcDistance(Node startNode, Node endNode)
    {
        return Math.Sqrt(Math.Pow(Math.Abs(startNode.m_origin.x - endNode.m_origin.x), 2) + Math.Pow(Math.Abs(startNode.m_origin.y - endNode.m_origin.y), 2));
    }

    public void BuildPath(List<Node> pathToBuild, Node currentNode)
    {
        pathToBuild.Insert(0, currentNode);
        List<Node> nextNodes = new List<Node>();
        Node nodeToAdd;
        double curDistance;

        for (int eachNodeIdx = 0; eachNodeIdx < currentNode.possiblePaths.Count; eachNodeIdx++)
        {
            Node nextNode = currentNode.possiblePaths[eachNodeIdx];
            if ((nextNode.levelFromStart == (currentNode.levelFromStart - 1)) && !pathToBuild.Contains(nextNode))
                nextNodes.Add(nextNode);
        }

        if (nextNodes.Count > 0)
        {
            curDistance = CalcDistance(currentNode, nextNodes[0]);
            nodeToAdd = nextNodes[0];

            for (int eachNodeIdx = 1; eachNodeIdx < nextNodes.Count; eachNodeIdx++)
            {
                Node nextNode = nextNodes[eachNodeIdx];
                double nextDistance = CalcDistance(currentNode, nextNode);
                if (nextDistance < curDistance)
                {
                    curDistance = nextDistance;
                    nodeToAdd = nextNode;
                }
            }

            BuildPath(pathToBuild, nodeToAdd);
        }
    }

    public void PrintPath(List<Node> pathToPrint)
    {
        Debug.Log("Current Path:");
        string pathStr = "";
        for (int eachNodeIdx = 0; eachNodeIdx < pathToPrint.Count; eachNodeIdx++)
        {
            Node curNode = pathToPrint[eachNodeIdx];
            pathStr += "(" + curNode.m_origin.x + "," + curNode.m_origin.y + ")";
        }
        Debug.Log(pathStr);
    }

    public Vector2 GetNextDirection()
    {
        Vector2 retVal = new Vector2();

        if (pathToEnd.Count >= 2)
        {
            if ((pathToEnd[1].m_origin.x - transform.position.x) > .2f) retVal.x = 1;
            else if ((pathToEnd[1].m_origin.x - transform.position.x) < (.2f * -1)) retVal.x = -1;
            else if ((pathToEnd[1].m_origin.y - transform.position.y) > .2f) retVal.y = 1;
            else if ((pathToEnd[1].m_origin.y - transform.position.y) < (.2f * -1)) retVal.y = -1;
        }
        else if (pathToEnd.Count == 1)
        {
            if ((target.position.x - transform.position.x) > .2f) retVal.x = 1;
            else if ((target.position.x - transform.position.x) < (.2f * -1)) retVal.x = -1;
            else if ((target.position.y - transform.position.y) > .2f) retVal.y = 1;
            else if ((target.position.y - transform.position.y) < (.2f * -1)) retVal.y = -1;
        }

        return retVal;
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        //InvokeRepeating("UpdatePath", 1f, 1f);
    }

    void UpdatePath()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        pathToEnd.Clear();
        SetStartEnd(transform.position, target.position);
        ClearNodeLevels();
        pathRenderer.positionCount = 0;

        if (TraversePath(new List<Node> { startNode }, 0))
        {
            BuildPath(pathToEnd, endNode);

            /*
            PrintPath(pathToEnd);
            pathRenderer.positionCount = pathToEnd.Count;
            if (pathToEnd.Count > 0)
            {
                for (int i = 0; i < pathToEnd.Count; i++)
                {
                    pathRenderer.SetPosition(i, pathToEnd[i].m_origin);
                }
            }
            */

        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePath();
    }
}
