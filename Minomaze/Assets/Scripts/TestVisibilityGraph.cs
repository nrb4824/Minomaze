using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Minomaze.Structs;
using UnityEngine.UI;
using TMPro;

public class TestVisibilityGraph : MonoBehaviour
{
    // Instance of the visibility graph
    VisibilityGraph graph = new VisibilityGraph();

    // LineRenderer for drawing the visibility graph
    public GameObject lr;
    public GameObject circle;

    public Vector2 startPoint;
    public Vector2 endPoint;

    // Drawing variables
    public Vector3 screenPosition;
    public Vector3 worldPosition;
    public bool draw = false;
    public bool drawStart = false;
    public bool drawEnd = false;
    public List<Vector3> verticies = new List<Vector3>();
    public Button drawButton;
    public Button drawEndButton;
    public Button drawStartButton;
    public TMP_Text score;

    public List<GameObject> edges = new List<GameObject>();
    public List<GameObject> polygons = new List<GameObject>();
    public List<GameObject> circles = new List<GameObject>();
    public List<GameObject> tempCircles = new List<GameObject>();
    public bool isClicking = false;
 

    // Colors for visualization
    public Color polygonColr = Color.red;
    public Color edgeColor = Color.blue;
    public Color startPointColor = Color.green;
    public Color endPointColor = Color.yellow;
    public Color pathColor = Color.magenta;


    private void Start()
    {
        graph.Vertices.Add(new Node(startPoint, endPoint, true, false));
        graph.Vertices.Add(new Node(endPoint, endPoint, false, true));
        graph.SetStart(startPoint);
        graph.SetEnd(endPoint);
        updateDraw();

        //InitializeVisibilityGraph();
    }


    #region Drawing
    void Update()
    {
        screenPosition = Input.mousePosition;
        screenPosition.z = Camera.main.nearClipPlane + 1;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        transform.position = worldPosition;

        if (draw)
        {
            if (Input.GetMouseButtonDown(0) && !isClicking)
            {
                isClicking = true;
                verticies.Add(worldPosition);
                tempCircles.Add(drawCircle(worldPosition, polygonColr));
            }
        }
        else if (drawStart)
        {
            if (Input.GetMouseButtonDown(0) && !isClicking)
            {
                isClicking = true;
                startPoint = worldPosition;
                graph.SetStart(worldPosition);
                drawStart = !drawStart;
                drawStartButton.GetComponent<Image>().color = drawStart ? Color.green : Color.red;
                clearLists();
                graph.CreateVisibilityGraph();
                updateDraw();
            }
        }
        else if(drawEnd)
        {
            if (Input.GetMouseButtonDown(0) && !isClicking)
            {
                isClicking = true;
                endPoint = worldPosition;
                graph.SetEnd(worldPosition);
                drawEnd = !drawEnd;
                drawEndButton.GetComponent<Image>().color = drawEnd ? Color.green : Color.red;
                clearLists();
                graph.CreateVisibilityGraph();
                updateDraw();
            }
        }
        else
        {
            if (verticies.Count > 0)
            {
                CreatePoly(verticies);
                foreach (GameObject circle in tempCircles)
                {
                    Destroy(circle);
                }
                tempCircles.Clear();
                verticies.Clear();
            }
        }
        if (Input.GetMouseButtonUp(0)) isClicking = false;
    }

    private Transform[] convertVector2ToTransform(Vector2 p, Vector2 q, float z = 0)
    {
        GameObject temp = new GameObject();
        GameObject temp2 = new GameObject();
        if (z != 0.0)
        {
            temp.transform.position = new Vector3(p.x, p.y, z);
            temp2.transform.position = new Vector3(q.x, q.y, z);
        }
        else
        {
            temp.transform.position = new Vector3(p.x, p.y, 0);
            temp2.transform.position = new Vector3(q.x, q.y, 0);
        }
        Transform[] points = new Transform[] { temp.transform, temp2.transform };
        Destroy(temp);
        Destroy(temp2);
        return points;
    }

    public GameObject drawLine(Vector2 p, Vector2 q, Color color, float z = 0)
    {
        GameObject temp = Instantiate(lr);
        temp.GetComponent<LineController>().SetUpLine(convertVector2ToTransform(p, q, z), color);
        return temp;
    }

    public GameObject drawCircle(Vector2 p, Color color, float z = -3)
    {
        GameObject temp = Instantiate(circle);
        GameObject pTemp = new GameObject();
        pTemp.transform.position = new Vector3(p.x, p.y, z);
        temp.GetComponent<CircleController>().SetUpCircle(pTemp.transform, color);
        Destroy(pTemp);
        return temp;
    }

    public void setDraw()
    {
        draw = !draw;
        drawButton.GetComponent<Image>().color = draw ? Color.green : Color.red;
    }

    public void setStartPoint()
    {
        drawStart = !drawStart;
        drawStartButton.GetComponent<Image>().color = drawStart ? Color.green : Color.red;
    }

    public void setEndPoint()
    {
        drawEnd = !drawEnd;
        drawEndButton.GetComponent<Image>().color = drawEnd ? Color.green : Color.red;
    }

    private void clearLists()
    {
        foreach (GameObject edge in edges)
        {
            Destroy(edge);
        }
        edges.Clear();
        foreach (GameObject poly in polygons)
        {
            Destroy(poly);
        }
        polygons.Clear();
        foreach (GameObject circle in circles)
        {
            Destroy(circle);
        }
        
        circles.Clear();
        graph.ClearLists();
    }

    public void CreatePoly(List<Vector3> verticies)
    {
        verticies.RemoveAt(verticies.Count - 1);
        if (verticies.Count > 2)
        {
            Vector2[] poly = new Vector2[verticies.Count];
            for (int i = 0; i < verticies.Count; i++)
            {
                poly[i] = new Vector2(verticies[i].x, verticies[i].y);
            }
            clearLists();

            graph.AddPolygon(poly);
            graph.CreateVisibilityGraph();
            updateDraw();
        }
    }

    private void updateDraw()
    {
        if (graph == null) InitializeVisibilityGraph();

        score.text = "Length: " + Mathf.Round(graph.PathScore * 10);

        // Draw Visibility Edges
        // Gizmos.color = edgeColor;
        foreach (var edge in graph.Edges)
        {
            Vector3 start = new Vector3(edge.Start.x, edge.Start.y, 0);
            Vector3 end = new Vector3(edge.End.x, edge.End.y, 0);
            edges.Add(drawLine(edge.Start, edge.End, edgeColor));
        }

        // Draw Polygons
        //Gizmos.color = polygonColr;
        foreach (var polygon in graph.Polygons)
        {
            int vertexCount = polygon.Length;
            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 current = new Vector2(polygon[i].x, polygon[i].y);
                Vector2 next = new Vector2(polygon[(i + 1) % vertexCount].x, polygon[(i + 1) % vertexCount].y);
                polygons.Add(drawLine(current, next, polygonColr, -2));

                // Draw vertices as small spheres for better visualization
                circles.Add(drawCircle(current, polygonColr));
            }
        }

        circles.Add(drawCircle(startPoint, startPointColor));
        circles.Add(drawCircle(endPoint, endPointColor));

        // If the start and end points are visible, draw the path
        if (graph.IsVisible(startPoint, endPoint))
        {
            edges.Add(drawLine(startPoint, endPoint, pathColor));
        }

        // Draw Path
        foreach (var edge in graph.GetPathEdges())
        {
            Vector3 start = new Vector3(edge.Start.x, edge.Start.y, 0);
            Vector3 end = new Vector3(edge.End.x, edge.End.y, 0);
            edges.Add(drawLine(edge.Start, edge.End, pathColor, -1));
        }
    }
    #endregion

    /// <summary>
    /// Initialize the visibility graph with some polygons
    /// </summary>
    public void InitializeVisibilityGraph()
    {
        graph = new VisibilityGraph();
        Vector2[] square = new Vector2[]
        {
            new Vector2(4,1),
            new Vector2(4,4),
            new Vector2(2,2),
            new Vector2(2,1)
        };
        graph.AddPolygon(square);

        Vector2[] square2 = new Vector2[]
        {
            new Vector2(8,2),
            new Vector2(8,8),
            new Vector2(5,5),
            new Vector2(5,1)
        };
        graph.AddPolygon(square2);

        graph.Vertices.Add(new Node(startPoint, endPoint, true, false));
        graph.Vertices.Add(new Node(endPoint, endPoint, false, true));
        graph.SetStart(startPoint);
        graph.SetEnd(endPoint);

        graph.CreateVisibilityGraph();
    }
}
