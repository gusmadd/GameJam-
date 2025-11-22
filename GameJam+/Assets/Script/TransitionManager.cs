using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("Fade Panel")]
    public Image blackScreen;

    [Header("Camera Effects")]
    public Camera mainCam;
    public float shakeIntensity = 0.15f;
    public float shakeDuration = 0.4f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    void Start()
    {
        StartCoroutine(InitializeAfterSceneLoad());
    }

    IEnumerator InitializeAfterSceneLoad()
    {
        yield return new WaitForEndOfFrame();

        mainCam = Camera.main;

        StartCoroutine(FadeInScene());
    }

    public IEnumerator FadeInScene()
    {
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime * 1.2f;
            blackScreen.color = new Color(0, 0, 0, t);
            yield return null;
        }
    }

    public void FadeOutAndLoadScene(string sceneName)
    {
        StartCoroutine(FadeOutCoroutine(sceneName));
    }

    private IEnumerator FadeOutCoroutine(string sceneName)
    {
        // Camera shake
        yield return StartCoroutine(CameraShake());

        // Fade to black
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * 1.2f;
            blackScreen.color = new Color(0, 0, 0, t);
            yield return null;
        }

        // Load scene
        yield return SceneManager.LoadSceneAsync(sceneName);

        // Setelah scene baru masuk → ambil camera baru
        mainCam = Camera.main;

        // Fade in scene baru
        yield return StartCoroutine(FadeInScene());
    }

    IEnumerator CameraShake()
    {
        Vector3 originalPos = mainCam.transform.localPosition;
        float t = 0f;

        while (t < shakeDuration)
        {
            t += Time.deltaTime;

            mainCam.transform.localPosition =
                originalPos + Random.insideUnitSphere * shakeIntensity;

            yield return null;
        }

        mainCam.transform.localPosition = originalPos;
    }
}
