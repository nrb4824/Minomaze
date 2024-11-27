using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Minomaze.Structs;

public class VisibilityGraph
{
    // Private fields should use camelCase with underscore prefix
    private List<Node> _vertices;
    private List<Edge> _edges;
    private List<Edge> _pathEdges;
    private List<Vector2[]> _polygons;
    private Vector2 _startPosition;
    private Vector2 _endPosition;
    private float _pathScore;

    // Public properties should use PascalCase
    public List<Node> Vertices => _vertices;
    public List<Edge> Edges => _edges;
    public List<Edge> PathEdges => _pathEdges;
    public List<Vector2[]> Polygons => _polygons;
    public float PathScore => _pathScore;

    // Constructor
    public VisibilityGraph()
    {
        _vertices = new List<Node>();
        _edges = new List<Edge>();
        _polygons = new List<Vector2[]>();
        _pathEdges = new List<Edge>();
    }

    public void SetStart(Vector2 newStart)
    {
        _vertices.Remove(_vertices.Find(x => x.IsStart));
        _vertices.Add(new Node(newStart, _endPosition, true, false));
        _startPosition = newStart;
    }

    public void SetEnd(Vector2 newEnd)
    {
        _vertices.Remove(_vertices.Find(x => x.IsEnd));
        _vertices.Add(new Node(newEnd, _endPosition, false, true));
        _endPosition = newEnd;
    }

    public void ClearLists(){
        _pathEdges.Clear();
        _edges.Clear();
        // clear neighbors list in all vertices
        foreach (var vertex in _vertices){
            vertex.Neighbors.Clear();
        }
    }

    public List<Edge> GetPathEdges(){
        return _pathEdges;
    }


    /// <summary>
    /// Adds a polygon's vertices to the visibility graph
    /// </summary>
    /// <param name="polygon">Array of Vector2 points defining the polygon</param>
    public void AddPolygon(Vector2[] polygon)
    {
        // Add each vertex as a node
        foreach (Vector2 vertex in polygon)
        {
            _vertices.Add(new Node(vertex, _endPosition));
        }
        _polygons.Add(polygon);
    }

    /// <summary>
    /// Adds all polygon edges to the visibility graph
    /// </summary>
    private void AddPolygonEdges()
    {
        foreach (Vector2[] polygon in _polygons)
        {
            int vertexCount = polygon.Length;
            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 startPoint = polygon[i];
                Vector2 endPoint = polygon[(i + 1) % vertexCount];
                _edges.Add(new Edge(startPoint, endPoint));
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
        foreach (var polygon in _polygons)
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
    /// Check if a line segment is non-adjacent edge of the same polygon
    /// </summary>
    /// <param name="p1"></param> Start point of the line segment
    /// <param name="p2"></param> End point of the line segment
    /// <returns></returns> True if the line segment is non-adjacent edge of the same polygon, false otherwise
    public bool IsNonAdjacentEdgeOfSamePolygon(Node p1, Node p2)
    {
        foreach (var polygon in _polygons)
        {
            int vertexCount = polygon.Length;

            int p1Index = -1;
            int p2Index = -1;

            for (int i = 0; i < vertexCount; i++)
            {
                if (polygon[i] == p1.Position)
                {
                    p1Index = i;
                }
                if (polygon[i] == p2.Position)
                {
                    p2Index = i;
                }
            }
            if (p1Index != -1 && p2Index != -1)
            {
                if (p2Index-p1Index != 1 && p1Index-p2Index != 1)
                {
                    return true;
                }
            }
        }
        return false;
    }


    /// <summary>
    /// Check if an edge is already in the graph
    /// </summary>
    /// <param name="p1"></param> Start point of the edge
    /// <param name="p2"></param> End point of the edge
    /// <returns></returns> True if the edge is already in the graph, false otherwise
    private bool IsEdgeInGraph(Node p1, Node p2)
    {
        foreach(var edge in _edges)
        {
            if ((edge.Start == p1.Position && edge.End == p2.Position) || (edge.Start == p2.Position && edge.End == p1.Position))
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
        for (int i = 0; i < _vertices.Count; i++)
        {
            for (int j = i + 1; j < _vertices.Count; j++)
            {
                // Skip if the edge is already in the graph
                if (!IsEdgeInGraph(_vertices[i], _vertices[j]) && !IsNonAdjacentEdgeOfSamePolygon(_vertices[i], _vertices[j]))
                {
                    // If _vertices[i] and _vertices[j] are visible, add an edge between them
                    if (IsVisible(_vertices[i].Position, _vertices[j].Position))
                    {
                        _edges.Add(new Edge(_vertices[i].Position, _vertices[j].Position));
                        _vertices[i].Neighbors.Add(_vertices[j]);
                        _vertices[j].Neighbors.Add(_vertices[i]);
                    }
                }  
            }
        }

        Dictionary<Vector2, Vector2> nearestToStart = AStarSearch();
        _pathScore = AStarPath(nearestToStart);
        
    }

    public Dictionary<Vector2, Vector2> AStarSearch(){
        // return the shortest path from the start to the end
        var prioQueue = new List<Node>();
        List<Node> visitedNodes = new List<Node>();
        Dictionary<Vector2, float> distances = new Dictionary<Vector2, float>();
        Dictionary<Vector2, Vector2> nearestToStart = new Dictionary<Vector2, Vector2>();

        foreach (var node in _vertices){
            distances[node.Position] = float.MaxValue;
        }

        Node Start = _vertices.Find(x => x.IsStart);
        Node End = _vertices.Find(x => x.IsEnd);

        distances[Start.Position] = 0;
        prioQueue.Add(Start);
        do {
            // sort prioQueue by distances[node.position] + node.heuristic
            prioQueue = prioQueue.OrderBy(x => distances[x.Position]).ThenBy(x => x.Heuristic).ToList();

            var node = prioQueue.First();
            prioQueue.Remove(node);

            if (node.IsEnd){
                break;
            }

            foreach (var cnn in node.Neighbors)
            {
                float successorCurrentCost = distances[node.Position] + node.Heuristic;
                // if cnn is in the open list
                if (prioQueue.Contains(cnn)){
                    if (distances[cnn.Position] <= successorCurrentCost){
                        continue;
                    }
                }else if (visitedNodes.Contains(cnn)){
                    if (distances[cnn.Position] <= successorCurrentCost){
                        continue;
                    }
                    // move from visitedNodes to prioQueue
                    prioQueue.Add(cnn);
                    visitedNodes.Remove(cnn);
                }else{
                    prioQueue.Add(cnn);
                }
                distances[cnn.Position] = distances[node.Position] + Vector2.Distance(node.Position, cnn.Position);
                nearestToStart[cnn.Position] = node.Position;
            }
            visitedNodes.Add(node);
        } while (prioQueue.Count > 0);  
        return nearestToStart;
    }

    public float AStarPath(Dictionary<Vector2, Vector2> nearestToStart){

        // find path based on NearestToStart
        Node Start = _vertices.Find(x => x.IsStart);
        Node End = _vertices.Find(x => x.IsEnd);
        Vector2 current = End.Position;
        float score = 0;

        int loopCount = 0;
        while (current != Start.Position){
            Edge newEdge = new Edge(current, nearestToStart[current]);
            _pathEdges.Add(newEdge);
            current = nearestToStart[current];
            score += Vector2.Distance(newEdge.Start, newEdge.End);

            loopCount ++;
            if (loopCount > 10){
                break;
            }
        }
        return score;
    }
}
