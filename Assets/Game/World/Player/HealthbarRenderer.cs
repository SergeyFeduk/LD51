using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Heart {
    public Sprite sprite;
    public int hpValue;
}

public class HealthbarRenderer : MonoBehaviour
{
    private const int heartAmount = 3;

    [Header("Imports")]
    [SerializeField] List<Heart> hearts = new List<Heart>();
    [SerializeField] private GameObject heartPrefab;

    List<Image> heartRenderers = new List<Image>();

    private void Start()
    {
        Player.instance.heath.damagedEvent.AddListener(UpdateBar);
        for (int i = 0; i < heartAmount; i++) {
            Image renderer = Instantiate(heartPrefab, transform.position, Quaternion.identity).GetComponent<Image>();
            renderer.transform.SetParent(transform);
            renderer.transform.localScale = Vector3.one;
            heartRenderers.Add(renderer);
        }
    }

    public void UpdateBar(int value) {
        for (int i = 0; i < heartAmount; i++) {
            int hpValue = Mathf.Clamp(value - (i * 4),0, 4);
            heartRenderers[i].sprite = hearts.Find(delegate(Heart x) { return x.hpValue == hpValue; }).sprite;
        }
    }
}
