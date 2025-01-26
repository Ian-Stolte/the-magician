using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private float bulletDelay;
    private float mouseAngle;
    private Vector3 bulletDir;
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private GameObject dashParticlePrefab;

    //Keybinds
    [SerializeField] private KeyCode dashBind;
    [SerializeField] private KeyCode shootBind;
    [SerializeField] private KeyCode fanBind;

    public float maxHealth;
    public float health;
    [SerializeField] private GameObject hpBar;
    [SerializeField] private Animator damageFlash;

    [SerializeField] private float speed;
    public bool paused;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOver;

    //Fan
    public LayerMask bubbleLayer;
    public LayerMask enemyLayer;
    private bool fanOn;
    [SerializeField] private float fanPower;
    [SerializeField] private float coneAngle;
    [SerializeField] private float coneRadius;

    //Dash
    private float dashTimer;
    [SerializeField] private float dashDelay;
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private Color dashColor;
    [SerializeField] private Color idleColor;
    private bool dashing;

    [SerializeField] private GameObject camera;


    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
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
            transform.GetChild(0).transform.RotateAround(transform.position, new Vector3(0, 0, 1), mouseAngle - transform.GetChild(0).transform.rotation.eulerAngles.z);
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
            }

            //Blow fan
            if (Input.GetKeyDown(fanBind))
                fanOn = true;
            if (Input.GetKeyUp(fanBind))
                fanOn = false;

            transform.GetChild(0).GetChild(0).gameObject.SetActive(fanOn);
            transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = !fanOn;   
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

            //Dash
            dashTimer = Mathf.Max(0, dashTimer - Time.deltaTime);
            if (Input.GetKeyDown(dashBind) && dashTimer == 0)
            {
                dashTimer = dashDelay;
                float dashDist = 6.6f;
                while (Physics2D.OverlapCircle(transform.position + (bulletDir.normalized*dashDist), 0.1f, LayerMask.GetMask("Obstacle")))
                {
                    dashDist -= 0.1f;
                }
                StartCoroutine(Dash(dashDist-0.1f));
            }
        }
    }

    private IEnumerator Dash(float dashDist)
    {
        dashing = true;
        fanOn = false;
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
        GameObject bubble = Instantiate(bubblePrefab, transform.position + bulletDir, Quaternion.identity, GameObject.Find("Bubbles").transform);
        bubble.GetComponent<Rigidbody2D>().velocity = bulletDir*bubble.GetComponent<Bubble>().speed;
        bubble.GetComponent<Bubble>().player = this;
    }

    public void TakeDamage(float dmg)
    {   
        health -= dmg;
        damageFlash.Play("DamageFlash");
        hpBar.GetComponent<Image>().fillAmount = health/maxHealth;
    }

    public void KillEnemy()
    {
        StartCoroutine(camera.GetComponent<CameraShake>().Shake(0.05f, 0.8f));
    }

    private void GameOver()
    {
        paused = true;
        gameOver.SetActive(true);
    }
}
