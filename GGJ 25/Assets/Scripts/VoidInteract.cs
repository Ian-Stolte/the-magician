using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidInteract : MonoBehaviour
{
    // player health script reference
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.CompareTag("Player"))
        {
            Debug.Log("Player fell in void");
            //player health script, kill function
        }
        else if(col.collider.CompareTag("Enemy"))
        {
            Debug.Log("Enemy fell in void");
            //get enemy health component of collider , kill function
            //maybe need to check if enemy is in bubble? and in this case, dont kill them?
        }
    }


}
