using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private bool _isGameOver = false;

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
}
