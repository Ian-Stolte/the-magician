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
            body.GetComponent<PlayerController>().TakeDamage(2);
        }
        else if(body.CompareTag("Enemy"))
        {
            body.GetComponent<EnemyManage>().healthUpdate(3);
        }
        else if(body.CompareTag("Bubble"))
        {
            Destroy(body);
        }
    }
}
