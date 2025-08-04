using System.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;

    [SerializeField] private GameObject[] _powerupPrefab;
    private int _randomPowerUp;

    private bool _stopSpawningEnemies = false;
    private bool _stopSpawningPowerUps = false;
    private GameObject _selectedPowerup;


    [Header("Wave System")]
    [SerializeField] private int _maxWave =3;
    [SerializeField] private int _currentWave = 0;
    [Tooltip("The amount of enemies will be multiplied by the current wave!")]
    [SerializeField] private int _enemiesToSpawn = 3;
    private int _enemiesInScene;

    [SerializeField] private GameManager _gameManager;
    [SerializeField] private UIManager _uiManager;


    void Start()
    {
        
    }

    //method to start spawning!
    public void StartSpawning()
    {
        _currentWave++;
        
        StartCoroutine(SpawnPowerupRoutine());
        if(_currentWave == _maxWave)
        {
            Debug.Log("SpawningBoss!");
            _uiManager.IncomingBoss();
            return;
        }

        _uiManager.IncomingWave(_currentWave);
        StartCoroutine(SpawnEnemiesRoutine(_enemiesToSpawn * _currentWave));
        Debug.Log(_enemiesToSpawn * _currentWave);
        
    }

    public void StopSpawning()
    {
        //stop coroutines from spawning till meteorite is destroyed again.
        StopAllCoroutines();
        _stopSpawningPowerUps = false;
        _stopSpawningEnemies = false;
    }

    public void OnPlayerDeath()
    {
        _stopSpawningEnemies = true;
        _stopSpawningPowerUps = true;
    }


    private void PickPowerupToSpawn()
    {
        int totalWeight = 0;
        for (int i = 0; i < _powerupPrefab.Length; i++)
        {
            totalWeight += _powerupPrefab[i].GetComponent<Powerup>().SpawnWeight();
        }

        int randomNumber = Random.Range(0, totalWeight);

        foreach (var powerup in _powerupPrefab)
        {
            int weight = powerup.GetComponent<Powerup>().SpawnWeight();
            if (randomNumber <= weight)
            {
                _selectedPowerup = powerup;
                break;
            }
            randomNumber -= weight;
        }
    }

    IEnumerator SpawnEnemiesRoutine(int enemiesToSpawn)
    {
        yield return new WaitForSeconds(4f);
        while (_stopSpawningEnemies == false && enemiesToSpawn > 0)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(-9.3f, 9.3f), 8);
            int randomEnemy = Random.Range(0, _enemyPrefab.Length);
            GameObject enemy = Instantiate(_enemyPrefab[randomEnemy], spawnPoint, Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;
            enemiesToSpawn--;
            Debug.Log(enemiesToSpawn);
            yield return new WaitForSeconds(5);
        }

        Debug.Log("Enemies have stopped Spawning!");
        StartCoroutine(_gameManager.EnemyCheckRoutine(true));
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawningPowerUps == false)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));
            Vector3 spawnPoint = new Vector2(Random.Range(-9.3f, 9.3f), 8);
            PickPowerupToSpawn();
            Instantiate(_selectedPowerup, spawnPoint, Quaternion.identity);
        }
    }
}
