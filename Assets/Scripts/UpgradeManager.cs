using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [Header("Till Next")]
    [SerializeField] int tillNextUpgrade;
    [SerializeField] TextMeshProUGUI tillNextTxt;

    [Header("Upgrade UI")]
    [SerializeField] Transform upgradeParent;
    [SerializeField] GameObject upgradeOption;
    [SerializeField] Transform iconParent;
    [SerializeField] GameObject upgradeIcon;
    [HideInInspector] public bool choosing;
    private List<CurrentUpgrade> currentUpgrades = new List<CurrentUpgrade>();

    [Header("References")]
    [SerializeField] GameManager gameManager;
    [SerializeField] PlayerStats stats;

    [Header("Upgrade Options")]
    [SerializeField] List<Upgrade> upgrades;
    private List<Upgrade> upgradesAvailable;


    public void Start()
    {
        upgradesAvailable = new List<Upgrade>(upgrades);    
    }

    public void NewGame()
    {
        tillNextUpgrade = 2;
        stats.ResetStats();
        foreach (Transform child in iconParent)
            Destroy(child.gameObject);

        upgradesAvailable = new List<Upgrade>(upgrades);
        currentUpgrades.Clear();
        //TODO: undo special upgrades
    }

    public IEnumerator NextRoom(int levelNum)
    {
        tillNextUpgrade--;
        if (tillNextUpgrade <= 0)
        {
            //reset tillNextUpgrade
            tillNextUpgrade = (levelNum >= 8) ? 3 : 2;

            //show upgrade options + pause game
            upgradeParent.gameObject.SetActive(true);
            List<Upgrade> choices = GetRandomUpgrades(3);
            for (int i = 0; i < 3; i++)
            {
                GameObject option = Instantiate(upgradeOption, Vector3.zero, Quaternion.identity, upgradeParent);
                option.GetComponent<RectTransform>().anchoredPosition = new Vector3(-700 + 700*i, 0);
                UpgradeChoice script = option.GetComponent<UpgradeChoice>();
                script.upgrade = choices[i];
                script.upgradeManager = this;
            }
            choosing = true;
            yield return new WaitUntil(() => !choosing);
            
            foreach (Transform child in upgradeParent)
                Destroy(child.gameObject);
            upgradeParent.gameObject.SetActive(false);
        }
        tillNextTxt.text = "Next Upgrade: " + (levelNum + tillNextUpgrade);
    }


    private List<Upgrade> GetRandomUpgrades(int count)
    {
        List<Upgrade> randomUpgrades = new List<Upgrade>();
        List<int> usedIndices = new List<int>();
        
        //force explode to appear if available
        /*Upgrade explode = upgradesAvailable.Find(u => u.id == "bubbleExplode");
        if (explode != null)
        {
            randomUpgrades.Add(explode);
            usedIndices.Add(upgradesAvailable.IndexOf(explode));
            count--;
        }*/

        for (int i = 0; i < count && i < upgradesAvailable.Count; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, upgradesAvailable.Count);
            } while (usedIndices.Contains(randomIndex));
            
            usedIndices.Add(randomIndex);
            randomUpgrades.Add(upgradesAvailable[randomIndex]);
        }
        return randomUpgrades;
    }


    public void AddUpgrade(Upgrade u)
    {
        if (u.id.Contains("buff_"))
        {
            stats.UpgradeStat(u.id.Split('_')[1], u.strength);
        }
        else if (u.id == "noSpikes")
        {
            stats.spikeDamage.multiplier = 0;
            upgradesAvailable.RemoveAll(up => up.id == "noSpikes");
        }
        else if (u.id == "bubbleExplode")
        {
            stats.bubbleExplode = true;
            upgradesAvailable.RemoveAll(up => up.id == "bubbleExplode");
        }
        //implement other special upgrades here

        //show UI icon
        
        CurrentUpgrade cu = currentUpgrades.Find(c => c.id == u.id);
        if (cu != null)
        {
            if (cu.id.Contains("buff_"))
            {
                cu.count++;
                cu.text.text = "<size=32>x</size>" + cu.count;
            }
        }
        else
        {
            GameObject newIcon = Instantiate(upgradeIcon, Vector2.zero, Quaternion.identity, iconParent.transform);
            newIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(-650 + 160*currentUpgrades.Count, 0);
            newIcon.transform.GetChild(1).GetComponent<Image>().sprite = u.icon;
            TextMeshProUGUI txt = newIcon.transform.GetChild(2).GetComponent<TextMeshProUGUI>();
            if (!u.id.Contains("buff_"))
                txt.text = "";
            currentUpgrades.Add(new CurrentUpgrade(u.id, 1, txt));
        }
        choosing = false;
    }


    [System.Serializable]
    public class Upgrade
    {
        public string name;
        public string description;
        public string id;
        public float strength;
        public Sprite icon;
    }

    [System.Serializable]
    public class CurrentUpgrade
    {
        public string id;
        public int count;
        public TextMeshProUGUI text;
        //public GameObject icon;

        public CurrentUpgrade(string id, int count, TextMeshProUGUI text)
        {
            this.id = id;
            this.count = count;
            this.text = text;
        }
    }
}