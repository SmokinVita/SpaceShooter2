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
    [SerializeField] private Image _magnetImg;

    [SerializeField] private Text _outOfAmmo;
    [SerializeField] private Text _ammoAmount;

    [SerializeField] private Text _incomingWaveText;
    [SerializeField] private Text _incomingBossText;
    [SerializeField] private Slider _bossHealth;
    [SerializeField] private Text _bossDefeatText;

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

    public void UpdateMagnetGauge(float currentMagnet)
    {
        _magnetImg.fillAmount = currentMagnet;
    }

    public void IncomingWave(int wave)
    {
        _incomingWaveText.text = $"Wave {wave} Incoming!";
        _incomingWaveText.enabled = true;
        StartCoroutine(WaveTextDeactivate());
    }

    IEnumerator WaveTextDeactivate()
    {
        yield return new WaitForSeconds(1.5f);
        _incomingWaveText.enabled = false;
    }

    public void IncomingBoss()
    {
        _incomingBossText.text = $"Boss incoming!";
        _incomingBossText.enabled = true;
        StartCoroutine(BossTextDeactivate());
    }

    IEnumerator BossTextDeactivate()
    {
        yield return new WaitForSeconds(2f);
        _incomingBossText.enabled = false;
    }

    public void ActiveBossHealth()
    {
        _bossHealth.gameObject.SetActive(true);
    }

    public void UpdateBossHealth(int health)
    {
        _bossHealth.value = health;
    }
    public void SetBossHealthBar(int heath)// Set the slider's max value to Boss's Max Health at the beginning
    {
        _bossHealth.maxValue = heath;
    }

    public void BossDefeatText()
    {
        _bossDefeatText.enabled = true;
        _restartText.enabled = true;
    }
}
