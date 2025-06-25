using UnityEngine;

public class Laser : MonoBehaviour
{

    [SerializeField] private float _speed = 8f;
    [SerializeField] private bool _isEnemyLaser = false;

    void Update()
    {
        if (_isEnemyLaser)
        {
            transform.Translate(Vector2.down * (_speed * Time.deltaTime));
        }
        else
        {
            transform.Translate(Vector3.up * (_speed * Time.deltaTime));
        }

        if (transform.position.y >= 8 || transform.position.y <= -8)
        {
            if (transform.parent != null)
            {
                Destroy(transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _isEnemyLaser == true)
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
                Destroy(this.gameObject);
            }
        }
    }
}
