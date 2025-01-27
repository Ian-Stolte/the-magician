using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeMessage : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    public IEnumerator NextScene(string sceneName)
    {
        for (float i = 0; i < 1; i += 0.01f)
        {
            canvasGroup.alpha = i;
            yield return new WaitForSeconds(0.01f);
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

    }
    public void NextSceneCall(string sceneName)
    {
        StartCoroutine(NextScene(sceneName));
    }
    public IEnumerator FadeIn()
    {
        for (float i = 1; i >= 0; i -= 0.01f)
        {
            canvasGroup.alpha = i;
            yield return new WaitForSeconds(0.01f);
        }
    }
}
