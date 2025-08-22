using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    private bool _isGameOver = false;
    private SpawnManager _spawnManager;
    [SerializeField] private GameObject[] _currentActiveEnemies;
    private bool _checkingForEnemies = false;

    [SerializeField] private GameObject _astroid;

    private void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("SpawnManager is Null!");

    }

    void Update()
    {
        if (_isGameOver == true && Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Application.isEditor)
            {
                Debug.Log("Tried to close application!");
            }
            else
            {
                Application.Quit();
            }
        }
    }

    public void UpdatePlayerStatus()
    {
        _isGameOver = true;
    }

    public IEnumerator EnemyCheckRoutine(bool checkForEnemies)
    {
        _checkingForEnemies = checkForEnemies;

        while (_checkingForEnemies)
        {
            _currentActiveEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            yield return new WaitForSeconds(1f);

            if(_currentActiveEnemies.Length <= 0)
            {
                _spawnManager.StopSpawning();
                Instantiate(_astroid);
                _checkingForEnemies = false;
            }
        }
    }
    
}
