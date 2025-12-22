using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialLoad : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] FadeMessage fader;
    [SerializeField] GameObject hpBar;
    [SerializeField] GameObject enemiesText;

    [Header("In-World")]
    [SerializeField] PlayerController player;
    [SerializeField] Transform enemies;
    
    private bool loadingScene;
    

    void Awake()
    {
        StartCoroutine(fader.FadeIn());
        player.SetInvulnerability(true);
    }

    private void Update()
    {
        if (enemies.childCount == 0 && !loadingScene)
        {
            fader.NextSceneCall("Combat Scene");
            loadingScene = true;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player.gameObject)
        {
            enemiesText.SetActive(true);
            hpBar.SetActive(true);
        }
    }
}
