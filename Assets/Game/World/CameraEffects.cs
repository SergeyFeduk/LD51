using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shake {
    public float amount;
    public float duration;
    public bool fadeOut;
}

public class CameraEffects : MonoBehaviour
{
    [SerializeField] private float smoothness;
    public static CameraEffects instance { get; private set; }
    private Vector3 velocity;

    private Vector2 targetPosition;

    public void Shake(float amount, float duration, bool fadeOut) {
        StartCoroutine(ShakeRoutine(amount,duration, fadeOut));
    }

    public void Shake(Shake shake) {
        StartCoroutine(ShakeRoutine(shake.amount, shake.duration, shake.fadeOut));
    }

    private IEnumerator ShakeRoutine(float amount, float duration, bool fadeOut) {
        Timer timer = new Timer(duration);
        timer.SetIndependentTime(true);
        while (!timer.ExecuteTimer()) {
            float xOff = (Random.value * 2 - 1) * amount;
            float yOff = (Random.value * 2 - 1) * amount;
            float dampValue = fadeOut ? Mathf.Lerp(1,0, timer.GetTimeLeft() / duration) : 1;
            transform.position += new Vector3(xOff, yOff, 0) * dampValue;
            transform.position = new Vector3(transform.position.x, transform.position.y, -10);
            yield return null;
        }
        transform.position = targetPosition;
    }

    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothness);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    private void Start()
    {
        targetPosition = transform.position;
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
        }
    }
}
