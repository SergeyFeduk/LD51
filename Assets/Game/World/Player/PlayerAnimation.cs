using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] private List<Sprite> front = new List<Sprite>();
    [SerializeField] private List<Sprite> back = new List<Sprite>();
    [SerializeField] private List<Sprite> side = new List<Sprite>();

    [SerializeField] private int fps;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private int frame = 0, maxValue = 0;
    private SpriteDirData data;

    public void AnimateWalk(Vector2 movementDirection) {
        int dx = movementDirection.x > 0 ? 1 : -1;
        if (Mathf.Abs(movementDirection.x) < 0.1f) dx = 0;
        int dy = movementDirection.y > 0 ? 1 : -1;
        if (Mathf.Abs(movementDirection.y) < 0.1f) dy = 0;

        data = GenerateSpriteDirData(dx, dy);
        maxValue = data.dir == SpriteDir.Side ? side.Count : data.dir == SpriteDir.Back ? back.Count : front.Count;
        data.index = frame;
        if (dx == 0 && dy == 0) {
            //data = new SpriteDirData(SpriteDir.Front, 0, false);
            data.index = 0;
        }
        SetSprite(data);
    }

    private IEnumerator FramerateRoutine() {
        Timer timer = new Timer();
        timer.SetTimerFrequency(fps);
        while (!timer.ExecuteTimer()) {
            yield return null;
        }
        frame = (frame + 1) % maxValue;
        StartCoroutine(FramerateRoutine());
    }

    private void Start()
    {
        StartCoroutine(FramerateRoutine());
    }

    private void SetSprite(SpriteDirData data) {
        switch (data.dir) {
            case SpriteDir.Front:
                spriteRenderer.sprite = front[data.index];
                break;
            case SpriteDir.Back:
                spriteRenderer.sprite = back[data.index];
                break;
            case SpriteDir.Side:
                spriteRenderer.flipX = !data.isRight;
                spriteRenderer.sprite = side[data.index];
                break;
        }
    }

    private SpriteDirData GenerateSpriteDirData(int dx, int dy) {
        if (dy == 1) {
            return new SpriteDirData(SpriteDir.Back, 0, false);
        }
        if (dx == 1 || dx == -1) {
            return new SpriteDirData(SpriteDir.Side, 0, dx == 1);
        }
        if (dy == -1)
        {
            return new SpriteDirData(SpriteDir.Front, 0, false);
        }
        return data;
    }
}
