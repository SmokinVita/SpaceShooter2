using UnityEngine;

public class SmartType : Enemy
{
    [SerializeField] private GameObject _backwardsFirePoint;
    [SerializeField] private float _backwardsCanFire = 2f;
    [SerializeField] private float _backFirerate = 2f;


    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {

        base.Update();
    }

    protected override void Shoot()
    {

        if (_player == null)
            return;

        if (_player.transform.position.y > transform.position.y && Time.time >= _backwardsCanFire)
        {
            _backwardsCanFire = Time.time + _backFirerate;
            Instantiate(_laserPrefab, _backwardsFirePoint.transform.position, Quaternion.identity);
        }
        else
            base.Shoot();
    }
}
