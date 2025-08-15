using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class AggressiveType : Enemy
{

    [SerializeField] private float _distanceToFocusPlayer = 2f;
    private float _distance;
    [SerializeField] private Quaternion _startRotation;

    protected override void Start()
    {
        base.Start();
        _startRotation = transform.rotation;
    }

 
    protected override void Update()
    {
        base.Update();
    }

    protected override void Movement()
    {
        if(_player == null)
        {
            base.Movement();
            return;
        }

        _distance = Vector3.Distance(transform.position, _player.transform.position);
        if (_distance < _distanceToFocusPlayer)
        {
            Vector2 direction = _player.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(Vector3.forward, -direction);
            transform.Translate(direction * (_speed * Time.deltaTime));
        }
        else if(_distance > _distanceToFocusPlayer) 
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, _startRotation, 1);
            base.Movement();
        }
    }

}
