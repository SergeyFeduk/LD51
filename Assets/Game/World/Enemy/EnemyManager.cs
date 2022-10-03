using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance { get; private set; }

    [SerializeField] private List<Transform> spawnPositions = new List<Transform>();
    [Header("Difficulty")]
    [SerializeField] private int maxEnemiesPerWave;
    [SerializeField] private int startingEnemiesPerWave;
    [SerializeField] private float addEnemiesPerWave;
    [Header("Imports")]
    [SerializeField] private GameObject groudEnemyPrefab;

    private List<EnemyAI> enemies = new List<EnemyAI>();
    private List<float> waveSequence = new List<float>();
    private float currentEnemiesPerWave;
    private Timer enemySequenceTimer = new Timer();

    public void RegenerateGrid() {
        foreach (EnemyAI enemy in enemies) {
            enemy.RegeneratePath();
        }
    }

    public void DestroyEnemy(EnemyAI enemy) {
        enemies.Remove(enemy);
    }

    public void GenerateWave()
    {
        if (enemies.Count > maxEnemiesPerWave * 2) return;
        enemySequenceTimer = new Timer(10);
        currentEnemiesPerWave = Mathf.Clamp(currentEnemiesPerWave + addEnemiesPerWave, startingEnemiesPerWave, maxEnemiesPerWave);
        waveSequence.Clear();
        for (int i = 0; i < Mathf.Floor(currentEnemiesPerWave); i++) {
            float time = Random.Range(0.0f, 10.0f);
            waveSequence.Add(time);
        }
        waveSequence.Sort();
        waveSequence.Reverse();
    }

    private void SpawnEnemy() {
        GameObject enemy = Instantiate(groudEnemyPrefab, spawnPositions[Random.Range(0, spawnPositions.Count)].position, Quaternion.identity);
        enemies.Add(enemy.GetComponent<EnemyAI>());
    }

    private void Update()
    {
        enemySequenceTimer.ExecuteTimer();
        if (waveSequence.Count == 0) return;
        if (waveSequence[0] > enemySequenceTimer.GetTimeLeft()) {
            waveSequence.RemoveAt(0);
            SpawnEnemy();
        }
    }

    private void Start()
    {
        currentEnemiesPerWave = startingEnemiesPerWave - addEnemiesPerWave;
        GenerateWave();
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
