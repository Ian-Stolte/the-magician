using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeMessage : MonoBehaviour
{
    public void nextScene()
    {
        SceneManager.LoadScene("Level Scene", LoadSceneMode.Single);
    }
}
