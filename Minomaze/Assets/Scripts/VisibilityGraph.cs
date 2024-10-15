using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class VisibilityGraph
{
    public struct Edge
    {
        public Vector2 Start;
        public Vector2 End;
        public Edge(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
    }

    // List of vertices in the environment
    public List<Vector2> vertices;
    // List of edges in the graph
    public List<Edge> edges;
    // List to store all polygons in the enviroment
    public List<Vector2[]> polygons;

    // Constructor
    public VisibilityGraph()
    {
        vertices = new List<Vector2>();
        edges = new List<Edge>();
        polygons = new List<Vector2[]>();
    }

    // Add the verticies of a polygon to the graph
    public void AddPolygon(Vector2[] polygon)
    {
        vertices.AddRange(polygon);
        polygons.Add(polygon);
    }

    // Adds all edges of polygons to the visibility graph.
    private void AddPolygonEdges()
    {
        foreach (var polygon in polygons)
        {
            int vertexCount = polygon.Length;
            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 p1 = polygon[i];
                Vector2 p2 = polygon[(i + 1) % vertexCount];
                edges.Add(new Edge(p1, p2));
            }
        }
    }

    /// <summary>
    /// Check if a line segment is visible in the environment
    /// </summary>
    /// <param name="p1"></param> Start point of the line segment
    /// <param name="p2"></param> End point of the line segment
    /// <returns></returns> True if the line segment is visible, false otherwise
    public bool IsVisible(Vector2 p1, Vector2 p2)
    {
        LineSegment visibilityLine = new LineSegment(p1, p2);

        // Iterate through all polygon edges to check for intersections
        foreach (var polygon in polygons)
        {
            int vertexCount = polygon.Length;
            for (int i = 0; i < vertexCount; i++)
            {
                // Define the current edge of the polygon
                Vector2 p3 = polygon[i];
                Vector2 p4 = polygon[(i + 1) % vertexCount];
                LineSegment polygonEdge = new LineSegment(p3, p4);

                // Check if the visibility line intersects the polygon edge
                if (visibilityLine.Intersects(polygonEdge))
                {
                    // To avoid considering adjacent edges of the same polygon as obstructions, check if the endpoints are shared
                    if(!visibilityLine.SharesEndpoint(polygonEdge))
                    {
                        // if there is an intersection it's not at a shared endpoint, visibility is blocked.
                        return false;
                    }
                }
            }
        }
        // If no intersections were found, the line is visible
        return true;
    }

    /// <summary>
    /// Check if an edge is already in the graph
    /// </summary>
    /// <param name="p1"></param> Start point of the edge
    /// <param name="p2"></param> End point of the edge
    /// <returns></returns> True if the edge is already in the graph, false otherwise
    private bool IsEdgeInGraph(Vector2 p1, Vector2 p2)
    {
        foreach(var edge in edges)
        {
            if ((edge.Start == p1 && edge.End == p2) || (edge.Start == p2 && edge.End == p1))
            {
                return true;
            }
        }
        return false;
    }

    // Generate the visibility graph by connecting visible vertices with edges
    public void CreateVisibilityGraph()
    {
        // Loop through every pair of vertices to check visibility
        for (int i = 0; i < vertices.Count; i++)
        {
            for (int j = i + 1; j < vertices.Count; j++)
            {
                // Skip if the edge is already in the graph
                if (!IsEdgeInGraph(vertices[i], vertices[j]))
                {
                    // If vertices[i] and vertices[j] are visible, add an edge between them
                    if (IsVisible(vertices[i], vertices[j]))
                    {
                        edges.Add(new Edge(vertices[i], vertices[j]));
                    }
                }
                
            }
        }
    }
}
