using UnityEngine;
using System.Collections;

public class MachineGun : Photon.MonoBehaviour
{
    public GameObject gun_Emitter_Object;
    public float weapon_Fire_Delay = 0.0F;
    public GameObject bullet_Impact_Prefab;
    public float damage = 25.0F;
    public Transform player;
    public float distance;
    private float weapon_Fire_Timer = 0.0F;

    public GameObject _enemyObj;
    EnemyHealth _enemyHealth;

    float timer;
    float shootDelay = .25f;
    ObjectPool pool;

    //public GameObject smoke;
    //public GameObject bigExplosion;
    //public Texture2D _aimTEX;

    public string objectDestructibleTag = "Player";

    // Use this for initialization
    void Start()
    {
        gun_Emitter_Object.GetComponent<ParticleEmitter>().emit = false;
        gun_Emitter_Object.GetComponent<AudioSource>();
        gun_Emitter_Object.GetComponent<ParticleEmitter>();
    }
    void Update()
    {

    }
    // Update is called once per frame
    public void Fire()
    {
        _enemyObj = GameObject.FindGameObjectWithTag("Player");
        pool = GetComponent<ObjectPool>();
        //_enemyHealth = _enemyObj.GetComponent<EnemyHealth>();
        weapon_Fire_Timer = 0.0F;
        gun_Emitter_Object.GetComponent<ParticleEmitter>().emit = true;
        //Debug.Log(weapon_Fire_Timer + "TIME is Working");
        //timer += Time.deltaTime;

        //if (timer >= shootDelay)
        //{
        //    Debug.Log("Player with ID " + PhotonNetwork.player.ID);
        //    pool.spawn(transform.position + transform.forward * 10f, transform.rotation);
        //    //_pewPew.Play();
        //    timer = 0;
        //}
        if (!gun_Emitter_Object.GetComponent<AudioSource>().isPlaying)
        {
            gun_Emitter_Object.GetComponent<AudioSource>().Play();
            Debug.Log("Audio is Playing");
        }

        RaycastHit hit;

        if (Physics.Raycast(gun_Emitter_Object.transform.position, gun_Emitter_Object.transform.forward, out hit))
        {
            Instantiate(bullet_Impact_Prefab, hit.point, Quaternion.LookRotation(hit.normal));
            hit.collider.SendMessageUpwards("ApplyDamage", damage, SendMessageOptions.DontRequireReceiver);
            if (hit.collider.tag == objectDestructibleTag)
            {
                Debug.Log("Its hitting the soldier...Should be shooting");
                //_enemyHealth.TakeDamage(damage);
                Debug.Log("DAMAGE : " + damage);
                //Instantiate(smoke, hit.collider.gameObject.transform.position, Quaternion.identity);
                //Instantiate(bigExplosion, hit.collider.gameObject.transform.position, Quaternion.identity);
                //Destroy(hit.collider.gameObject);
                Debug.Log("Hit Soldier");
            }

        }

        weapon_Fire_Timer += Time.deltaTime;
        //Debug.Log(weapon_Fire_Timer + "TIME is Working");
    }
    
}
