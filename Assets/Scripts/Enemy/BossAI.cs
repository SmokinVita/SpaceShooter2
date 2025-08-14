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
    private bool _attackFinished;
    //bool to check if can move
    [SerializeField] private bool _canMove = true;
    [SerializeField] private bool _moveRight = true;
    [SerializeField] private GameObject[] _wayPoints;


    [SerializeField] private GameObject[] _missleSpawnPoints;
    [SerializeField] private GameObject _missilePrefab;

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
            _attackFinished = false;
            //Fire an attack
            Attack();
            Debug.Log("Fire!!");
            //yield return new WaitForSeconds(5f);
            yield return new WaitUntil(() => _attackFinished);
            _canMove = true;
        }
    }

    private void Attack()
    {
        int randomAttack = 0;//Random.Range(0, 5);
        switch (randomAttack)
        {
            case 0://4 milssile's from the sides, 2 from each side
                //pick two from left 0 - 3
                int rightPoint = Random.Range(0, 2);   
                int leftPoint = Random.Range(4, 6);
                StartCoroutine(MissileAttackRoutine(leftPoint,rightPoint));
                //pick two from right 4 - 7
                //Instantiate left 
                //instantiate right after 1.5 seconds -> IEnumerator?
                break;
            case 1://spawn enemies from top or side
                break;
            case 2://Lasers
                break;
            case 3://drop down bomb's to explode
                break;
            case 4://
                break;

            default:
                break;
        }
    }

    private void Damage()
    {
        _health--;
        //if health is half active 2 shields.
        if (_health <= 0)
        {
            //show explosion animation
            //destroy Boss
        }
    }

    IEnumerator MissileAttackRoutine(int leftPoints, int rightPoints)
    {
        //display warning
        Debug.Log($"Left points: {leftPoints} : {leftPoints + 2}. Right Points: {rightPoints} : {rightPoints +2}");
        yield return new WaitForSeconds(1f);
        GameObject rightMissile1 = Instantiate(_missilePrefab, _missleSpawnPoints[rightPoints].transform.position, Quaternion.Euler(Vector3.forward * 90));
        rightMissile1.GetComponent<Missile>().MissileMoveLeft(false);
        GameObject rightMissile2 = Instantiate(_missilePrefab, _missleSpawnPoints[rightPoints + 2].transform.position, Quaternion.Euler(Vector3.forward * 90));
        rightMissile2.GetComponent<Missile>().MissileMoveLeft(false);
        yield return new WaitForSeconds(1.5f);
        GameObject leftMissile1 = Instantiate(_missilePrefab, _missleSpawnPoints[leftPoints].transform.position, Quaternion.Euler(Vector3.forward * -90));
        leftMissile1.GetComponent<Missile>().MissileMoveLeft(true);
        GameObject leftMissile2 = Instantiate(_missilePrefab, _missleSpawnPoints[leftPoints + 2].transform.position, Quaternion.Euler(Vector3.forward * -90));
        leftMissile2.GetComponent<Missile>().MissileMoveLeft(true);
        yield return new WaitForSeconds(3f);
        _attackFinished = true;
    }
}
/*
 * Boss moves back and forth and will randomly stop fire an attack.
 * at Half health 2 shields with heal will appear around Boss
 * Some attacks will be spawning in enemies. 
 * attack missiles from the sides
 */
