using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesInteract : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
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
            Debug.Log("Enemy was spiked");
            //get enemy health component of collider , kill function
            //maybe need to check if enemy is in bubble? and in this case, dont kill them?
        }
    }
}
