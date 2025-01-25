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

    public float maxHealth;
    public float health;
    [SerializeField] private float speed;
    public bool paused;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOver;


    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        if (!paused)
        {
            mouseAngle = GetMouseRot();
            transform.GetChild(0).transform.RotateAround(transform.position, new Vector3(0, 0, 1), mouseAngle - transform.GetChild(0).transform.rotation.eulerAngles.z);
            if (health <= 0)
            {
                GameOver();
            }
        }
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

        bulletDelay = Mathf.Max(0, bulletDelay - Time.deltaTime);
        if (Input.GetMouseButtonDown(0) && bulletDelay == 0)
        {
            bulletDelay = 0.5f;
            FireBullet();
        }
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

        if (!paused)
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
        Camera cam = GameObject.Find("Main Camera").GetComponent<Camera>();
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
        bubble.GetComponent<Bubble>().direction = bulletDir;
        bubble.GetComponent<Bubble>().player = this;
    }

    public void TakeDamage(float dmg)
    {   
        health -= dmg;
        //GetComponent<Animator>().Play("TakeDamage");
        GameObject.Find("HP Bar").GetComponent<Image>().fillAmount = health/maxHealth;
    }

    private void GameOver()
    {
        paused = true;
        gameOver.SetActive(true);
    }
}
