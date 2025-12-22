using UnityEngine;

public class PlatformSettings : MonoBehaviour
{
    static PlatformSettings settings;
    public static PlatformSettings Get() { return settings; }
    void Awake() { settings = this; }

    [Header("Platform Bools")]
    [HideInInspector] public bool mobile;
    public bool editorOverride;

    [Header("Rotation")]
    [SerializeField] private GameObject rotateOverlay;
    private int width;
    private int height;


    void Start()
    {
        #if UNITY_EDITOR
            mobile = editorOverride;
        #elif UNITY_WEBGL
            mobile = IsMobileBrowser();
        #elif UNITY_ANDROID || UNITY_IOS
            mobile = true;
        #else
            mobile = false;
        #endif

        width = Screen.width;
        height = Screen.height;

        EvaluateOrientation();
    }

    void Update()
    {
        if (Screen.width != width || Screen.height != height)
        {
            width = Screen.width;
            height = Screen.height;
            EvaluateOrientation();
        }
    }

    private bool IsMobileBrowser()
    {
        if (Input.touchSupported)
            return true;

        float minDimension = Mathf.Min(Screen.width, Screen.height);
        return minDimension <= 768;
    }

    void EvaluateOrientation()
    {
        bool landscape = Screen.width >= Screen.height * 1.5f;

        //gameRoot.SetActive(landscape);
        rotateOverlay.SetActive(!landscape);
        Time.timeScale = landscape ? 1f : 0f;
    }
}
