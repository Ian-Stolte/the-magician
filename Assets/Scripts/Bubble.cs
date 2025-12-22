using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    public float maxLifetime;
    private float lifetime;

    void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime > maxLifetime)
            Destroy(gameObject);
    }
}
