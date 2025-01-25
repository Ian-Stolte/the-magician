using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
    private float distanceTraveled;
    public float damage;

    private PlayerController player;
    

    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
    }

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

            Collider2D playerCollision = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Player"));
            if (playerCollision != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }

            Collider2D bubbleCollision = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Bubble"));
            if (bubbleCollision != null)
            {
                if (bubbleCollision.CompareTag("Bubble"))
                {
                    Destroy(bubbleCollision.gameObject);
                    speed -= 3;
                    if (speed <= 0)
                        Destroy(gameObject);
                }
            }
        }
    }
}
