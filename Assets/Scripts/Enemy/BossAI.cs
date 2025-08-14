using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    //speed
    [SerializeField] private float _speed = 3f;
    //health
    [SerializeField] private int _health = 100;
    private bool _isDead = false;
    //bool to check if can move
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _moveRight = true;
    [SerializeField] private GameObject[] _wayPoints;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(BossAttackCoolDownRoutine());
    }

    // Update is called once per frame
    void Update()
    {


        Movement();
    }



    private void Movement()
    {
        if (_canMove)
        {
            if (_moveRight)
                transform.Translate(Vector2.right * (_speed * Time.deltaTime));
            else if (!_moveRight)
                transform.Translate(Vector2.left * (_speed * Time.deltaTime));

            ChangeDirection();
        }

        //if can move
        //move left to edge of scene, then back right. Repeat.
    }

    private void ChangeDirection()
    {
        if (transform.position.x >= _wayPoints[0].transform.position.x)
        {
            Debug.Log("Hit waypoints 0");
            _moveRight = false;
        }
        else if (transform.position.x <= _wayPoints[1].transform.position.x)
        {
            _moveRight = true;
        }
    }

    IEnumerator BossAttackCoolDownRoutine()
    {
        yield return null;
        while (!_isDead)
        {
            float randomCoolDown = Random.Range(5f, 20f);
            yield return new WaitForSeconds(randomCoolDown);
            _canMove = false;
            //Fire an attack
            Debug.Log("Fire!!");
            yield return new WaitForSeconds(5f);
            _canMove = true;
        }
    }
}



/*
 * Boss moves back and forth and will randomly stop fire an attack.
 * at Half health 2 shields with heal will appear around Boss
 * Some attacks will be spawning in enemies. 
 * attack missiles from the sides
 */
