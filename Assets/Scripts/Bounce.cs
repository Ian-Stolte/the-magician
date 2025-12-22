using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bounce : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    private float bounceTimer;

    [Header("Speed")]
    public float startSpeed;
    public float maxSpeed;
    public float bounceSpdIncrease;
    [SerializeField] float minBounceSpeed;
    private Vector2 velo;
    
    [HideInInspector] public int bounceCount;
    public TMPro.TextMeshProUGUI bounceText;


    void Update()
    {
        bounceTimer = Mathf.Max(0, bounceTimer - Time.deltaTime);
        velo = rb.linearVelocity;
        if (bounceText != null)
            bounceText.text = bounceCount.ToString();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            GameObject.Find("Audio Manager").GetComponent<AudioManager>().Play("Bounce");
            bounceCount++;
            Vector2 normal = collision.contacts[0].normal;

            if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y))
            {
                rb.linearVelocity = new Vector2(velo.x * -1, velo.y);
                if (Vector3.Magnitude(rb.linearVelocity) < maxSpeed)
                    rb.linearVelocity *= bounceSpdIncrease;
            }
            else
            {
                rb.linearVelocity = new Vector2(velo.x, velo.y * -1);
                if (Vector3.Magnitude(rb.linearVelocity) < maxSpeed)
                    rb.linearVelocity *= bounceSpdIncrease;
            }

            //increase speed if too slow to prevent getting stuck on walls
            if (rb.linearVelocity.magnitude < minBounceSpeed)
            {
                rb.linearVelocity += rb.linearVelocity.normalized * 2;
            }
            bounceTimer = 0.5f;
        }
    }
}
