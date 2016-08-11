using UnityEngine;
using System.Collections;

public class AsteroidSpawnPoint : MonoBehaviour
{

    public GameObject _asteroid1;
    public GameObject _asteroid2;
    public GameObject _asteroid3;
    public GameObject _asteroidSpawn;
    public float _spawnRate = 20.0f;
    public float _spawnTimer = 200.0f;
    public float _asteroidVelocity = 5f;
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
            int randomIndex = Random.Range(0, 2);
            if(randomIndex == 0)
            {
                GameObject s = Instantiate(_asteroid1, _asteroidSpawn.transform.position, Quaternion.identity) as GameObject;
                Vector3 something = s.GetComponent<Rigidbody>().velocity;
                
                DestroyObject(s, 1000);
            }
            if (randomIndex == 1)
            {
                GameObject b = Instantiate(_asteroid2, _asteroidSpawn.transform.position, Quaternion.identity) as GameObject;
                b.GetComponent<Rigidbody>();
                Vector3 something = b.GetComponent<Rigidbody>().velocity;
                DestroyObject(b, 1000);
            }
            if (randomIndex == 2)
            {
                GameObject c = Instantiate(_asteroid3, _asteroidSpawn.transform.position, Quaternion.identity) as GameObject;
                c.GetComponent<Rigidbody>();
                Vector3 something = c.GetComponent<Rigidbody>().velocity;
                DestroyObject(c, 1000);
            }
            
            _spawnTimer = 0;
        }

    }
}
