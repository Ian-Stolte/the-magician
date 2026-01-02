using UnityEngine;
using System.Collections;

public class BossCard : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth;
    private int health;

    [Header("Enemies")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform enemyParent;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
            StartCoroutine(SpawnEnemies(4));
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject body = col.gameObject;
        if(body.CompareTag("Bubble"))
        {
            //TODO: take damage if bubbled projectile
            Destroy(body);
        }
    }

    private IEnumerator SpawnEnemies(int n)
    {
        //show dialogue? spawn an enemy & spotlight at different points, waiting a moment between each
        Time.timeScale = 0.4f;
        for (int i = 0; i < n; i++)
        {
            float angle = (360f / n) * i;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            Vector3 spawnPos = transform.position + direction * 8f;
            Instantiate(enemyPrefab, spawnPos, Quaternion.identity, enemyParent);
            yield return new WaitForSeconds(0.4f);
        }
        Time.timeScale = 1f;
    }

    private void CardExplode()
    {
        //pick a point, spawn a card, wait a second, spawn cards in all directions with velocities away from the center

    }

    private void CardSpring()
    {
        //pick two points, spawn a deck at the first, wait a second, spawn cards moving from the deck to the second point
    }
}
