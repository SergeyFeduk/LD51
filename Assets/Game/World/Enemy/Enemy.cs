using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float maxHp;
    [SerializeField] private Shake deathShake;
    [SerializeField] private EnemyAI myAi;
    [SerializeField] private float tileTestOffset;

    [Header("Imports")]
    [SerializeField] private float pitchVariance;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private GameObject deathPs;
    [SerializeField] private Transform appearanceTransform;
    private float hp;

    public void Die() {
        // Explosion animation goes here
        CameraEffects.instance.Shake(deathShake);
        EnemyManager.instance.DestroyEnemy(GetComponent<EnemyAI>());
        AudioManager.instance.PlaySound(deathClip, pitchVariance);
        Player.instance.counter.AddKill();
        Instantiate(deathPs, appearanceTransform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    public void Damage(float amount) {
        hp -= amount;
        AudioManager.instance.PlaySound(damageClip, pitchVariance);
        if (hp <= 0) Die();
    }

    private void Update()
    {
        if (myAi.currentState != State.ApproachingGrid && !myAi.isJumping && !isOffTile()) Die();
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
        foreach (Vector3 pos in positions)
        {
            if (TileManager.instance.TestTile(transform.position + pos * tileTestOffset))
            {
                tilesOn++;
            }
        }
        return tilesOn >= 2;
    }
}
