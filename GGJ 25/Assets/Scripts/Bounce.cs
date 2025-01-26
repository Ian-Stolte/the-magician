using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private float bounceTimer;

    private Vector2 velo;
    public float maxSpeed;

    public int bounceCount;
    

    void Awake()
    {
        bounceCount = 0;
    }
    void Update()
    {
        bounceTimer = Mathf.Max(0, bounceTimer - Time.deltaTime);
        velo = rb.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            bounceCount++;
            Vector2 normal = collision.contacts[0].normal;

            if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
            {
                rb.velocity = new Vector2(velo.x * -1, velo.y);
                if (Vector3.Magnitude(rb.velocity) < maxSpeed)
                    rb.velocity *= 1.5f;
            }
            else
            {
                rb.velocity = new Vector2(velo.x, velo.y * -1);
                if (Vector3.Magnitude(rb.velocity) < maxSpeed)
                    rb.velocity *= 1.5f;
            }
            
            
            bounceTimer = 0.5f;
        }
        print("bouncecount is " + bounceCount);
    }

    
}
