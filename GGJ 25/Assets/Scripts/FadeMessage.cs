using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeMessage : MonoBehaviour
{
    [SerializeField]
    private Animation fade;
    public void nextScene()
    {
        SceneManager.LoadScene("Level Scene", LoadSceneMode.Single);
    }

    public void fadePlay()
    {
        fade.Play();
    }
}
