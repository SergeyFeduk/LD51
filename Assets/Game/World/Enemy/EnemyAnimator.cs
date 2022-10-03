using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpriteDir {
    Front,
    Back,
    Side
}

public struct SpriteDirData {
    public SpriteDir dir;
    public int index;
    public bool isRight;
    public SpriteDirData(SpriteDir dir, int index, bool isRight)
    {
        this.dir = dir;
        this.index = index;
        this.isRight = isRight;
    }
}

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private List<Sprite> front = new List<Sprite>();
    [SerializeField] private List<Sprite> back = new List<Sprite>();
    [SerializeField] private List<Sprite> side = new List<Sprite>();

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Transform rendererTransform;
    private bool isFacingRight = true;
    private bool isAnimationChangeBlocked = false;

    public void SetSprite(SpriteDirData data) {
        switch (data.dir) {
            case SpriteDir.Front:
                spriteRenderer.sprite = front[data.index];
                break;
            case SpriteDir.Back:
                spriteRenderer.sprite = back[data.index];
                break;
            case SpriteDir.Side:
                isFacingRight = data.isRight;
                spriteRenderer.sprite = side[data.index];
                spriteRenderer.flipX = !data.isRight;
                break;
        }
    }

    public void SetRendererTransform(Vector3 position) {
        rendererTransform.localPosition = position;
    }

    public SpriteDirData GetDirByDelta(int dx, int dy) {
        if (dx == 1) {
            return new SpriteDirData(SpriteDir.Side, 0, true);
        }
        if (dx == -1)
        {
            return new SpriteDirData(SpriteDir.Side, 0, false);
        }
        if (dy == 1)
        {
            return new SpriteDirData(SpriteDir.Front, 0, false);
        }
        if (dy == -1)
        {
            return new SpriteDirData(SpriteDir.Back, 0, false);
        }
        return new SpriteDirData(SpriteDir.Side, 0, true);
    }
}
