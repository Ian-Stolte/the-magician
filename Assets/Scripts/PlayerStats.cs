using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public bool bubbleExplode;

    [System.Serializable]
    public class Stat
    {
        public float baseValue;
        public float multiplier;

        public float val => baseValue * multiplier;
    }
    public Stat speed;
    public Stat fanPower;
    public Stat bubbleLife;
    public Stat bubbleSize;
    public Stat bounceSpeed;
    public Stat spikeDamage;

    private Dictionary<string, Stat> stats;
    private void Awake()
    {
        stats = new Dictionary<string, Stat>
        {
            { "speed", speed },
            { "fanPower", fanPower },
            { "bubbleLife", bubbleLife },
            { "bubbleSize", bubbleSize },
            { "bounceSpd", bounceSpeed },
            { "spikeDmg", spikeDamage }
        };
    }

    public void ResetStats()
    {
        foreach (Stat s in stats.Values)
        {
            s.multiplier = 1f;
        }
        bubbleExplode = false;
    }

    public void UpgradeStat(string name, float strength)
    {
        stats[name].multiplier += strength;
    }
}