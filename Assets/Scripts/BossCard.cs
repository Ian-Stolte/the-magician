using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class BossCard : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] Image healthBar;
    [SerializeField] int maxHealth;
    private int health;

    [Header("Enemies")]
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform enemyParent;

    [Header("Attack Timers")]
    [SerializeField] float spawnDelay;
    private float spawnTimer;
    [SerializeField] float explosionDelay;
    private float explosionTimer;
    [SerializeField] float fireDelay;
    private float fireTimer;

    [Header("Attacks")]
    [SerializeField] GameObject cardPrefab;
    [SerializeField] GameObject deflectablePrefab;
    [SerializeField] Transform bulletParent;
    private bool firing;

    [Header("References")]
    [SerializeField] CanvasGroup fader;
    private Transform player;


    void Start()
    {
        health = maxHealth;
        player = GameObject.Find("Player").transform;

        spawnTimer = 2f;
        explosionTimer = 5f;
        fireTimer = 10f;
    }

    void Update()
    {
        //Spawn enemies
        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0)
        {
            spawnTimer = spawnDelay;
            StartCoroutine(SpawnEnemies(4));
        }

        //Card explosions
        if (!firing && fireTimer > 1f)
            explosionTimer -= Time.deltaTime;
        if (explosionTimer <= 0)
        {
            explosionTimer = explosionDelay;
            StartCoroutine(PickExplosionPoints(4));
        }

        //Card fire
        if (explosionTimer > 1f)
            fireTimer -= Time.deltaTime;
        if (fireTimer <= 0)
        {
            fireTimer = fireDelay;
            StartCoroutine(FireCards(5));
        }
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        GameObject body = col.gameObject;
        if(body.CompareTag("Bubble"))
        {
            Destroy(body);
        }
        else if (body.CompareTag("Deflectable"))
        {
            Destroy(body);

            //Check health
            health -= 1;
            StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.05f, 0.8f));
            if (health <= 0)
            {
                player.GetComponent<PlayerController>().StartCoroutine(Death());
            }
            healthBar.fillAmount = health / (float)maxHealth;
            //TODO: taking damage vfx
        }
    }

    private IEnumerator Death()
    {
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(enemy);
        Destroy(healthBar.transform.parent.gameObject);
        Time.timeScale = 1f;
        Destroy(gameObject);
        
        yield return new WaitForSeconds(1f);
        float fadeTimer = 0f;
        while (fadeTimer < 1)
        {
            fadeTimer += Time.deltaTime;
            yield return null;
            fader.alpha = fadeTimer;
        }
        SceneManager.LoadScene("Combat Scene"); 
    }

    private IEnumerator SpawnEnemies(int n)
    {
        //show dialogue?

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


    private IEnumerator PickExplosionPoints(int n)
    {
        for (int i = 0; i < n; i++)
        {
            Vector3 targetPos = Vector3.zero;
            do
            {
                targetPos = transform.position + Random.insideUnitSphere * 8f;
            } while (Vector3.Distance(player.position, targetPos) < 2.5f);
            StartCoroutine(CardExplode(targetPos));
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator CardExplode(Vector3 position)
    {
        //spawn a card, wait a second, then spawn cards shooting away from the center in all directions
        GameObject rootCard = Instantiate(cardPrefab, position, Quaternion.identity, bulletParent);
        yield return new WaitForSeconds(1f);
        if (rootCard == null)
            yield break;
        Destroy(rootCard);

        for (int i = 0; i < 6; i++)
        {
            float angle = (360f / 6) * i;
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            GameObject card = Instantiate(cardPrefab, position, Quaternion.identity, bulletParent);
            card.GetComponent<CardProjectile>().direction = direction;
        }
    }


    private IEnumerator FireCards(int n)
    {
        firing = true;
        //rapidly shoot cards toward the player-- if bubbled, they can be fired back
        for (int i = 0; i < n; i++)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            GameObject card = Instantiate(deflectablePrefab, transform.position, Quaternion.identity, bulletParent);
            card.GetComponent<Deflectable>().direction = direction;
            yield return new WaitForSeconds(0.5f);
        }
        firing = false;
    }

    private void CardSpring()
    {
        //pick two points, spawn a deck at the first, wait a second, then spawn cards moving from the deck to the second point
    }
}
