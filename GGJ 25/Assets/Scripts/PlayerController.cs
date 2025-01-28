using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speed;
    private float bulletDelay;
    private float mouseAngle;
    private Vector3 bulletDir;
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private GameObject dashParticlePrefab;
    [SerializeField] private GameObject camera;

    [Header("Enemies")]
    [SerializeField] private Transform enemies;
    [SerializeField] private TMPro.TextMeshProUGUI enemyText;

    [Header("Keybinds")]
    [SerializeField] private KeyCode dashBind;
    [SerializeField] private KeyCode shootBind;
    [SerializeField] private KeyCode fanBind;

    [Header("Health")]
    public float maxHealth;
    public float health;
    public GameObject hpBar;
    [SerializeField] private Animator damageFlash;
    private bool playerInvulnerability;

    [Header("Pause")]
    public bool paused;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOver;

    [Header("Fan")]
    public LayerMask bubbleLayer;
    public LayerMask enemyLayer;
    private bool fanOn;
    [SerializeField] private float fanPower;
    [SerializeField] private float coneAngle;
    [SerializeField] private float coneRadius;
    [SerializeField] private ParticleSystem fanParticles;

    [Header("Dash")]
    private float dashTimer;
    [SerializeField] private float dashDelay;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private Color dashColor;
    [SerializeField] private Color idleColor;
    private bool dashing;
    [Header("Score")]
    public int playerScore;
    public int highScore;
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    [SerializeField] private float scoreMultiplier;
    [SerializeField] private bool multiplierOn;

    [SerializeField] private float comboCounter;
    [SerializeField] private GameObject comboPrefab;
    [SerializeField] private Transform comboTextHolder;
    [SerializeField] private Transform bounceTextHolder;
    [SerializeField] private GameObject multiplierText;
    public int bounceScore;




    void Start()
    {
        health = maxHealth;
        scoreMultiplier = 1f;
    }

    void Update()
    {
        //Enemy text
        enemyText.text = "Enemies:  <b>" + enemies.childCount;

        //Score text
        scoreText.text = "Score:  <b>" + playerScore;

        //Pause game
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (!paused)
            {
                paused = true;
                pauseMenu.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                paused = false;
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
        }

        if (!paused && !dashing)
        {
            mouseAngle = GetMouseRot();
            transform.GetChild(0).transform.RotateAround(transform.position - new Vector3(0, 0.2f, 0), new Vector3(0, 0, 1), mouseAngle - transform.GetChild(0).transform.rotation.eulerAngles.z);
            if (health <= 0)
            {
                GameOver();
            }
            
            //Fire bubble
            bulletDelay = Mathf.Max(0, bulletDelay - Time.deltaTime);
            if (Input.GetKeyDown(shootBind) && bulletDelay == 0)
            {
                bulletDelay = 0.5f;
                FireBullet();
                GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("FireBubble");
            }

            //Blow fan
            if (Input.GetKeyDown(fanBind))
            {
                fanOn = true;
                fanParticles.Play();
                GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("Blow");
            }
            if (Input.GetKeyUp(fanBind))
            {
                fanOn = false;
                fanParticles.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
                GameObject.Find("AudioManager").GetComponent<AudioManager>().Stop("Blow");
            }

            transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(fanOn);
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = !fanOn;   

            //Dash
            dashTimer = Mathf.Max(0, dashTimer - Time.deltaTime);
            if (Input.GetKeyDown(dashBind) && dashTimer == 0)
            {
                GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("Dash");
                dashTimer = dashDelay;
                float dashDist = 6.6f;
                while (Physics2D.OverlapCircle(transform.position + (bulletDir.normalized*dashDist), 0.1f, LayerMask.GetMask("Obstacle")))
                {
                    dashDist -= 0.1f;
                }
                StartCoroutine(Dash(dashDist-0.1f));
            }
        }
        comboCounter = Mathf.Max(0, comboCounter - Time.deltaTime);
        if(comboCounter <= 0f)
        {
            scoreMultiplier = 1f;
            multiplierOn = false;
            changeMultiplier();
        }
        else
        {
            multiplierOn = true;
        }

    }

    private IEnumerator Dash(float dashDist)
    {
        dashing = true;
        fanOn = false;
        fanParticles.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
        GameObject particles = Instantiate(dashParticlePrefab, transform.position, Quaternion.identity, transform);
        Vector3 dir = bulletDir;
        float distTraveled = 0;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = dashColor;
        transform.GetChild(0).gameObject.SetActive(false);
        for (float i = 0; i < 1; i += 0.01f * (1/dashDuration))
        {
            Vector3 change = dashSpeed * 0.01f * (-Mathf.Pow((i-0.25f), 2) + 1) * dir;
            transform.position += change;
            distTraveled += Vector3.Magnitude(change);
            if (distTraveled > dashDist)
                i = 1.01f; //end for loop
            yield return new WaitForSeconds(0.01f);
        }
        GetComponent<SpriteRenderer>().color = idleColor;
        GetComponent<BoxCollider2D>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        dashing = false;
        yield return new WaitForSeconds(3);
        Destroy(particles);
    }


    void FixedUpdate()
    {
        int horiz = 0;
        int vert = 0;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            horiz -= 1;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            horiz += 1;
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            vert -= 1;
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            vert += 1;

        if (!paused && !dashing)
        {
            float moveMag = Mathf.Sqrt(Mathf.Pow(horiz, 2) + Mathf.Pow(vert, 2));
            if (moveMag > 0)
                transform.position += new Vector3(horiz*speed*0.02f/moveMag, vert*speed*0.02f/moveMag, 0);
        }

        if (fanOn)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, coneRadius);
            foreach (Collider2D collider in colliders)
            {
                if (((1 << collider.gameObject.layer) & bubbleLayer) != 0)
                {
                    GameObject obj = collider.gameObject;
                    Vector2 targetDir = ((Vector2)obj.transform.position - (Vector2)transform.position).normalized;
                    float angleToTarget = Vector2.Angle(bulletDir.normalized, targetDir);
                    if (angleToTarget <= coneAngle)
                    {
                        float inverseDist = 1.0f/(0.5f + Vector2.Distance(transform.position, obj.transform.position)/2.0f);
                        float angleStr = Mathf.InverseLerp(coneAngle, 0, angleToTarget);
                        if(obj.CompareTag("Enemy"))
                        {
                            obj.transform.parent.GetComponent<Rigidbody2D>().AddForce(targetDir * fanPower*inverseDist*angleStr);    
                        }
                        else
                            obj.GetComponent<Rigidbody2D>().AddForce(targetDir * fanPower*inverseDist*angleStr);
                    }
                }
            }
        }
    }

    private float GetMouseRot()
    {
        Vector3 mousePos = Input.mousePosition;
        Rect canvasRect = GameObject.Find("Canvas").GetComponent<RectTransform>().rect;
        Vector3 canvasScale = GameObject.Find("Canvas").GetComponent<RectTransform>().localScale;
        Camera cam = camera.GetComponent<Camera>();
        float camWidth = cam.orthographicSize*cam.aspect;
        float camHeight = cam.orthographicSize;
        float playerXPct = ((transform.position.x - cam.transform.position.x) + camWidth) / (camWidth*2);
        float playerYpct = ((transform.position.y - cam.transform.position.y) + camHeight) / (camHeight*2);
        float mouseXChange = mousePos.x - playerXPct*canvasRect.width*canvasScale.x;
        float mouseYChange = mousePos.y - playerYpct*canvasRect.height*canvasScale.y;
        bulletDir = new Vector3(mouseXChange, mouseYChange, 0);
        bulletDir = Vector3.Normalize(bulletDir);
        return(Mathf.Atan2(mouseYChange, mouseXChange) * Mathf.Rad2Deg - 90);
    }

    public void FireBullet()
    {
        //GetComponent<Animator>().Play("Fire");
        GameObject bubble = Instantiate(bubblePrefab, transform.position + bulletDir * 2, Quaternion.identity, GameObject.Find("Bubbles").transform);
        bubble.GetComponent<Rigidbody2D>().velocity = bulletDir*bubble.GetComponent<Bubble>().speed;
        bubble.GetComponent<Bubble>().player = this;
    }

    public void TakeDamage(float dmg)
    {   
        GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("PlayerHit");
        hpBar.transform.parent.gameObject.SetActive(true);
        if(!playerInvulnerability || health >= 3)
        {
            health -= dmg;
            hpBar.GetComponent<Image>().fillAmount = health/maxHealth;
        }
        damageFlash.Play("DamageFlash");

    }

    public void SetInvulnerability(bool invulnerability)
    {
        playerInvulnerability = invulnerability;
        hpBar.transform.parent.gameObject.SetActive(!invulnerability);
    }

    public void KillEnemy()
    {
        StartCoroutine(camera.GetComponent<CameraShake>().Shake(0.05f, 0.8f));
    }

    private void GameOver()
    {
        paused = true;
        gameOver.SetActive(true);
        gameOver.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = "Score:  <b>" + playerScore;
        gameOver.transform.GetChild(3).gameObject.SetActive(playerScore > highScore);
        if (playerScore > highScore)
        {
            highScore = playerScore;
        }
    }

    public void AddScore(int enemyScore)
    {
        GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("Ping");
        comboCounter = 3f;
        if(enemyScore > 5)
        {
            showBounce(enemyScore-5);
        }
        if(multiplierOn)
        {
            scoreMultiplier += 0.5f;
            enemyScore = (int)(enemyScore*scoreMultiplier*0.5f);
            showScore();
        }
        changeMultiplier();
        playerScore += enemyScore;
    }

    private void showScore()
    {
        GameObject tempComboDisplay = Instantiate(comboPrefab, comboTextHolder);
        if(scoreMultiplier < 2.0f)
            tempComboDisplay.GetComponent<TextMeshProUGUI>().text = "Double Kill!";
        else if(scoreMultiplier < 2.5f)
            tempComboDisplay.GetComponent<TextMeshProUGUI>().text = "Triple Kill!";
        else if(scoreMultiplier < 3.0f)
            tempComboDisplay.GetComponent<TextMeshProUGUI>().text = "Quadruple Kill!";
        StartCoroutine(killText(tempComboDisplay));
        //create a text object with an autostart animation of fading onto the screen then fading down and destroying self, create with corresponding text to score multiplier
        //update 
    }

    private void showBounce(int bounceCount)
    {
        GameObject tempBounceDisplay = Instantiate(comboPrefab, bounceTextHolder);
        tempBounceDisplay.GetComponent<TextMeshProUGUI>().text = "Bounce x" + bounceCount;
        StartCoroutine(killText(tempBounceDisplay));
    }

    private void changeMultiplier()
    {
        multiplierText.GetComponent<RectTransform>().localScale = new Vector3(scoreMultiplier, scoreMultiplier, 1f);
        multiplierText.GetComponent<TextMeshProUGUI>().text = scoreMultiplier + "x";
    }

    public void resetScore()
    {
        playerScore = 0;
        bounceScore = 0;
    }
    public IEnumerator killText(GameObject tempComboText)
    {
        for (float i = 3; i > 0; i -= 0.01f)
        {
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(tempComboText);

    }
}
