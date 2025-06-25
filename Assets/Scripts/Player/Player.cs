using JetBrains.Annotations;
using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float _horizontalInput;
    private float _verticalInput;
    private Vector3 _direction;


    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private GameObject _laser;
    [SerializeField] private GameObject _firePOS;
    [SerializeField] private int _ammoCount = 15;
    [SerializeField] private float _fireRate = .5f;
    private float _nextFire;

    [SerializeField] private int _lives = 3;

    [SerializeField] private SpawnManager _spawnManager;

    private bool _isTripleShotActive = false;
    [SerializeField] private GameObject _tripleShotPrefab;

    [SerializeField] private float _speedBoostAmount = 2f;

    [SerializeField] private int _shieldStrength = 3;
    [SerializeField] private GameObject _shield;
    [SerializeField] private SpriteRenderer _shieldRender;
    private bool _isShieldActive = false;

    [SerializeField] private GameObject _leftEngine, _rightEngine;

    [SerializeField] private int _score;

    [SerializeField] private UIManager _uiManager;
    //know about audiomanager
    //[SerializeField] private AudioManager _audio;
    [SerializeField] private AudioClip _laserSFX;
    [SerializeField] private AudioClip _explosionSFX;
    private AudioSource _audioSource;


    [SerializeField] private float _thrusterBoostAmount = 2f;
    [SerializeField] private float _thrusterTemp;
    [SerializeField] private float _maxThrusterTemp = 5f;
    private bool _isThrusterOverheating = false;

    [SerializeField] private GameObject _beam;

    [SerializeField] private CameraShake _cameraShake;

    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);

        _spawnManager = FindObjectOfType<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("SpawnManager is NULL!");

        _uiManager = FindObjectOfType<UIManager>();
        if (_uiManager == null)
            Debug.Log("UIManager is NULL!");

        _cameraShake = FindObjectOfType<CameraShake>();
        if (_cameraShake == null)
            Debug.Log("CameraShake is NULL!");

        /*_audio = FindObjectOfType<AudioManager>();
        if (_audio == null)
            Debug.Log("AudioManager is NULL!");
        */
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            Debug.Log("AudioSource on Player is NULL!");
        else
            _audioSource.clip = _laserSFX;


        _uiManager.AmmoAmountText(_ammoCount);
    }


    private void Update()
    {
        Movement();

        if (Input.GetKeyDown(KeyCode.H)) {
            Heal();
        }

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _nextFire && _ammoCount > 0)
            Shoot();
    }

    private void Movement()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _verticalInput = Input.GetAxis("Vertical");
        _direction = new Vector3(_horizontalInput, _verticalInput, 0);

        if (Input.GetKey(KeyCode.LeftShift) && !_isThrusterOverheating)
        {
            transform.Translate(_direction * ((_speed * _thrusterBoostAmount) * Time.deltaTime));
            _thrusterTemp += Time.deltaTime;
            
            if (_thrusterTemp >= _maxThrusterTemp)
            {
                Debug.Log("Thurster is overheating!");
                _isThrusterOverheating = true;
                StartCoroutine(ThrusterCooldownRoutine());
            }
        }
        else
        {
            transform.Translate(_direction * (_speed * Time.deltaTime));
            if(_thrusterTemp > 0 && !_isThrusterOverheating)
                _thrusterTemp -= Time.deltaTime;
        }

        _uiManager.ThrusterTempGauge(_thrusterTemp / _maxThrusterTemp, _isThrusterOverheating);

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, -3.7f, 0), 0);

        if (transform.position.x >= 11.3f)
        {
            transform.position = new Vector3(-11.3f, transform.position.y);
        }
        else if (transform.position.x <= -11.3f)
        {
            transform.position = new Vector3(11.3f, transform.position.y);
        }
    }

    IEnumerator ThrusterCooldownRoutine()
    {
        while (_isThrusterOverheating)
        {
            yield return new WaitForSeconds(1f);
            _thrusterTemp -= .5f;
            if (_thrusterTemp <= 0)
                _isThrusterOverheating = false;
        }
    }

    private void Shoot()
    {
        _ammoCount--;
        _uiManager.AmmoAmountText(_ammoCount);
        if (_ammoCount <= 0)
            _uiManager.AmmoText();

        _nextFire = Time.time + _fireRate;

        if (_isTripleShotActive == true)
        {
            Instantiate(_tripleShotPrefab, _firePOS.transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laser, _firePOS.transform.position, Quaternion.identity);
        }
        //player laser sfx
        //_audio.PlayLaserSFX();
        //would it be better for the class to hold the clip or audiomanager?
        _audioSource.Play();

    }

    public void Damage()
    {
        if (_isShieldActive == true)
        {
            _shieldStrength--;
            //change shield's transparncy

            _shieldRender.color = new Color(1f, 1f, 1f, (_shieldStrength / 3f));

            if (_shieldStrength <= 0)
            {
                _isShieldActive = false;
                _shield.SetActive(false);
            }
            return;
        }

        _lives--;
        _cameraShake.ShakeCamera();
        //display dmg engine
        if (_lives == 2)
        {
            _leftEngine.SetActive(true);
        }
        else if (_lives == 1)
        {
            _rightEngine.SetActive(true);
        }
        _uiManager.UpdateLives(_lives);
        if (_lives <= 0)
        {
            //_uiManager.DisplayGameOverText();
            _audioSource.clip = _explosionSFX;
            _audioSource.Play();
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void Heal()
    {
        _lives++;
        if(_lives >= 3)
        {
            _lives = 3;
        }
        _uiManager.UpdateLives(_lives);
        if (_lives == 3)
        {
            _leftEngine.SetActive(false);
        }
        else if (_lives == 2)
        {
            _rightEngine.SetActive(false);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speed *= _speedBoostAmount;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _speed /= _speedBoostAmount;
    }

    public void ShieldActivate()
    {
        _isShieldActive = true;
        _shield.SetActive(true);
        _shieldStrength = 3;
    }

    //method to add 10 to score!
    //communicate with UI to update score
    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    public void FillAmmo()
    {
        _ammoCount = 15;
        _uiManager.AmmoAmountText(_ammoCount);
    }

    public void ActiveBeam()
    {
        _beam.SetActive(true);
        StartCoroutine(BeamPowerDownRoutine());
    }

    IEnumerator BeamPowerDownRoutine()
    {
        yield return new WaitForSeconds(5f);
        _beam.SetActive(false);
    }
}
