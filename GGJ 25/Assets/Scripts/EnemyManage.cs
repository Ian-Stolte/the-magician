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
 
 
    private void Awake()
    {
        bubbleObj = transform.GetChild(0);
    }


    private void Update()
    {
        bubbleTimer = Mathf.Max(0, bubbleTimer - Time.deltaTime);
        if (bubbleTimer == 0 && bubbled)
            bubbleChange(false);
    }

    public void healthUpdate(int change)
    {
        Debug.Log("health change " + change);
        health -= change;
        if(health <= 0)
        {
            death();
        }
    }

    public void bubbleChange(bool check)
    {
        if(check && !bubbled)
        {
            bubbleObj.gameObject.SetActive(true);
            GetComponent<EnemyMovement>().mode = "IDLE";
            bubbleTimer = bubbleTime;
        }
        else
        {
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
            bubbleChange(true);
            Destroy(body);
        }
    }

    private void death()
    {
        bubbleChange(false);
        Destroy(this.gameObject);
    }
}