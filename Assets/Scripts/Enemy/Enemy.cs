using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float _speed = 4f;
    private Animator _anim;

    [SerializeField] protected Player _player;
    [SerializeField] protected bool _isDead = false;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionSFX;

    [SerializeField] protected GameObject _laserPrefab;
    [SerializeField] private float _fireRate = 3f;
    protected float _canShoot = 3f;

    [SerializeField] protected int _enemyID = 0;

    private Vector3 _curveStartingPoint = new Vector3(-11.5f, 5.89f);
    [SerializeField] private Transform[] _curvePoints;
    [SerializeField] private GameObject[] _curvePrefabs;
    private GameObject _instantiatedPoints;
    float t;

    [SerializeField] private float _raycastDistance = 2f;
    [SerializeField] private LayerMask _powerupLayer;

    [SerializeField] private GameObject _shield;
    private bool _shieldActive;

    protected virtual void Start()
    {
        RandomShieldApplier();
        EnemySelector();

        _player = FindObjectOfType<Player>();
        if (_player == null)
            Debug.Log("Player is NULL!");

        _anim = GetComponent<Animator>();
        if (_anim == null)
            Debug.Log("Animator is NULL!");

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            Debug.Log("AudioSource on Enemy is NULL!");
        else
            _audioSource.clip = _explosionSFX;
    }

    private void RandomShieldApplier()
    {
        int r = Random.Range(0, 4);
        if (r == 2)
        {
            _shieldActive = true;
            _shield.SetActive(true);
        }
    }

    private void EnemySelector()
    {
        _enemyID = Random.Range(0, 2);
        if (_enemyID == 1)
        {
            transform.position = _curveStartingPoint;
            int randomPath = Random.Range(0, _curvePrefabs.Length);

            _instantiatedPoints = Instantiate(_curvePrefabs[randomPath], transform.position, Quaternion.identity);
            _curvePoints = new Transform[_instantiatedPoints.transform.childCount];
            for (int i = 0; i < _instantiatedPoints.transform.childCount; i++)
            {
                _curvePoints[i] = _instantiatedPoints.transform.GetChild(i);
            }
            _instantiatedPoints.transform.parent = null;

        }
    }

    protected virtual void Update()
    {
        if(_isDead) return;

        Movement();
        Shoot();

        
    }

    protected virtual void Movement()
    {
        switch (_enemyID)
        {
            case 0:
                transform.Translate(Vector3.down * (_speed * Time.deltaTime));
                if (transform.position.y <= -5.5f)
                {
                    float newXPOS = Random.Range(-9.4f, 9.4f);

                    transform.position = new Vector3(newXPOS, 7.43f);
                }
                break;
            case 1:

                if (t < 1f)
                {
                    t += Time.deltaTime / _speed;
                    Vector3 pos = Mathf.Pow(1 - t, 2) * _curvePoints[0].position + 2 * (1 - t) * t * _curvePoints[1].position + Mathf.Pow(t, 2) * _curvePoints[2].position;
                    transform.position = pos;
                    if (transform.position.x >= _curvePoints[2].position.x)
                    {
                        transform.position = _curveStartingPoint;
                        t = 0f;
                    }
                }

                break;
            case 2:
                break;
        }

    }

    protected virtual void Shoot()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, _raycastDistance, _powerupLayer);
        Debug.DrawRay(transform.position, Vector2.down * _raycastDistance, Color.green);
        if (hit)
        {
            _canShoot = 1;
        }

        if (Time.time >= _canShoot)
        {
            _fireRate = Random.Range(3f, 7f);
            _canShoot = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
                lasers[i].AssignEnemyLaser();
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(_shieldActive)
        {
            _shield.SetActive(false);
            _shieldActive = false;
            Destroy(other.gameObject);
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
            }
            _anim.SetTrigger("OnEnemyDeath");
            _audioSource.Play(); //what is _audioSource?.Player();
            _speed = 0;
            _isDead = true;
            Destroy(_instantiatedPoints);
            Destroy(this.gameObject, 2.5f);
        }

        if (other.CompareTag("Laser"))
        {
            Destroy(other.gameObject);
            if (_player != null)
            {
                _player.AddScore(10);
            }
            _anim.SetTrigger("OnEnemyDeath");
            _audioSource.Play();
            Destroy(GetComponent<Collider2D>());
            _speed = 0;
            _isDead = true;
            Destroy(_instantiatedPoints);
            Destroy(this.gameObject, 2.5f);
        }

        if (other.CompareTag("Beam"))
        {
            _anim.SetTrigger("OnEnemyDeath");
            _isDead = true;
            Destroy(_instantiatedPoints);
            Destroy(this.gameObject, 2.5f);
        }

        if(other.CompareTag("HomingMissile"))
        {
            _anim.SetTrigger("OnEnemyDeath");
            _isDead= true;
            Destroy(other);
            Destroy(this.gameObject, 2.5f);
        }
    }
}
