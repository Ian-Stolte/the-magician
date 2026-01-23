using UnityEngine;

public class PlatformSettings : MonoBehaviour
{
    static PlatformSettings settings;
    public static PlatformSettings Get() { return settings; }
    void Awake() { settings = this; }

    [Header("Platform Bools")]
    public bool mobile;
    private bool inputDetected;
    public bool editorOverride;

    [Header("Rotation")]
    [SerializeField] GameObject rotateOverlay;
    [SerializeField] TMPro.TextMeshProUGUI debugText;
    private int width;
    private int height;


    void Start()
    {
        #if UNITY_EDITOR
            mobile = editorOverride;
        #elif UNITY_WEBGL
            mobile = IsMobileBrowser(); //change to unknown until first touch? (would need enum)
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

        //wait for first input to determine platform type
        if (!inputDetected && Input.touchCount > 0)
        {
            inputDetected = true;
            mobile = true;
        }
        else if (!inputDetected && Input.GetMouseButtonDown(0))
        {
            inputDetected = true;
            mobile = false;
        }
    }

    private bool IsMobileBrowser() //this is not accurate!
    {
        //float minDimension = Mathf.Min(Screen.width, Screen.height);
        //return minDimension <= 768 && Input.touchSupported;
        return Input.touchSupported;
    }

    void EvaluateOrientation()
    {
        bool landscape = Screen.width >= Screen.height * 1.5f;
        if (debugText)
            debugText.text = Screen.width + " x " + Screen.height;

        rotateOverlay.SetActive(!landscape);
        Time.timeScale = landscape ? 1f : 0f;
    }
}
