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
            float dmg = body.GetComponent<PlayerStats>().spikeDamage.val;
            if (dmg > 0)
               body.GetComponent<PlayerController>().TakeDamage(dmg);
        }
        else if(body.CompareTag("Enemy"))
        {
            body.GetComponent<EnemyStats>().HealthUpdate(3);
        }
        else if(body.CompareTag("Bubble"))
        {
            Destroy(body);
        }
    }
}
