using UnityEngine;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Pause")]
    public bool paused;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameOver;

    [Header("Enemies")]
    [SerializeField] Transform enemies;
    [SerializeField] TextMeshProUGUI enemyText;

    [Header("Score")]
    [SerializeField] GameObject comboPrefab;
    [SerializeField] Transform comboTextHolder;
    [SerializeField] GameObject multiplierText;
    [SerializeField] float comboResetTime;
    [SerializeField] float multikillResetTime;
    private float comboTimer;
    private float multikillTimer;
    private int killStreak;
    private bool multiplierOn;
    private float scoreMultiplier;
    
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Transform bounceTextHolder;
    [SerializeField] int highScore;
    private GameObject currentText;
    private int playerScore;

    [Header("References")]
    [SerializeField] TileGeneration tileGen;
    private AudioManager audioManager;
    private PlatformSettings platform;
    
    public TextMeshProUGUI platformText;
        

    void Start()
    {
        scoreMultiplier = 1f;
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
        platform = PlatformSettings.Get();
        if (platformText)
            platformText.text = "Mobile: " + platform.mobile;
    }

    void Update()
    {
        //Enemy text
        enemyText.text = "Enemies: <b>" + enemies.childCount;

        //Score text
        scoreText.text = "Score: <b>" + playerScore;

        //Combo multiplier
        if (!tileGen || !tileGen.generating)
            comboTimer = Mathf.Max(0, comboTimer - Time.deltaTime);
        if(comboTimer <= 0f)
        {
            scoreMultiplier = 1f;
            ChangeMultiplier();
        }
        multikillTimer = Mathf.Max(0, multikillTimer - Time.deltaTime);

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
    }


    public void GameOver()
    {
        paused = true;
        gameOver.SetActive(true);
        gameOver.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = "Score:  <b>" + playerScore;
        gameOver.transform.GetChild(3).gameObject.SetActive(playerScore > highScore);
        if (playerScore > highScore)
            highScore = playerScore;
    }

    public void KillEnemyFX()
    {
        StartCoroutine(Camera.main.GetComponent<CameraShake>().Shake(0.05f, 0.8f));
    }

    //add to score after killing an enemy, increase multiplier
    public void AddScore(int enemyScore)
    {
        audioManager.Play("Ping");
        comboTimer = comboResetTime;
        if (enemyScore > 6) //if more than 1 bounce
        {
            ShowBounce(enemyScore-5);
        }
        if (multikillTimer > 0f)
        {
            killStreak++;
            ShowMultikill();
        }
        else
            killStreak = 1;

        multikillTimer = multikillResetTime;

        if (comboTimer > 0f && (scoreMultiplier > 1 || killStreak > 1))
        {
            enemyScore = (int)Mathf.Round(enemyScore*scoreMultiplier);
            scoreMultiplier = Mathf.Min(5, scoreMultiplier + 0.5f);
        }
        ChangeMultiplier();
        playerScore += enemyScore;
    }

    //create a text object w/ autostart animation to show the current kill combo
    private void ShowMultikill()
    {
        GameObject comboTxt = Instantiate(comboPrefab, comboTextHolder);
        if(killStreak == 2)
            comboTxt.GetComponent<TextMeshProUGUI>().text = "Double Kill!";
        else if(killStreak == 3)
            comboTxt.GetComponent<TextMeshProUGUI>().text = "Triple Kill!";
        else if(killStreak == 4)
            comboTxt.GetComponent<TextMeshProUGUI>().text = "Quadruple Kill!";
        else
            comboTxt.GetComponent<TextMeshProUGUI>().text = "Rampage!";
        StartCoroutine(KillText(comboTxt, 3));
    }

    //create a text object w/ autostart animation to show the number of bounces for the last kill
    private void ShowBounce(int bounceCount)
    {
        GameObject bounceTxt = Instantiate(comboPrefab, bounceTextHolder);
        bounceTxt.GetComponent<TextMeshProUGUI>().text = "Bounce x" + bounceCount;
        StartCoroutine(KillText(bounceTxt, 3));
    }

    //update score multiplier text
    private void ChangeMultiplier()
    {
        float txtScale = Mathf.Min(0.5f + scoreMultiplier/2f, 2f);
        multiplierText.GetComponent<RectTransform>().localScale = new Vector3(txtScale, txtScale, 1f);
        multiplierText.GetComponent<TextMeshProUGUI>().text = scoreMultiplier + "x";
    }

    public void ResetScore()
    {
        playerScore = 0;
    }

    //destroy object after a delay
    public IEnumerator KillText(GameObject obj, float wait)
    {
        if (currentText != null)
            Destroy(currentText);
        currentText = obj;
        yield return new WaitForSeconds(wait/2f);
        currentText = null;
        yield return new WaitForSeconds(wait/2f);
        Destroy(obj);
    }
}