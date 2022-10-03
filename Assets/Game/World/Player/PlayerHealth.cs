using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHp;
    [SerializeField] private float tileTestOffset;
    [SerializeField] private Shake damageShake;
    [SerializeField] private float deathTime;

    [Header("Import")]
    [SerializeField] private GameObject robotDeathScreen;
    [SerializeField] private GameObject fallDeathScreen;
    [SerializeField] private TMP_Text robotKillsText;
    [SerializeField] private TMP_Text fallKillsText;

    public UnityEvent<int> damagedEvent;
    private int hp;

    private bool isAlive = true;

    public void Die(bool robotCaused) {
        if (!isAlive) return; 
        isAlive = false;
        StartCoroutine(DeathSequence(robotCaused));
    }

    private IEnumerator DeathSequence(bool robotCaused) {
        Timer timer = new Timer(deathTime);
        while (!timer.ExecuteTimer()) {
            yield return null;
        }
        switch (robotCaused)
        {
            case true:
                robotDeathScreen.SetActive(true);
                robotKillsText.text = string.Format("You killed {0} robots", Player.instance.counter.GetKills());
                break;
            case false:
                fallDeathScreen.SetActive(true);
                fallKillsText.text = string.Format("You killed {0} robots", Player.instance.counter.GetKills());
                break;
        }
        Time.timeScale = 0;
    }

    public void Damage(int amount) {
        CameraEffects.instance.Shake(damageShake);
        hp -= amount;
        damagedEvent.Invoke(hp);
        if (hp <= 0) {
            Die(true);
        }
    }

    private void Update()
    {
        if (!isOffTile()) Die(false);
    }

    private void Start()
    {
        hp = maxHp;
    }

    private bool isOffTile()
    {
        int tilesOn = 0;
        if (TileManager.instance.TestTile(transform.position))
        {
            tilesOn++;
        }
        Vector3[] positions = new Vector3[] { new Vector3(-1, -1), new Vector3(1, -1), new Vector3(-1, 1), new Vector3(1, 1) };
        foreach (Vector3 pos in positions) {
            if (TileManager.instance.TestTile(transform.position + pos * tileTestOffset))
            {
                tilesOn++;
            }
        }
        return tilesOn >= 2;
    }

}
