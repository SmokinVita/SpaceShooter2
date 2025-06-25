using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _score;
    [SerializeField] private Image _livesImage;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    [SerializeField] private Image _thrusterImg;

    [SerializeField] private Text _outOfAmmo;
    [SerializeField] private Text _ammoAmount;

    private GameManager _gameManager;

    private void Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null)
            Debug.Log("GameManager is NULL!");

        _score.text = "Score: " + 0;
        _gameOverText.enabled = false;
        _restartText.enabled = false;
    }

    //update text on screen
    public void UpdateScore(int score)
    {
        _score.text = "Score: " + score;
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives < 0)
            return;
        //display img sprite
        //give it a new one based on the currentLives index
        _livesImage.sprite = _livesSprites[currentLives];

        if (currentLives <= 0)
        {
            DisplayGameOverText();
        }
    }

    public void DisplayGameOverText()
    {
        _gameManager.UpdatePlayerStatus();
        _gameOverText.enabled = true;
        _restartText.enabled = true;
        StartCoroutine(GameOverFlickerRoutine());
    }

    //cause game over text to flicker like arcade games.
    private IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(.5f);
            _gameOverText.enabled = false;
            _restartText.enabled = false;
            yield return new WaitForSeconds(.5f);
            _gameOverText.enabled = true;
            _restartText.enabled = true;
        }
    }

    public void ThrusterTempGauge(float currentTemp, bool isThrusterOverHeating)
    {
        _thrusterImg.fillAmount = currentTemp;

        if (isThrusterOverHeating)
            _thrusterImg.color = Color.red;
        else
            _thrusterImg.color = Color.Lerp(Color.green, Color.red, currentTemp);
    }

    public void AmmoText()
    {
        Debug.Log("Ammo Text Called");

        if (_outOfAmmo.IsActive())
            _outOfAmmo.enabled = false;
        else
            _outOfAmmo.enabled = true;

    }

    public void AmmoAmountText(int amount)
    {
        _ammoAmount.text = $"Ammo: {amount}";
    }

}
