using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
    private float distanceTraveled;

    public PlayerController player;
    

    void FixedUpdate()
    {
        if (!player.paused)
        {
            transform.position += direction*speed*0.02f;
            distanceTraveled += Vector3.Magnitude(direction)*speed*0.02f;
            if (distanceTraveled > 20)
            {
                Destroy(gameObject);
            }
        }
    }
}
