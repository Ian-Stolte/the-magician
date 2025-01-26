using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLoad : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private Transform enemies;

    [SerializeField]
    private FadeMessage fader;

    private bool check;
    void Awake()
    {
        player.SetInvulnerability(true);
        check = false;
    }
    private void Update()
    {
        if (enemies.childCount == 0 && !check)
        {
            fader.NextSceneCall("Combat Scene");
            check = true;
        }

    }
}
