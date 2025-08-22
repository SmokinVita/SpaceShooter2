using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    private bool _moveLeft = false;

    private void Start()
    {
        Destroy(this.gameObject, 10f);
    }

    void Update()
    {
        transform.Translate(Vector2.up * _speed * Time.deltaTime);
    }

    public void MissileMoveLeft(bool moveLeft)
        { _moveLeft = moveLeft; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
            {
                player.Damage();
                Destroy(this.gameObject);
            }
        }
    }
}
