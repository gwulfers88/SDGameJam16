using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class WarriorFSMOOP : Steering
{
    public WarriorState _state;
    #region Warrior Data
    public float _speed = 3.0f;
    public float _fleeSpeed = 5.0f;
    public float _health;
    public float _gold;
    public float _maxHealth = 1000f;
    public float _gatherDistance = 1.0f;
    public float _resourceAmt = 1;
    public int _energyAmt = 1;
    public float _maxResourceAmt;
    public float _fatigue = 5f;
    public float _fatigueRate = 5.0f;
    public float _fleeWaitTime = 5.0f;
    public float _minHealthLimit;
    public int _number = 0;
    public GameObject _target;
    //public GameObject[] _nodes;
    //public Transform[] waypoints;
    //public Transform waypoint;
    //public int CurWayPoint = 6;
    //public int maxWaypoint = 0;
    public GameObject _player;
    EnemyHealth _enemyHealth;
    #endregion
    // Use this for initialization

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _target = _player;
        //CurWayPoint = 0;
        //waypoint = waypoints[CurWayPoint];
        //maxWaypoint = waypoints.Length - 1;
        SpawnState();
        _state.Init();

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * _fleeSpeed * Time.deltaTime;
        _player = GameObject.Find("Player1(Clone)");
        _target = _player;
        if(_target)
        {
            transform.LookAt(_player.transform);
            seek = true;
            seekWeight = 5;
        }
        if (_state.next != null)
        {
            _state.Finish();
            _state = _state.next;
            _state.Init();
        }

        _state.Update();
        Locomotion();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.tag == "Bullet")
        {
            Debug.Log("I'm hit");
        }
    }
    void SpawnState()
    {
        _number = Random.Range(1, 2);
        Debug.Log("State number is :" + _number);
        if (_number == 1)
        {
            _state = new AttackPlayers(this);
            Debug.Log("State is now :" + _state);
            return;
        }
    }

}
