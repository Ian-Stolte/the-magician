using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLoad : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;
    void Awake()
    {
        player.SetInvulnerability(true);
    }
}
