using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class EnemyManage : MonoBehaviour
{

    [SerializeField]
    int health;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void healthUpdate(int change)
    {
        Debug.Log("health change " + change);
        health -= change;
    }
}
