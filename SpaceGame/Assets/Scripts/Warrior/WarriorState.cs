using UnityEngine;
using System.Collections;

public class WarriorState : MonoBehaviour
{
    protected WarriorFSMOOP data;
    public WarriorState next = null;

    // Use this for initialization
    public WarriorState(WarriorFSMOOP _data)
    {
        data = _data;
    }

    public virtual void Init()
    {
        data._health = data._minHealthLimit;

    }

    // Update is called once per frame
    public virtual void Update()
    {

        //data.arrive = false;

        //Global transition
        //if (Vector3.Distance(data._player.transform.position, transform.position) < 2f)
        //{
        //    next = new Detonate(data);
        //}
        //data._health -= data._fatigue * Time.deltaTime * data._fatigueRate;
    }
    public virtual void Finish()
    {

    }
    protected void UpdateMove(GameObject obj)
    {
        //data._speed = 1.0f;
        ////Do state update
        //data.transform.LookAt(obj.transform);
        //data.ClearBehaviors();
        //data.arrive = true;
        //data.target = obj;
    }
    public void Damage(int _damage)
    {
        data._health -= _damage;
        Debug.Log("Im Hit im HIT!!! Said the Worker!");
    }

}
