using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : Photon.MonoBehaviour
{
    public ExplosionPOOL explosionPool;
    public GameObject poolObj;
    public float health = 100;
    public int explosionEstimatedAmount = 10;
    public GameObject explossion; //explosion prefab, in case of explosion bullet turret
    public GameObject pool; //assign death explosion
    public bool destroyParent; //check this if you wish to destroy the parent object of this script
    public GameObject healthBar;
    public Canvas canvas;
    public Vector3 healthBarOffset;
    public Text _HQmessagesText;
    string _HQmessages;
    string _message1;

    private Transform target;
    private float maxHealth;
    private float healthBarMaxWidth;
    //public GameObject _levelManagerObj;
    //LevelManager _levelManagerScript;
    public GameObject _player;
    //HQ_Health _hqHealth;

    void Awake()
    {

    }
    void Start()
    {

        if (healthBar)
        {
            target = transform;
            healthBar = Instantiate(healthBar, transform.position, Quaternion.identity) as GameObject;
            if (!canvas && GameObject.FindGameObjectWithTag("World Canvas"))
            {
                canvas = GameObject.FindGameObjectWithTag("World Canvas").GetComponent<Canvas>();
            }
            Debug.Log("Canvas Found " + canvas);
            healthBar.transform.SetParent(canvas.transform, false);
            maxHealth = health;
            healthBarMaxWidth = healthBar.transform.localScale.x;
        }

    }

    public void Update()
    {
        poolObj = GameObject.Find("ExplosionPOOL");
        explosionPool = poolObj.GetComponent<ExplosionPOOL>();
        //_HQmessagesText = GameObject.FindGameObjectWithTag("messages").GetComponent<Text>();
        //Debug.Log("hq messages is found : " + _HQmessagesText);
        //_HQmessagesText.text = _HQmessages + System.Environment.NewLine + "WHAT THE FUCK";
        //HQmessagesText.text = _message1;

        //_levelManagerObj = GameObject.Find("LevelManager");
        //_levelManagerScript = _levelManagerObj.GetComponent<LevelManager>();

        if (healthBar)
        {
            _player = GameObject.Find("Player");
            //_hqHealth = _HQ.GetComponent<HQ_Health>();
            healthBar.transform.position = target.position + healthBarOffset;
            healthBar.transform.rotation = Camera.main.transform.rotation;
        }
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Explode();
            _message1 = "Enemy Died";
        }
        if (healthBar)
            CalculatePercentage();
    }

    public virtual void Explode()
    {
        GameObject explo = explosionPool.GetPooledObject();
        if (explo == null)
        {
            return;
        }

        explo.transform.position = gameObject.transform.position;
        explo.SetActive(true);
        Debug.Log("Enemy died");

        if (explo == null)
        {
            return;
        }

        Destroy(gameObject, 0.2f);
        Destroy(healthBar);
    }

    void CalculatePercentage()
    {
        float dec = health / maxHealth; //turn to decimal
        float decPerc = dec * 100; //get percentage of max health

        float perDec = decPerc / 100; //turn to decimal
        float result = perDec * healthBarMaxWidth; //get percentage of max health bar width

        Vector3 newScale = new Vector3(result, healthBar.transform.localScale.y, healthBar.transform.localScale.z);
        healthBar.transform.localScale = newScale;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            TakeDamage(20);
            Debug.Log("Took Damage");
            _HQmessages = "Enemy Took Damage";
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "HQ")
        {
            TakeDamage(100);
            //_hqHealth.TakeDamage(10);
            _HQmessages = "HQ Took Damage";
            Destroy(this.gameObject);
        }
    }
    //public void Pooling()
    //{
    //    explosionPool = new TurretSystem_ObjectPooler();

    //    pool = new GameObject();
    //    pool.name = "Pool " + gameObject.name;

    //    /* Mathf.CeilToInt(1 / rateOfexplosions);*/ //simply estimate the amount to pool. it will grow if its not enough
    //    if (explossion)
    //    {
    //        explosionPool.pooledObject = explossion;
    //        explosionPool.pooledAmount = explosionEstimatedAmount;
    //        explosionPool.Start();
    //        foreach (GameObject obj in explosionPool.pooledObjects)
    //        {
    //            obj.transform.parent = pool.transform;
    //        }
    //    }
    //}
}
