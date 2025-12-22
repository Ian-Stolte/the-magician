using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth;
    [HideInInspector] public float health;
    public GameObject hpBar;
    [SerializeField] Animator damageFlash;
    private bool playerInvulnerability;

    [Header("Movement")]
    [SerializeField] Joystick joystick;
    private Vector3 moveDir;

    [Header("Touch Input")]
    [SerializeField] RectTransform dashButton;
    private Rect dashRect;
    private Rect joystickRect;
    private int abilityTouch = -1;
    private float touchTimer;

    [Header("Bubble")]
    [SerializeField] GameObject bubblePrefab;
    private Transform bubbleParent;
    private float bubbleDelay;
    private float mouseAngle;
    private Vector3 aimDir;

    [Header("Fan")]
    public LayerMask bubbleLayer;
    public LayerMask enemyLayer;
    [SerializeField] float coneAngle;
    [SerializeField] float coneSizeBase;
    [SerializeField] ParticleSystem fanParticles;
    private bool fanOn;

    [Header("Dash")]
    [SerializeField] Image dashCDFill;
    [SerializeField] float dashDelay;
    [SerializeField] float dashSpeed;
    [SerializeField] float dashDuration;
    [SerializeField] Color dashColor;
    [SerializeField] Color idleColor;
    [SerializeField] GameObject dashParticles;
    [SerializeField] GameObject explosionParticles;
    private float dashTimer;
    private bool dashing;

    [Header("References")]
    [SerializeField] RectTransform canvas;
    private AudioManager audioManager;
    private GameManager gameManager;
    private PlayerStats stats;


    void Start()
    {
        health = maxHealth;
        if (joystick)
            joystickRect = CreateRect(joystick.GetComponent<RectTransform>());
        if (dashButton)
            dashRect = CreateRect(dashButton);
        gameManager = GetComponent<GameManager>();
        stats = GetComponent<PlayerStats>();
        bubbleParent = GameObject.Find("Bubbles").transform;
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
    }

    private Rect CreateRect(RectTransform rect)
    {
        // Get world corners
        Vector3[] corners = new Vector3[4];
        rect.GetWorldCorners(corners);

        // Convert world coordinates → screen coordinates
        Vector2 min = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
        Vector2 max = RectTransformUtility.WorldToScreenPoint(null, corners[2]);

        Vector2 center = min + (max - min) / 2;
        Vector2 size = (max - min) * 3f;
        min = center - size / 2;
        max = center + size / 2;

        return new Rect(min, max - min);
    }

    void Update()
    {
        if (!gameManager.paused && !dashing)
        {
            if (health <= 0)
                gameManager.GameOver();

            //Touch Input
            for (int i = 0; i < Input.touchCount; i++)
            {
                var t = Input.GetTouch(i);
            
                if (t.phase == TouchPhase.Began)
                {
                    if (abilityTouch == -1 && !joystickRect.Contains(t.position) && !dashRect.Contains(t.position))
                    {
                        abilityTouch = t.fingerId;
                        touchTimer = 0;
                    }
                }

                if (t.fingerId == abilityTouch)
                {
                    touchTimer += Time.deltaTime;
                    if (touchTimer > 0.15f && !fanOn)
                        StartFan();
                    SetAimDir(t.position);
                }

                if (t.phase == TouchPhase.Ended && t.fingerId == abilityTouch)
                {
                    abilityTouch = -1;
                    if (touchTimer < 0.15f)
                        FireBubble();
                    else
                        StopFan();
                }
            }

            // Mouse Aim
            if (Input.touchCount == 0)
            {
                SetAimDir(Input.mousePosition);
            }

            //Fire bubble
            bubbleDelay = Mathf.Max(0, bubbleDelay - Time.deltaTime);
            if (Input.GetMouseButtonDown(0))
                FireBubble();

            //Blow fan
            if (Input.GetMouseButton(1))
                StartFan();
            if (Input.GetMouseButtonUp(1))
                StopFan();
            transform.GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(fanOn);
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = !fanOn;   
            if (fanOn)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, coneSizeBase * stats.fanPower.val);
                foreach (Collider2D collider in colliders)
                {
                    if (((1 << collider.gameObject.layer) & bubbleLayer) != 0)
                    {
                        GameObject obj = collider.gameObject;
                        Vector2 targetDir = ((Vector2)obj.transform.position - (Vector2)transform.position).normalized;
                        float angleToTarget = Vector2.Angle(aimDir.normalized, targetDir);
                        if (angleToTarget <= coneAngle)
                        {
                            float inverseDist = 1.0f/(0.5f + Vector2.Distance(transform.position, obj.transform.position)/2.0f);
                            float angleStr = Mathf.InverseLerp(coneAngle, 0, angleToTarget);
                            if(obj.CompareTag("Enemy"))
                                obj.transform.parent.GetComponent<Rigidbody2D>().AddForce(targetDir * stats.fanPower.val*inverseDist*angleStr * Time.deltaTime, ForceMode2D.Impulse);    
                            else
                                obj.GetComponent<Rigidbody2D>().AddForce(targetDir * stats.fanPower.val*inverseDist*angleStr * Time.deltaTime, ForceMode2D.Impulse);
                        }
                    }
                }
            }

            //Dash
            dashTimer = Mathf.Max(0, dashTimer - Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Space))
                Dash();
        }
        dashCDFill.fillAmount = dashTimer/dashDelay;
    
        //Bubble explosion
        if (dashing && stats.bubbleExplode)
        {
            Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, 0.8f, bubbleLayer);
            foreach (Collider2D col in cols)
            {
                if (col.transform.parent == bubbleParent)
                {
                    Destroy(col.gameObject);
                    //knock back enemies
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(col.transform.position, 6f, enemyLayer);
                    foreach (Collider2D enemy in hitEnemies)
                    {
                        Vector3 knockDir = (enemy.transform.position - col.transform.position).normalized;
                        float dist = Vector3.Distance(enemy.transform.position, col.transform.position);
                        float kbStrength = 1 - dist/8f; //100% -> 25%
                        enemy.GetComponent<Rigidbody2D>().AddForce(knockDir * 3 * kbStrength, ForceMode2D.Impulse);
                        StartCoroutine(StunEnemy(enemy.GetComponent<EnemyMovement>()));
                    }
                    //show particles & SFX
                    audioManager.Play("Explosion");
                    GameObject particles = Instantiate(explosionParticles, col.transform.position, Quaternion.identity);
                }
            }
        }
    }

    private IEnumerator StunEnemy(EnemyMovement enemy)
    {
        enemy.mode = "IDLE";
        yield return new WaitForSeconds(0.5f);
        enemy.mode = "MOVE";
    }

    //Movement
    void FixedUpdate()
    {
        if (!gameManager.paused && !dashing)
        {
            float horiz = joystick.Horizontal;
            float vert = joystick.Vertical;
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
                horiz -= 1;
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
                horiz += 1;
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
                vert -= 1;
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
                vert += 1;

            moveDir = new Vector3(Mathf.Clamp(horiz, -1, 1), Mathf.Clamp(vert, -1, 1), 0);
            if (moveDir.magnitude > 1)
                moveDir = moveDir.normalized;
                
            transform.position += moveDir * 0.02f * stats.speed.val;
        }
    }


    private void SetAimDir(Vector3 pos)
    {
        Rect canvasRect = canvas.rect;
        Vector3 canvasScale = canvas.localScale;
        Camera cam = Camera.main.GetComponent<Camera>();
        float camWidth = cam.orthographicSize*cam.aspect;
        float camHeight = cam.orthographicSize;
        float playerXPct = ((transform.position.x - cam.transform.position.x) + camWidth) / (camWidth*2);
        float playerYpct = ((transform.position.y - cam.transform.position.y) + camHeight) / (camHeight*2);
        float mouseXChange = pos.x - playerXPct*canvasRect.width*canvasScale.x;
        float mouseYChange = pos.y - playerYpct*canvasRect.height*canvasScale.y;
        aimDir = new Vector3(mouseXChange, mouseYChange, 0);
        aimDir = Vector3.Normalize(aimDir);

        float mouseAngle = Mathf.Atan2(mouseYChange, mouseXChange) * Mathf.Rad2Deg - 90;
        transform.GetChild(0).transform.RotateAround(transform.position - new Vector3(0, 0.2f, 0), new Vector3(0, 0, 1), mouseAngle - transform.GetChild(0).transform.rotation.eulerAngles.z);
    }

    public void FireBubble()
    {
        if (bubbleDelay == 0)
        {
            bubbleDelay = 0.5f;
            //audioManager.Play("FireBubble");   
            
            GameObject bubble = Instantiate(bubblePrefab, transform.position + aimDir * 2, Quaternion.identity, bubbleParent);
            bubble.transform.localScale *= stats.bubbleSize.val;
            bubble.GetComponent<Rigidbody2D>().linearVelocity = aimDir*bubble.GetComponent<Bounce>().startSpeed;
            bubble.GetComponent<Bubble>().maxLifetime = stats.bubbleLife.val;
            bubble.GetComponent<Bounce>().bounceSpdIncrease = stats.bounceSpeed.val;
        }
    }

    public void StartFan()
    {
        fanOn = true;
        fanParticles.Play();
        //audioManager.Play("Blow");
    }

    public void StopFan()
    {
        fanOn = false;
        fanParticles.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
        //audioManager.Stop("Blow");
    }

    public void Dash()
    {
        if (dashTimer == 0)
        {
            audioManager.Play("Dash");
            dashTimer = dashDelay;
            float dashDist = 6.6f;
            while (Physics2D.OverlapCircle(transform.position + (moveDir.normalized*dashDist), 0.1f, LayerMask.GetMask("Obstacle")))
            {
                dashDist -= 0.1f;
            }
            StartCoroutine(Dash(dashDist-0.1f));
        }
    }

    private IEnumerator Dash(float dashDist)
    {
        dashing = true;
        fanOn = false;
        fanParticles.Stop(withChildren: false, ParticleSystemStopBehavior.StopEmitting);
        GameObject particles = Instantiate(dashParticles, transform.position, Quaternion.identity, transform);

        float distTraveled = 0;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().color = dashColor;
        transform.GetChild(0).gameObject.SetActive(false);
        float timer = 0f;
        while (timer < 1)
        {
            timer += Time.deltaTime * (1/dashDuration);
            Vector3 change = dashSpeed * Time.deltaTime * (-Mathf.Pow((timer-0.25f), 2) + 1) * moveDir;
            transform.position += change;
            distTraveled += Vector3.Magnitude(change);
            if (distTraveled > dashDist)
                break;
            yield return null;
        }
        GetComponent<SpriteRenderer>().color = idleColor;
        GetComponent<BoxCollider2D>().enabled = true;
        transform.GetChild(0).gameObject.SetActive(true);
        dashing = false;
        yield return new WaitForSeconds(3);
        Destroy(particles);
    }

    public void TakeDamage(float dmg)
    {   
        audioManager.Play("PlayerHit");
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
}
