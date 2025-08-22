using System.Collections;
using UnityEngine;

public class DodgeyType : Enemy
{

    [SerializeField] private float _beamFireTime;
    [SerializeField] private GameObject _laserBeam;
    private bool _canDodge = true;

    protected override void Start()
    {
        base.Start();
        if (_enemyID == 0)
            transform.position = new Vector3(11.3f, Random.Range(1, 5.75f));
        else if (_enemyID == 1)
            transform.position = new Vector3(-11.3f, Random.Range(1, 5.75f));

    }

    protected override void Update()
    {
        base.Update();
        Collider2D laser = Physics2D.OverlapBox(transform.position + new Vector3(0, -2f), new Vector2(1, 2), 0);
        if (laser != null && !_isDead)
        {
            if (laser.CompareTag("Laser") && _canDodge)
            {
                PickDirection();
                _canDodge = false;
                StartCoroutine(DodgeCoolDownRoutine());
            }
        }
    }

    IEnumerator DodgeCoolDownRoutine()
    {
        yield return new WaitForSeconds(2f);
        _canDodge = true;
    }

    protected override void Movement()
    {
        switch (_enemyID)
        {
            case 0:

                transform.Translate(Vector3.left * (_speed * Time.deltaTime));
                if (transform.position.x < -11.4f)
                    transform.position = new Vector3(11.3f, Random.Range(1, 5.75f));
                break;
            case 1:

                transform.Translate(Vector3.right * (_speed * Time.deltaTime));
                if (transform.position.x > 11.3f)
                    transform.position = new Vector3(-11.3f, Random.Range(1, 5.75f));
                break;
        }

    }

    private void PickDirection()
    {
        float randomDirection = Random.Range(0, 4);
        switch (randomDirection)
        {
            case 0:
                transform.position = new Vector2(transform.position.x + 3f, transform.position.y);
                break;
            case 1:
                transform.position = new Vector2(transform.position.x - 3f, transform.position.y);
                break;
            case 2:
                break;
        }
    }

    protected override void Shoot()
    {
        if (Time.time >= _canShoot)
        {
            _canShoot = Time.time + _beamFireTime;
            _laserBeam.SetActive(true);
            StartCoroutine(BeamCoolDownRoutine());
        }
    }

    private IEnumerator BeamCoolDownRoutine()
    {
        yield return new WaitForSeconds(3f);
        _laserBeam.SetActive(false);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);
        _laserBeam.SetActive(false);
    }
}
