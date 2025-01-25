using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    [SerializeField]
    bool bounceMode = false;
    //Vector2 originalPos = this.transform.position;
    void Awake()
    {
        bounceMode = true;
    }
    void Update()
    {
        //tween between a slightly elevated y and slightly lowered y to create bouncing effect if bounceMode
    }

}
