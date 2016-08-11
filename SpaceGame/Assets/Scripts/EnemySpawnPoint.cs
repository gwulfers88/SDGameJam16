using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class EnemySpawnPoint : MonoBehaviour
{
    public GameObject _soldier;
    public GameObject _soliderSpawn;
    public float _spawnRate = 20.0f;
    public float _spawnTimer = 200.0f;
    public float _soldierVelocity = 5f;
    //public Text _numEnemiesSpawnedText;
    //public string _string;
    //public int _numEnemiesSpawned;



    // Use this for initialization
    void Start()
    {
        // _numEnemiesSpawned = 0;
        _spawnTimer = _spawnRate;
    }

    // Update is called once per frame
    void Update()
    {
        _spawnTimer += Time.deltaTime;
        // _string= Convert.ToString(_numEnemiesSpawned);
        //_numEnemiesSpawnedText.text = "Enemies : " + _string; 
        if (_spawnTimer >= _spawnRate)
        {
            GameObject s = Instantiate(_soldier, _soliderSpawn.transform.position, Quaternion.identity) as GameObject;
            //_numEnemiesSpawned++;
            s.GetComponent<Rigidbody>();
            s.GetComponent<WarriorState>();
            //DestroyObject(s, 100);
            _spawnTimer = 0;
        }

    }
}
