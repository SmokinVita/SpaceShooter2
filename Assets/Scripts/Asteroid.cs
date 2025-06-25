using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private float _rotateSpeed = 3f;
    [SerializeField] private GameObject _explosionPrefab;
    private SpawnManager _spawnManager;

    void Start()
    {
        _spawnManager = FindObjectOfType<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("SpawnManager is NULL!");
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * (_rotateSpeed * Time.deltaTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser"))
        {
            Destroy(collision.gameObject);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, .5f);
        }
    }
}
