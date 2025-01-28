using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Quit : MonoBehaviour
{
    [SerializeField] private GameObject fader;

    public void QuitToTitle()
    {
        Time.timeScale = 1;
        StartCoroutine(QuitCor());
    }

    private IEnumerator QuitCor()
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            fader.GetComponent<CanvasGroup>().alpha = i;
            yield return new WaitForSeconds(0.02f);
        }
        SceneManager.LoadScene("Title Screen");
    }
}
