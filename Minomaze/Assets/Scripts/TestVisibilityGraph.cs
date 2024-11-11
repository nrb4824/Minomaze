using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Minomaze.structs;
using UnityEngine.UI;

public class TestVisibilityGraph : MonoBehaviour
{
    // Instance of the visibility graph
    VisibilityGraph graph = new VisibilityGraph();

    // LineRenderer for drawing the visibility graph
    public GameObject lr;
    public GameObject circle;


    public Vector2 endPoint;
    public Vector2 startPoint;

    // Drawing variables
    public Vector3 screenPosition;
    public Vector3 worldPosition;
    public bool draw = false;
    public List<Vector3> verticies = new List<Vector3>();
    public Button drawButton;
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
        graph.vertices.Add(new Node(startPoint, endPoint, true, false));
        graph.vertices.Add(new Node(endPoint, endPoint, false, true));
        graph.startPosition = startPoint;
        graph.endPosition = endPoint;
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

    public GameObject drawCircle(Vector2 p, Color color)
    {
        GameObject temp = Instantiate(circle);
        GameObject pTemp = new GameObject();
        pTemp.transform.position = new Vector3(p.x, p.y, 0);
        temp.GetComponent<CircleController>().SetUpCircle(pTemp.transform, color);
        Destroy(pTemp);
        return temp;
    }

    public void setDraw()
    {
        draw = !draw;
        drawButton.GetComponent<Image>().color = draw ? Color.green : Color.red;
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

            graph.edges.Clear();

            graph.AddPolygon(poly);
            graph.CreateVisibilityGraph();
            updateDraw();
        }
    }

    private void updateDraw()
    {
        if (graph == null) InitializeVisibilityGraph();

        // Draw Visibility Edges
        // Gizmos.color = edgeColor;
        foreach (var edge in graph.edges)
        {
            Vector3 start = new Vector3(edge.Start.x, edge.Start.y, 0);
            Vector3 end = new Vector3(edge.End.x, edge.End.y, 0);
            edges.Add(drawLine(edge.Start, edge.End, edgeColor));
        }

        // Draw Polygons
        //Gizmos.color = polygonColr;
        foreach (var polygon in graph.polygons)
        {
            int vertexCount = polygon.Length;
            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 current = new Vector2(polygon[i].x, polygon[i].y);
                Vector2 next = new Vector2(polygon[(i + 1) % vertexCount].x, polygon[(i + 1) % vertexCount].y);
                polygons.Add(drawLine(current, next, polygonColr, -1));

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

        graph.vertices.Add(new Node(startPoint, endPoint, true, false));
        graph.vertices.Add(new Node(endPoint, endPoint, false, true));
        graph.startPosition = startPoint;
        graph.endPosition = endPoint;

        graph.CreateVisibilityGraph();
    }

    /// <summary>
    /// Draw the polygons and visibility edges.
    /// </summary>
/*    private void OnDrawGizmos()
    {
        //if (graph == null) InitializeVisibilityGraph();

        // Draw Visibility Edges
        Gizmos.color = edgeColor;
        foreach (var edge in graph.edges)
        {
            Vector3 start = new Vector3(edge.Start.x, edge.Start.y, 0);
            Vector3 end = new Vector3(edge.End.x, edge.End.y, 0);
            Gizmos.DrawLine(start, end);
        }

        // Draw Polygons
        Gizmos.color = polygonColr;
        foreach (var polygon in graph.polygons)
        {
            int vertexCount = polygon.Length;
            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 current = new Vector3(polygon[i].x, polygon[i].y, 0);
                Vector3 next = new Vector3(polygon[(i + 1) % vertexCount].x, polygon[(i + 1) % vertexCount].y, 0);
                Gizmos.DrawLine(current, next);

                // Draw vertices as small spheres for better visualization
                Gizmos.DrawSphere(current, 0.1f);
            }
        }

        Gizmos.color = startPointColor;
        Gizmos.DrawSphere(new Vector3(startPoint.x, startPoint.y, 0), 0.2f);
        Gizmos.color = endPointColor;
        Gizmos.DrawSphere(new Vector3(endPoint.x, endPoint.y, 0), 0.2f);

        // If the start and end points are visible, draw the path
        if (graph.IsVisible(startPoint, endPoint))
        {
            Gizmos.color = pathColor;
            Gizmos.DrawLine(new Vector3(startPoint.x, startPoint.y, 0), new Vector3(endPoint.x, endPoint.y, 0));
        }
    }*/


    /// <summary>
    /// Regenerate the visibility graph when the script is changed
    /// </summary>
/*    private void OnValidate()
    {
        InitializeVisibilityGraph();
    }*/
}
