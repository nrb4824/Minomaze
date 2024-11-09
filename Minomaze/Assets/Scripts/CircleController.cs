using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleController : MonoBehaviour
{
    private SpriteRenderer sr;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetUpCircle(Transform pos, Color color)
    {
        sr.color = color;
        transform.position = pos.position;
    }
}
