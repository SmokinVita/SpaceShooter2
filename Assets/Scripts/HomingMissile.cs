using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    //currentTarget
    //list to hold all targets
    [SerializeField] private float _speed = 3f;
    private GameObject _currentTarget;
    [SerializeField] private GameObject[] _targets;
    private float _distance;

    void Start()
    {
        LocateTarget();
    }

    void Update()
    {
        //move toward target
        //if target becomes NULL find next target.
        if(_currentTarget == null)
        {
            LocateTarget();
            transform.Translate(Vector2.up * (_speed * Time.deltaTime));
        }
        else
        {
            Vector2 direction = _currentTarget.transform.position - transform.position;
            transform.rotation = Quaternion.LookRotation(transform.forward, direction);
            transform.Translate(Vector2.up *(_speed * Time.deltaTime));
        }
    }

    //method to find targets and select closest
    [ContextMenu("FindEnemies")]
    private void LocateTarget()
    {
        _targets = GameObject.FindGameObjectsWithTag("Enemy");
        _distance = Mathf.Infinity;

        if (_targets != null)
        {
            foreach (var enemy in _targets)
            {
                Vector2 diff = enemy.transform.position - transform.position;
                float currentDistance = diff.sqrMagnitude;
                if (currentDistance < _distance)
                {
                    _currentTarget = enemy;
                    _distance = currentDistance;
                }
            }
        }
    }
}
