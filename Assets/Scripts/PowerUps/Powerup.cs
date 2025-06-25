using UnityEngine;

public class Powerup : MonoBehaviour
{

    [SerializeField] private float _speed = 3f;
    [SerializeField] private int _powerupID = 0;//0 = triple, 1 = Speed, 2 = Shields
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _explosionSFX;
    [SerializeField] private int _spawnWeight;

    void Update()
    {
        transform.Translate(Vector2.down * (_speed * Time.deltaTime));

        if (transform.position.y <= -5.8f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                switch (_powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldActivate();
                        break;
                    case 3:
                        player.FillAmmo();
                        break;
                    case 4:
                        player.Heal();
                        break;
                    case 5:
                        player.ActiveBeam();
                        break;
                }
                AudioSource.PlayClipAtPoint(_explosionSFX, transform.position);
                Destroy(this.gameObject);
            }
        }
    }

    public int SpawnWeight()
    {
        return _spawnWeight;
    }
}
