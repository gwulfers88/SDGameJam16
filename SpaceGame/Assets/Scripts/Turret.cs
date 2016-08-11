using UnityEngine;
using System.Collections;

public class Turret : Photon.MonoBehaviour
{
    public float _distance;
    public float _maxGuardRange= 20.0f;
    public GameObject _players;
    public Transform _player;
    EnemyHealth _enemyHealth;

    MachineGun _machine_Gun_Script;
    float timer;
    float shootDelay = .25f;
    ObjectPool pool;

    void Start()
    {
        //_soldier = GameObject.Find("Soldier(Clone)");
        _machine_Gun_Script = GetComponent<MachineGun>();
        pool = GetComponent<ObjectPool>();
        //_maxGuardRange = 20.0f;
    }
    void Update()
    {
        //timer += Time.deltaTime;

        ////if (timer >= shootDelay)
        ////{
        //    Debug.Log("Player with ID " + PhotonNetwork.player.ID);
        //    pool.spawn(transform.position + transform.forward * 10f, transform.rotation);
        //    //_pewPew.Play();
        //    timer = 0;
        ////}
        _players = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Found Player : " + _players);
        _distance = Vector3.Distance(_players.transform.position, transform.position);
        //Debug.Log("Turret distance is : " + _distance);
        if (_distance < _maxGuardRange)
        {
            Vector3 TargetDirection = _players.transform.position - transform.position; //Target direction we wish to go
            float RotSpeed = 2f * Time.deltaTime;    //Rotation speed
            Vector3 newDir = Vector3.RotateTowards(transform.forward, TargetDirection, RotSpeed, 0);    //New direction we want to go to
            newDir.Normalize(); //Normalize the new direction ( between 0 and 1 );
            transform.rotation = Quaternion.LookRotation(newDir);   //Rotate towards the new direction 
            _machine_Gun_Script.Fire();
            Debug.Log("Turret script is firing");
        }
        if (_distance > _maxGuardRange)
        {
            _machine_Gun_Script.gun_Emitter_Object.GetComponent<ParticleEmitter>().emit = false;
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //if (_meleeSettings.CanMelee)
        //{
        // Draw helpers: Show angle lines in Scene view to see how big melee state radius is.
        Gizmos.DrawWireSphere(transform.position, _maxGuardRange);

        // Draw helpers: Show angle lines in Scene view to see how big melee angle range is.
        Gizmos.color = Color.yellow;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawLine(Vector3.zero, Quaternion.AngleAxis(-0.5f * 90.0f, Vector3.up) * (_maxGuardRange * Vector3.forward));
        Gizmos.DrawLine(Vector3.zero, Quaternion.AngleAxis(0.5f * 90.0f, Vector3.up) * (_maxGuardRange * Vector3.forward));
        //}
    }
    void OnCollisionEnter(Collision other)
    {
        _enemyHealth = GetComponent<EnemyHealth>();
        _enemyHealth.Explode();
        Debug.Log("Exploded!");
        Debug.Log("Alien hit");
        if (other.collider.tag == "Bullet")
        {
            _enemyHealth = GetComponent<EnemyHealth>();
            _enemyHealth.Explode();
            Debug.Log("Exploded!");
            Debug.Log("Alien got hit");
        }
    }
    
}
