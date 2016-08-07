using UnityEngine;
using System.Collections;

public class Turret : Photon.MonoBehaviour
{
    public float _distance;
    public float _maxGuardRange;
    public GameObject _soldier;
    public Transform _player;

    MachineGun _machine_Gun_Script;
  

    void Start()
    {
        //_soldier = GameObject.Find("Soldier(Clone)");
        _machine_Gun_Script = GetComponent<MachineGun>();
    
        _maxGuardRange = 20.0f;
    }
    void Update()
    {
        _soldier = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Found Player : " + _soldier);
        _distance = Vector3.Distance(_soldier.transform.position, transform.position);
        if (_distance < _maxGuardRange)
        {
            Vector3 TargetDirection = _soldier.transform.position - transform.position; //Target direction we wish to go
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
}
