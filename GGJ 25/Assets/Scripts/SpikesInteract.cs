using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikesInteract : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject body = col.gameObject;
        if(body.CompareTag("Player"))
        {
            Debug.Log("Player fell in void");
            //player health script, kill function
        }
        else if(body.CompareTag("Enemy"))
        {
            body.GetComponent<EnemyManage>().healthUpdate(3);
        }
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        GameObject body = col.gameObject;
        if(body.CompareTag("Enemy"))
        {
            body.GetComponent<EnemyManage>().healthUpdate(3);
            
        }
    }
}
