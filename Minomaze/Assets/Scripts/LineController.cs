using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] points = new Transform[2];
    private Color color;

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    public void SetUpLine(Transform[] points, Color color)
    {
        lr.startColor = color;
        lr.endColor = color;
        lr.positionCount = points.Length;
        this.points = points;
        for (int i = 0; i < points.Length; i++)
        {
            lr.SetPosition(i, points[i].position);
        }
    }
}
