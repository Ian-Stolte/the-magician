using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EnemyManage : MonoBehaviour
{
    [SerializeField]
    int health;
    Transform bubbleObj;
    private bool bubbled;
    private float bubbleTimer;
    [SerializeField] private float bubbleTime;

    [SerializeField] private int enemyScore;

    public int killBonus;

    private int bubbleBounceCount;
 
 
    private void Awake()
    {
        bubbleObj = transform.GetChild(0);
    }


    private void Update()
    {
        bubbleTimer = Mathf.Max(0, bubbleTimer - Time.deltaTime);
        if (bubbleTimer == 0 && bubbled)
            BubbleChange(false);
    }
    public void healthUpdate(int change)
    {
        health -= change;
        if(health <= 0)
        {
            PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
            player.KillEnemy();
            Death();
        }
    }

    public void BubbleChange(bool check)
    {
        if(check && !bubbled)
        {
            bubbleObj.gameObject.SetActive(true);
            GetComponent<EnemyMovement>().mode = "IDLE";
            bubbleTimer = bubbleTime;
        }
        else
        {
            enemyScore = bubbleObj.GetComponent<Bounce>().bounceCount;
            bubbleObj.GetComponent<Bounce>().bounceCount = 0;
            bubbleObj.gameObject.SetActive(false);
            GetComponent<EnemyMovement>().mode = "MOVE";
        }
        bubbled = check;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject body = col.gameObject;
        if(body.CompareTag("Bubble") && !bubbled)
        {
            BubbleChange(true);
            Destroy(body);
        }
    }

    private void Death()
    {
        PlayerController player = GameObject.Find("Player").GetComponent<PlayerController>();
        player.AddScore(enemyScore + killBonus);
        enemyScore = 0;
        BubbleChange(false);
        Destroy(this.gameObject);
    }
}