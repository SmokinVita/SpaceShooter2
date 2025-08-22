using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    //speed
    [SerializeField] private float _speed = 3f;
    //health
    [SerializeField] private int _health = 100;
    private bool _isDead = false;
    private bool _attackFinished;
    //bool to check if can move
    [SerializeField] private bool _canMove = false;
    [SerializeField] private bool _moveRight = true;
    [SerializeField] private GameObject[] _wayPoints;
    private Animator _animator;

    //Missile from the sides
    [SerializeField] private GameObject[] _missleSpawnPoints;
    [SerializeField] private GameObject _missilePrefab;

    //Enemy Spawning Info
    [SerializeField] private SpawnManager _spawnManager;
    [SerializeField] private int _spawnAmount = 4;
    [SerializeField] private int _currentSpawnTime;

    //Regular Laser Fire
    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private GameObject[] _firePoints;
    [SerializeField] private int _amountOfLasersShot = 4;
    private int _currentLasersShot;

    //LaserWaterFall
    [SerializeField] private GameObject[] _pointsOfFire;
    [SerializeField] private GameObject[] _warnings;
    [SerializeField] private List<GameObject> _activePoints = new List<GameObject>();
    [SerializeField] private List<GameObject> _activeWarnings = new List<GameObject>();
    //[SerializeField] private 

    private UIManager _uiManager;
    private GameManager _gameManager;

    [SerializeField] private GameObject _shield;

    [SerializeField] private GameObject[] _explosions;
    private Collider2D _bossCollider;
    private SpriteRenderer _bossRender;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("SpawnManager is NULL!");

        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.Log("Boss Animator is NULL!");

        _uiManager = FindObjectOfType<UIManager>();
        if (_uiManager == null)
            Debug.Log("UIManager is NULL!");

        _gameManager = FindObjectOfType<GameManager>();
        if (_gameManager == null)
            Debug.Log("Game Manager is NULL!");

        _bossRender = GetComponent<SpriteRenderer>();
        if (_bossRender == null)
            Debug.Log("Boss SpriteRender is NULL!");  
        
        _bossCollider = GetComponent<Collider2D>();
        if (_bossCollider == null)
            Debug.Log("Boss Collider is NULL!");

        StartCoroutine(BossAttackCoolDownRoutine());

        
    }

    private void OnEnable()
    {
        _uiManager.UpdateBossHealth(_health);
        _uiManager.SetBossHealthBar(_health);
    }

    public void StartMovement()
    {
        Debug.Log("StartMovement");
        _animator.enabled = false;
        _canMove = true;
        _uiManager.ActiveBossHealth();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (_canMove)
        {
            if (_moveRight)
                transform.Translate(Vector2.right * (_speed * Time.deltaTime));
            else if (!_moveRight)
                transform.Translate(Vector2.left * (_speed * Time.deltaTime));

            ChangeDirection();
        }
    }

    private void ChangeDirection()
    {
        if (transform.position.x >= _wayPoints[0].transform.position.x)
        {
            Debug.Log("Hit waypoints 0");
            _moveRight = false;
        }
        else if (transform.position.x <= _wayPoints[1].transform.position.x)
        {
            _moveRight = true;
        }
    }

    IEnumerator BossAttackCoolDownRoutine()
    {
        yield return null;
        while (!_isDead)
        {
            _attackFinished = false;
            float randomCoolDown = Random.Range(6f, 15f);
            yield return new WaitForSeconds(randomCoolDown);
            _canMove = false;
            ActivateShields();
            //Fire an attack
            Attack();

            yield return new WaitUntil(() => _attackFinished);
            _canMove = true;
            DeactiveShields();
        }
    }

    private void Attack()
    {
        int randomAttack = Random.Range(0, 4);
        switch (randomAttack)
        {
            case 0://4 milssile's from the sides, 2 from each side
                //pick two from left 0 - 3
                int rightPoint = Random.Range(0, 2);
                int leftPoint = Random.Range(4, 6);
                StartCoroutine(MissileAttackRoutine(leftPoint, rightPoint));
                //pick two from right 4 - 7
                //Instantiate left 
                //instantiate right after 1.5 seconds -> IEnumerator?
                break;
            case 1://spawn enemies
                _currentSpawnTime++;
                StartCoroutine(_spawnManager.SpawnEnemiesRoutine(_currentSpawnTime * _spawnAmount));
                break;
            case 2://Lasers
                //Fire laser from Boss.
                FireLasers();
                break;
            case 3://Laser Water fall, leave open a gap
                LaserWaterFall();
                break;
            default:
                break;
        }
    }

    private void ActivateShields()
    {
        _shield.SetActive(true);
    }

    private void DeactiveShields()
    {
        _shield.SetActive(false);
    }

    private void Damage()
    {
        _health--;
        if (_health <= 0)
        {
            _bossCollider.enabled = false;
            _isDead = true;
            StopAllCoroutines();
            StartCoroutine(DeathAnimation());
            _canMove = false;
        }

        _uiManager.UpdateBossHealth(_health);
    }

    IEnumerator MissileAttackRoutine(int leftPoints, int rightPoints)
    {
        //display warning
        Debug.Log($"Left points: {leftPoints} : {leftPoints + 2}. Right Points: {rightPoints} : {rightPoints + 2}");
        yield return new WaitForSeconds(1f);
        GameObject rightMissile1 = Instantiate(_missilePrefab, _missleSpawnPoints[rightPoints].transform.position, Quaternion.Euler(Vector3.forward * 90));
        rightMissile1.GetComponent<Missile>().MissileMoveLeft(false);
        GameObject rightMissile2 = Instantiate(_missilePrefab, _missleSpawnPoints[rightPoints + 2].transform.position, Quaternion.Euler(Vector3.forward * 90));
        rightMissile2.GetComponent<Missile>().MissileMoveLeft(false);
        yield return new WaitForSeconds(1.5f);
        GameObject leftMissile1 = Instantiate(_missilePrefab, _missleSpawnPoints[leftPoints].transform.position, Quaternion.Euler(Vector3.forward * -90));
        leftMissile1.GetComponent<Missile>().MissileMoveLeft(true);
        GameObject leftMissile2 = Instantiate(_missilePrefab, _missleSpawnPoints[leftPoints + 2].transform.position, Quaternion.Euler(Vector3.forward * -90));
        leftMissile2.GetComponent<Missile>().MissileMoveLeft(true);
        yield return new WaitForSeconds(3f);
        _attackFinished = true;
    }

    public void FinishedAttack()
    {
        _attackFinished = true;
    }

    private void FireLasers()
    {
        StartCoroutine(FireLaserRoutine());
    }

    IEnumerator FireLaserRoutine()
    {
        _currentLasersShot = 0;

        while (_currentLasersShot < _amountOfLasersShot)
        {
            for (int i = 0; i < _firePoints.Length; i++)
            {
                GameObject laser = Instantiate(_laserPrefab, _firePoints[i].transform.position, Quaternion.identity);
                laser.GetComponent<Laser>().AssignEnemyLaser();
            }

            _currentLasersShot++;
            yield return new WaitForSeconds(.1f);
        }

        _attackFinished = true;
    }

    private void LaserWaterFall()
    {
        //Flash the warnings of where the lasers will fall
        //fire from selected points
        StartCoroutine(LaserWaterFallRoutine());
    }

    IEnumerator LaserWaterFallRoutine()
    {
        yield return null;
        _activePoints.Clear();
        _activeWarnings.Clear();
        _activeWarnings.Clear();
        int i = 0;
        int randomIndex = Random.Range(2, _pointsOfFire.Length);
        foreach (var points in _pointsOfFire)
        {
            _activePoints.Add(points);
            _activeWarnings.Add(points.transform.GetChild(0).gameObject);
        }

        _activeWarnings.RemoveAt(randomIndex);
        _activeWarnings.RemoveAt(randomIndex - 1);
        _activeWarnings.RemoveAt(randomIndex - 2);

        int y = 0;
        while (y < 5)
        {
            foreach (var warning in _activeWarnings)
            {
                warning.SetActive(true);
            }

            yield return new WaitForSeconds(1f);


            foreach (var warning in _activeWarnings)
            {
                warning.SetActive(false);
            }

            yield return new WaitForSeconds(1f);
            y++;
        }

        _activePoints.RemoveAt(randomIndex);
        _activePoints.RemoveAt(randomIndex - 1);
        _activePoints.RemoveAt(randomIndex - 2);

        while (i < 20)
        {
            foreach (var laserpoint in _activePoints)
            {
                GameObject laser = Instantiate(_laserPrefab, laserpoint.transform.position, Quaternion.identity);
                laser.GetComponent<Laser>().AssignEnemyLaser();
            }
            i++;
            yield return new WaitForSeconds(.1f);
        }

        _attackFinished = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_shield.activeSelf == true)
        {
            Destroy(other.gameObject);
            return;
        }

        if (other.CompareTag("Laser") || other.CompareTag("HomingMissile"))
        {

            Destroy(other.gameObject);
            //_anim.SetTrigger("OnEnemyDeath");
            //_audioSource.Play();
            Damage();
        }

        if (other.CompareTag("Beam"))
        {
            Damage();
            //_anim.SetTrigger("OnEnemyDeath");
            _isDead = true;
            //Destroy(_instantiatedPoints);

            //Set a cool down from hitting.
            Destroy(GetComponent<Collider2D>());
            Destroy(this.gameObject, 2.5f);
        }
    }

    IEnumerator DeathAnimation()
    {
        for (int i = 0; i < _explosions.Length; i++)
        {
            yield return new WaitForSeconds(1.5f);
            _explosions[i].SetActive(true);

        }
        Debug.Log("did this escape before fourth is finsih playing");
        yield return new WaitForSeconds(1f);
        _bossRender.enabled = false;
        yield return new WaitForSeconds(1f);
        _uiManager.BossDefeatText();
        Destroy(gameObject);
        _gameManager.UpdatePlayerStatus();
    }

}
