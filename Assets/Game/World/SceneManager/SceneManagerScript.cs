using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerScript : MonoBehaviour
{
    [SerializeField] private float animationTime;
    [SerializeField] private AnimationCurve animationCurve;
    [SerializeField] private SpriteRenderer fadeRenderer;
    public static SceneManagerScript instance { get; private set; }

    public void ChangeScene(string sceneName) {
        Time.timeScale = 1;
        StartCoroutine(SceneChangeRoutine(sceneName));
    }

    private IEnumerator SceneOpenRoutine() {
        Timer timer = new Timer(animationTime);
        timer.SetIndependentTime(true);
        while (!timer.ExecuteTimer()) {
            fadeRenderer.color = Color.Lerp(new Color(0, 0, 0, 0), Color.black, animationCurve.Evaluate(timer.GetTimeLeft() / animationTime));
            yield return null;
        }
    }

    private IEnumerator SceneChangeRoutine(string sceneName)
    {
        Timer timer = new Timer(animationTime);
        timer.SetIndependentTime(true);
        while (!timer.ExecuteTimer())
        {
            fadeRenderer.color = Color.Lerp(Color.black, new Color(0, 0, 0, 0), animationCurve.Evaluate(timer.GetTimeLeft() / animationTime));
            yield return null;
        }
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SceneOpenRoutine());
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
