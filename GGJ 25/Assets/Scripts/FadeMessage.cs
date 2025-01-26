using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeMessage : MonoBehaviour
{
    [SerializeField]
    private Animation fade;
    public void nextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void fadePlay()
    {
        fade.Play();
    }
}
