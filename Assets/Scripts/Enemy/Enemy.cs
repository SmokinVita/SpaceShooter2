using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    private Animator _anim;

    [SerializeField] private Player _player;

    private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionSFX;

    [SerializeField] private GameObject _laserPrefab;
    [SerializeField] private float _fireRate = 3f;
    private float _canShoot = 3f;

    void Start()
    {
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

    void Update()
    {
        Shoot();

        transform.Translate(Vector3.down * (_speed * Time.deltaTime));
        if (transform.position.y <= -5.5f)
        {
            float newXPOS = Random.Range(-9.4f, 9.4f);

            transform.position = new Vector3(newXPOS, 7.43f);
        }
    }

    private void Shoot()
    {
        if (Time.time >= _canShoot)
        {
            _fireRate = Random.Range(3f, 7f);
            _canShoot = Time.time + _fireRate;
            GameObject enemyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();
            Debug.Log("fired!");

            for (int i = 0; i < lasers.Length; i++)
                lasers[i].AssignEnemyLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (_player != null)
            {
                _player.Damage();
            }
            _anim.SetTrigger("OnEnemyDeath");
            _audioSource.Play(); //what is _audioSource?.Player();
            _speed = 0;
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
            Destroy(this.gameObject, 2.5f);
        }

        if(other.CompareTag("Beam"))
        {
            _anim.SetTrigger("OnEnemyDeath");
            Destroy(this.gameObject, 2.5f);
        }
    }
}
