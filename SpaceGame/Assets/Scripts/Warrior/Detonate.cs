using UnityEngine;
using System.Collections;

public class Detonate : WarriorState
{
    EnemyHealth _enemyHealth;
    MachineGun _machineGun;
    public Detonate(WarriorFSMOOP data) : base(data)
    {

    }

    public override void Init()
    {
        //data._fatigue = 5f;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        if (next != null)
        {
            return;
        }
        Damage(100);
        _enemyHealth.Explode();
        Debug.Log("Detonatation Successfull");
        UpdateMove(data._player);
    }
}
