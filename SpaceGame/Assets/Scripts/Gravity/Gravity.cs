using UnityEngine;
using System.Collections;

public class Gravity : MonoBehaviour
{

    public GameObject AstrObj;	//Sun
    public GameObject _player;	//Planet one
    //public GameObject planetOneMoon;	//Planet's One Moon

    public float vAstrMass = 100.0f;
    public float planetOneMass = 10.0f;
    //public float planetOneMoonMass = 5.0f;

    public Vector3 planetOneVel;
    //public Vector3 planetOneMoonVel;

    public Vector3 planetOneAccel;
    //public Vector3 planetOneMoonAccel;

    public Vector3 planetOneForce;
    public Vector3 planetOneMoonForce;

    public float globalGravity = 6.67f;

    void  Start()
    {
        AstrObj = GameObject.Find("Asteroid").gameObject;
        _player = GameObject.Find("Player").gameObject;
        //planetOneMoon = GameObject.Find("Moon").gameObject;

        planetOneVel = new Vector3(3, 0, 0);
        //planetOneMoonVel = new Vector3(1, 0, 0);
    }

    void Update()
    {
        AstrObj = GameObject.Find("Asteroid").gameObject;
        _player = GameObject.Find("Player").gameObject;
        CalculateForce();
    }

    void CalculateForce()
    {
        //PlanetOne
        float planetOneDistToSun = Vector3.Distance(_player.transform.position, AstrObj.transform.position) + 1;
        Vector3 planetOneDirOfForce  = AstrObj.transform.position - _player.transform.position;
        planetOneDirOfForce = planetOneDirOfForce.normalized;

        planetOneForce = planetOneDirOfForce * globalGravity * vAstrMass * planetOneMass / (planetOneDistToSun * planetOneDistToSun);

        planetOneAccel = planetOneForce / planetOneMass;

        planetOneVel += planetOneAccel * Time.deltaTime;

        _player.transform.position += planetOneVel * Time.deltaTime;

    }
}
