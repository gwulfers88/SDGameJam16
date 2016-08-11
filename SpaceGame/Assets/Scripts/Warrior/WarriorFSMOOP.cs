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
    MachineGun _machine_Gun_Script;
    float timer;
    float shootDelay = 5f;
    ObjectPool pool;
    //public GameObject[] _nodes;
    //public Transform[] waypoints;
    //public Transform waypoint;
    //public int CurWayPoint = 6;
    //public int maxWaypoint = 0;
    public GameObject _player;
    EnemyHealth _enemyHealth;
    AudioSource _pewPew;
    #endregion
    // Use this for initialization

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _target = _player;
        pool = GetComponent<ObjectPool>();
        _pewPew = GetComponent<AudioSource>();
        //CurWayPoint = 0;
        //waypoint = waypoints[CurWayPoint];
        //maxWaypoint = waypoints.Length - 1;
        //SpawnState();
        _state.Init();

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * _fleeSpeed * Time.deltaTime;
        _player = GameObject.FindGameObjectWithTag("Player");
        _target = _player;
        if(_target)
        {
            Debug.Log("Enemy found target : " + _target);
            transform.LookAt(_player.transform);
            seek = true;
            seekWeight = 5;
            timer += Time.deltaTime;
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
            //Set distance to infinity, so first rock we look at is closest
            float distance = Mathf.Infinity;
            float d = 200f;
            //Look for nearest rock
            foreach (GameObject obj in objs)
            {
                if( d < distance)
                {
                    if (timer >= shootDelay)
                    {
                        Debug.Log("Player with ID " + PhotonNetwork.player.ID);
                        pool.spawn(transform.position + transform.forward * 10f, transform.rotation);
                        _pewPew.Play();
                        timer = 0;
                    }
                }
            }
            
        }
        if (_state)
        {
            if (_state.next != null)
            {
                _state.Finish();
                _state = _state.next;
                _state.Init();
            }

            _state.Update();
        }
        Locomotion();
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.collider.CompareTag("Player") || other.collider.tag == "Bullet")
        {
            _enemyHealth = GetComponent<EnemyHealth>();
            if (_enemyHealth)
            {
                _enemyHealth.Explode();
            }
            Debug.Log("Exploded!");
            Debug.Log("Alien got hit");
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
