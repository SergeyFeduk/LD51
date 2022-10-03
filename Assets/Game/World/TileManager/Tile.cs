using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Vector3 initialSize;
    [SerializeField] private float animationDuration;
    [SerializeField] private AnimationCurve animationCurve;
    [Header("Includes")]
    [SerializeField] private float animationDownOffset;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite warnSprite;

    private Vector3 initialPosition;
    public bool isWalkable = true;
    public bool isWarned = false;
    public void Warn() {
        spriteRenderer.color = new Color(1f, .8f, .8f, 1);
        spriteRenderer.sprite = warnSprite;
        spriteRenderer.sortingOrder = -1;
        isWarned = true;
    }

    public void Drop() {
        StartCoroutine(DropAnimation(animationDuration));
        isWalkable = false;
    }

    public void Regenerate() {
        spriteRenderer.color = new Color(1f, 1f, 1f, 1);
        spriteRenderer.sprite = normalSprite;
        spriteRenderer.sortingOrder = 0;
        StartCoroutine(RegenerateAnimation(animationDuration));
        isWalkable = true;
        isWarned = false;
    }

    private IEnumerator RegenerateAnimation(float duration) {
        Timer timer = new Timer(duration);
        transform.position = initialPosition;
        while (!timer.ExecuteTimer()) {
            transform.localScale = Vector3.Lerp(initialSize, Vector3.zero, animationCurve.Evaluate(timer.GetTimeLeft() / duration));
            yield return null;
        }
    }

    private IEnumerator DropAnimation(float duration)
    {
        Timer timer = new Timer(duration);

        while (!timer.ExecuteTimer())
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, initialSize, animationCurve.Evaluate(timer.GetTimeLeft() / duration));
            transform.position = Vector3.Lerp(initialPosition - new Vector3(0, animationDownOffset, 0), initialPosition, animationCurve.Evaluate(timer.GetTimeLeft() / duration));
            yield return null;
        }
    }

    private void Start()
    {
        initialPosition = transform.position;
    }
}
