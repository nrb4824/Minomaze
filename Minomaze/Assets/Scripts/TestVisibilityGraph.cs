using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Minomaze.structs;

public class TestVisibilityGraph : MonoBehaviour
{
    // Instance of the visibility graph
    VisibilityGraph graph = new VisibilityGraph();

    
    public Vector2 endPoint = new Vector2(12, 6);
    public Vector2 startPoint = new Vector2(0, 0);

    // Colors for visualization
    public Color polygonColr = Color.red;
    public Color edgeColor = Color.blue;
    public Color startPointColor = Color.green;
    public Color endPointColor = Color.yellow;
    public Color pathColor = Color.magenta;

    private void Start()
    {
        InitializeVisibilityGraph();
    }

    /// <summary>
    /// Initialize the visibility graph with some polygons
    /// </summary>
    public void InitializeVisibilityGraph()
    {
        graph = new VisibilityGraph();
        Vector2[] square = new Vector2[]
        {
            new Vector2(1,1),
            new Vector2(4,1),
            new Vector2(4,4),
            new Vector2(1,4)
        };
        graph.AddPolygon(square);

        Vector2[] triangle = new Vector2[]
        {
            new Vector2(5,5),
            new Vector2(7,5),
            new Vector2(6,8)
        };
        graph.AddPolygon(triangle);

        Vector2[] polygon = new Vector2[]
        {
            new Vector2(8,1),
            new Vector2(10,2),
            new Vector2(11,4),
            new Vector2(9,5),
            new Vector2(7,3)
        };
        graph.AddPolygon(polygon);

        graph.vertices.Add(new Node(startPoint, endPoint, true, false));
        graph.vertices.Add(new Node(endPoint, endPoint, false, true));
        graph.startPosition = startPoint;
        graph.endPosition = endPoint;

        graph.CreateVisibilityGraph();
    }

    /// <summary>
    /// Draw the polygons and visibility edges.
    /// </summary>
    private void OnDrawGizmos()
    {
        if (graph == null) InitializeVisibilityGraph();

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
    }

    /// <summary>
    /// Regenerate the visibility graph when the script is changed
    /// </summary>
    private void OnValidate()
    {
        InitializeVisibilityGraph();
    }
}
