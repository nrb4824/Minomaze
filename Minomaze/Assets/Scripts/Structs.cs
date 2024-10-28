using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Minomaze.structs{
    public struct Node
    {
        public Vector2 position;
        public List<Node> neighbors;

        public float heuristic;

        public bool isStart;
        public bool isEnd;

        public Node(Vector2 pos, Vector2 goal, bool start = false, bool end = false)
        {
            position = pos;
            neighbors = new List<Node>();
            heuristic = Vector2.Distance(pos, goal);
            isStart = start;
            isEnd = end;
        }
    }
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
}