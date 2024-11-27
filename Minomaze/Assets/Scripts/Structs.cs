using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minomaze.Structs
{
    public struct Node
    {
        // Public properties
        public Vector2 Position { get; set; }
        public Vector2 Goal { get; set; }
        public Vector2 NearestToStart { get; set; }
        public List<Node> Neighbors { get; set; }
        public float Heuristic { get; set; }
        public float MinCostToStart { get; set; }
        
        // Public flags
        public bool IsStart { get; set; }
        public bool IsEnd { get; set; }
        public bool IsVisited { get; set; }

        public Node(Vector2 position, Vector2 goal, bool isStart = false, bool isEnd = false)
        {
            Position = position;
            Goal = goal;
            Neighbors = new List<Node>();
            Heuristic = Vector2.Distance(position, goal);
            IsStart = isStart;
            IsEnd = isEnd;
            MinCostToStart = float.MaxValue;
            IsVisited = false;
            NearestToStart = Vector2.zero;
        }
    }

    public struct Edge
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        public Edge(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
        }
    }
}