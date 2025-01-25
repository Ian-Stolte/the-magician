using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float speed;
    private float distanceTraveled;

    [SerializeField] private float maxLifetime;
    private float lifetime;

    [HideInInspector] public PlayerController player;
    

    void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime > maxLifetime)
            Destroy(gameObject);
    }

    /*void FixedUpdate()
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
    }*/
}
