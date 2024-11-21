using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;

namespace Minomaze.structs{
    public struct Node
    {
        public Vector2 position;
        public Vector2 goal;

        public Vector2 NearestToStart;
        public List<Node> neighbors;

        public float heuristic;

        public bool isStart;
        public bool isEnd;
        public bool Visited;


        public float minCostToStart;

        public Node(Vector2 pos, Vector2 goal_, bool start = false, bool end = false)
        {
            position = pos;
            neighbors = new List<Node>();
            heuristic = Vector2.Distance(pos, goal_);
            goal = goal_;
            isStart = start;
            isEnd = end;
            minCostToStart = float.MaxValue;
            Visited = false;
            NearestToStart = Vector2.zero;
        }

        public Node returnCopy(){
            return new Node(position, goal, isStart, isEnd);
        }

        public void equals(Node n){
            // see if two nodes are equal
            if (n.position == position){
                return;
            }
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