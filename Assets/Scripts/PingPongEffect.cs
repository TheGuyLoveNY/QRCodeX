using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPongEffect : MonoBehaviour
{
    private Vector2 startingPosition;
    public float speed = 2f;
    public float distance = 5f;

    private void Start()
    {
        startingPosition = transform.position;
    }


    private void Update()
    {
        transform.position = startingPosition + new Vector2(0f, Mathf.Sin(Time.time * speed) * distance);
    }


}
