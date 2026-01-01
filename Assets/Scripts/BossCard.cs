using UnityEngine;

public class BossCard : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth;
    private int health;


    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject body = col.gameObject;
        if(body.CompareTag("Bubble"))
        {
            //TODO: take damage if bubbled projectile
            Destroy(body);
        }
    }
}
