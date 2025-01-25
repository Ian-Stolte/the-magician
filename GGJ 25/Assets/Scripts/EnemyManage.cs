using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EnemyManage : MonoBehaviour
{

    [SerializeField]
    int health;

    public void healthUpdate(int change)
    {
        Debug.Log("health change " + change);
        health -= change;
        if(health <= 0)
        {
            death();
        }
    }

    private void death()
    {
        Debug.Log("enemy died");
        Destroy(this.gameObject);
        
    }
}
