using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPool : Photon.MonoBehaviour
{
    public string objToSpawn;
    List<GameObject> pool;
    
    public int poolSize = 0;
    int startSize = 5;
    const int maxSize = 10;

	// Use this for initialization
	void Start ()
    {
        //if (photonView.isMine)
        {
            pool = new List<GameObject>();

            while (poolSize != startSize)
            {
                Debug.Log("Adding to pool");
                addToPool(PhotonNetwork.Instantiate(objToSpawn, Vector3.zero, Quaternion.identity, 0));
            }
        }
	}
	
    void addToPool(GameObject obj, bool active = false)
    {
        obj.SetActive(active);
        pool.Add(obj);
        poolSize++;
    }

    public void spawn(Vector3 position, Quaternion rotation)
    {
        foreach(GameObject obj in pool)
        {
            if(!obj.activeSelf)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
                obj.SetActive(true);
                
                return;
            }
        }

        if (poolSize < maxSize)
        {
            addToPool(Instantiate(Resources.Load(objToSpawn), position, rotation) as GameObject, true);
        }
    }
}
