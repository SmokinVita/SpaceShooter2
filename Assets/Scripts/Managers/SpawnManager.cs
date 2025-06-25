using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject _enemyContainer;

    [SerializeField] private GameObject[] _powerupPrefab;
    private int _randomPowerUp;

    private bool _stopSpawning = false;
    private GameObject _selectedPowerup;

    void Start()
    {
        //StartCoroutine(SpawnEnemiesRoutine());
        //StartCoroutine(SpawnPowerupRoutine());
    }

    //method to start spawning!
    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemiesRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
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

    IEnumerator SpawnEnemiesRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            Vector3 spawnPoint = new Vector3(Random.Range(-9.3f, 9.3f), 8);
            GameObject enemy = Instantiate(_enemyPrefab, spawnPoint, Quaternion.identity);
            enemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5);
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3f);
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(Random.Range(3f, 7f));
            Vector3 spawnPoint = new Vector2(Random.Range(-9.3f, 9.3f), 8);
            PickPowerupToSpawn();
            Instantiate(_selectedPowerup, spawnPoint, Quaternion.identity);
        }
    }
}
