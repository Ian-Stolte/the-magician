using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflectable : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 direction;
    public float speed;
    public float maxDist;
    protected float distTraveled;
    
    [Header("Properties")]
    public float damage;
    private bool bubbled;
    [SerializeField] private float bubbleDuration;

    [Header("References")]
    private PlayerController player;
    private GameManager gameManager;
    

    void Awake()
    {
        player = GameObject.Find("Player").GetComponent<PlayerController>();
        gameManager = GameObject.Find("Player").GetComponent<GameManager>();
    }

    void FixedUpdate()
    {
        if (!gameManager.paused && !bubbled)
        {
            transform.position += direction*speed*0.02f;
            distTraveled += Vector3.Magnitude(direction)*speed*0.02f;
            if (distTraveled > maxDist)
                Destroy(gameObject);
        }
    }

    void Update()
    {
        if (!bubbled)
        {
            Collider2D playerCol = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Player"));
            if (playerCol != null)
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }

            Collider2D bubbleCol = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Bubble"));
            if (bubbleCol != null)
            {
                if (bubbleCol.CompareTag("Bubble") && !bubbleCol.transform.parent.name.Contains("Card"))
                {
                    Destroy(bubbleCol.gameObject);
                    transform.GetChild(0).gameObject.SetActive(true);
                    bubbled = true;
                }
            }
        }
        else
        {
            bubbleDuration -= Time.deltaTime;
            if (bubbleDuration <= 0)
            {
                Destroy(gameObject);
            }
        }

        Collider2D wallCol = Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Obstacle"));
        if (wallCol != null)
        {
            Destroy(gameObject);
        }
    }
}
