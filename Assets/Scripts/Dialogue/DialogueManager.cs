using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Newtonsoft.Json;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    [Header("Dialogue")]
    //public GameObject dialogueBox;
    //public TextMeshProUGUI txt;
    public Transform dialogueParent;
    public GameObject textPrefab;
    public float typeSpeed;
    public JSONParser parser;
    private bool skip;
    private GameObject textObj;

    [Header("Coroutines")]
    public IEnumerator playMultipleCor;
    public IEnumerator playCor;


    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            skip = true;
        }*/
    }

    //Used to play multiple lines in sequence, pausing waitTime seconds between each
    public void PlayMultiple(string[] lines, float waitTime)
    {
        //StopDialogue();
        playMultipleCor = PlayMultipleCor(lines, waitTime);
        StartCoroutine(playMultipleCor);
    }

    //The associated coroutine for PlayMultiple()
    public IEnumerator PlayMultipleCor(string[] lines, float waitTime)
    {
        foreach (string s in lines)
        {
            if (playCor != null)
                StopCoroutine(playCor);
            //playCor = SpawnDialogue(s, waitTime, dialogueParent);
            //yield return playCor;
            yield return SpawnDialogue(s, waitTime, dialogueParent);
        }
    }

    //Used to spawn a line of dialogue as a new prefab, then pause for waitTime seconds
    public IEnumerator SpawnDialogue(string line, float waitTime, Transform parent)
    {
        if (textObj)
            Destroy(textObj);
        Vector2 offset = new Vector2(Random.Range(-70, 70), Random.Range(0, 70));
        float rotation = Random.Range(-20, 20);
        textObj = Instantiate(textPrefab, Vector2.zero, Quaternion.Euler(0, 0, rotation), parent);
        textObj.GetComponent<RectTransform>().anchoredPosition = offset;
        textObj.GetComponent<TextMeshProUGUI>().text = line;
        yield return new WaitForSeconds(waitTime);
        //TODO: fade it out nicely before destroying?
        Destroy(textObj);
    }
    
    //Used to type out a single line of dialogue letter by letter, pausing waitTime seconds at the end
    /*public IEnumerator PlayDialogue(string line, float waitTime)
    {
        skip = false;
        txt.text = "";
        dialogueBox.SetActive(true);
        
        bool addingHTML = false;
        string html = "";

        //Type out dialogue
        foreach (char c in line)
        {
            //Don't type HTML content 1-by-1
            if (c == '<')
            {
                addingHTML = true;
                html = "<";
            }
            else if (c == '>')
            {
                addingHTML = false;
                txt.text += html + ">";
            }
            else if (addingHTML)
            {
                html += c;
            }

            else if (c == '`') //Use ` characters for pauses, so don't actually type them out
            {
                if (!skip)
                    yield return new WaitForSeconds(0.15f * typeSpeed);
            }
            else
            {
                txt.text += c;
                if (!skip)
                {
                    if (c == '.' || c == ',')
                        yield return new WaitForSeconds(0.15f * typeSpeed);
                    else if (c == ' ')
                        yield return new WaitForSeconds(0.08f * typeSpeed);
                    else
                        yield return new WaitForSeconds(0.04f * typeSpeed);
                }
            }
        }
        if (line[line.Length - 1] == '—') //Shorten the waitTime when ending on an en dash
            waitTime *= 0.5f;

        //Pause for waitTime seconds unless skip triggered
        skip = false;
        while (!skip && waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            yield return null;
        }

        dialogueBox.SetActive(false);
        txt.text = "";
    }*/

    //Play dialogue by querying from JSON data (requires the associated JSON parser)
    public string[] PlayByID(string sheetName, string ID, int variation, float waitTime, bool play = true)
    {
        string[] lines = parser.FindByID(sheetName, ID, variation);
        if (play)
            PlayMultiple(lines, waitTime);
        return lines;
    }

    //Used to stop ongoing dialogue
    /*public void StopDialogue()
    {
        if (playCor != null)
            StopCoroutine(playCor);
        if (playMultipleCor != null)
            StopCoroutine(playMultipleCor);
        dialogueBox.SetActive(false);
    }*/
}