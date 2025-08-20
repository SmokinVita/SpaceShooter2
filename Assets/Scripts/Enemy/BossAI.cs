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


    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("SpawnManager is NULL!");

        _animator = GetComponent<Animator>();
        if (_animator == null)
            Debug.Log("Boss Animator is NULL!");

        StartCoroutine(BossAttackCoolDownRoutine());
    }

    public void StartMovement()
    {
        Debug.Log("StartMovement");
        _animator.enabled = false;
        _canMove = true;
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
            float randomCoolDown = Random.Range(5f, 20f);
            yield return new WaitForSeconds(randomCoolDown);
            _canMove = false;
            //Fire an attack
            Attack();
            Debug.Log("Fire!!");
            //yield return new WaitForSeconds(5f);
            yield return new WaitUntil(() => _attackFinished);
            _canMove = true;
        }
    }

    private void Attack()
    {
        int randomAttack = 4;//Random.Range(0, 5);
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
            case 3://drop down bomb's to explode
                break;
            case 4://Laser Water fall, leave open a gap
                LaserWaterFall();
                break;
            default:
                break;
        }
    }

    private void ActivateShields()
    {

    }

    private void DeactiveShields()
    {

    }

    private void Damage()
    {
        _health--;
        //if health is half active 2 shields.
        if (_health <= 0)
        {
            //show explosion animation
            //destroy Boss
        }
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
                Instantiate(_laserPrefab, _firePoints[i].transform.position, Quaternion.identity);
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
        int randomIndex = Random.Range(0, _pointsOfFire.Length);
        Debug.Log($"Got {randomIndex}");
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
}
/*
 * Boss moves back and forth and will randomly stop fire an attack.
 * at Half health 2 shields with heal will appear around Boss
 * Some attacks will be spawning in enemies. 
 * attack missiles from the sides
 */
