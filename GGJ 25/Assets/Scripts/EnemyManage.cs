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
    private void Awake()
    {
        bubbleObj = this.gameObject.transform.GetChild(0);
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
        if(check)
        {
            bubbleObj.gameObject.SetActive(true);
        }
        else
        {
            bubbleObj.gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject body = col.gameObject;
        if(body.CompareTag("Bubble"))
        {
            bubbleChange(true);
        }
    }

    private void death()
    {
        bubbleChange(false);
        Debug.Log("enemy died");
        Destroy(this.gameObject);
        
    }


}
