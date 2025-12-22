using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] int health;

    [Header("Bubble")]
    [SerializeField] float bubbleTime;
    private Transform bubbleObj;
    private bool bubbled;
    private float bubbleTimer;

    [Header("Score")]
    public int baseScore;
    private float spawnBuffer = 0.5f;

    [Header("References")]
    [HideInInspector] public AudioManager audioManager;


    private void Awake()
    {
        bubbleObj = transform.GetChild(0);
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
    }

    private void Update()
    {
        spawnBuffer = Mathf.Max(0, spawnBuffer - Time.deltaTime);

        bubbleTimer = Mathf.Max(0, bubbleTimer - Time.deltaTime);
        if (bubbleTimer == 0 && bubbled)
            BubbleChange(false);

        if (Input.GetKeyDown(KeyCode.K))
            Death();
    }

    public void HealthUpdate(int change)
    {
        health -= change;
        if(health <= 0)
        {
            GameObject.Find("Player").GetComponent<GameManager>().KillEnemyFX();
            Death();
        }
    }

    public void BubbleChange(bool becomeBubbled)
    {
        if(becomeBubbled && !bubbled)
        {
            audioManager.Play("Bubbled");
            bubbleObj.gameObject.SetActive(true);
            GetComponent<EnemyMovement>().mode = "IDLE";
        }
        else
        {
            audioManager.Play("Pop");
            StartCoroutine(ResetBounces());
            bubbleObj.gameObject.SetActive(false);
            GetComponent<EnemyMovement>().mode = "MOVE";
        }
        bubbled = becomeBubbled;
    }

    private IEnumerator ResetBounces()
    {
        yield return new WaitForSeconds(1.5f);
        bubbleObj.GetComponent<Bounce>().bounceCount = 0;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject body = col.gameObject;
        if(body.CompareTag("Bubble") && !bubbled)
        {
            Bounce b = body.GetComponent<Bounce>();
            bubbleObj.GetComponent<Bounce>().bounceSpdIncrease = b.bounceSpdIncrease;
            bubbleObj.transform.localScale = new Vector3(1, 1, 1) * (0.5f + 0.8f * body.transform.localScale.x);
            bubbleTimer = bubbleTime * GameObject.Find("Player").GetComponent<PlayerStats>().bubbleLife.multiplier;
            BubbleChange(true);
            Destroy(body);
        }
    }

    private void Death()
    {
        if (spawnBuffer <= 0)
            GameObject.Find("Player").GetComponent<GameManager>().AddScore(baseScore + bubbleObj.GetComponent<Bounce>().bounceCount);
        Destroy(gameObject);
    }
}