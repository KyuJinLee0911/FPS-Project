using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    public UnitCode unitCode { get; }
    public float hp { get; set; }
    public float defence { get; set; }

    public Status()
    {

    }

    public Status (UnitCode unitCode, float hp, float defence)
    {
        this.unitCode = unitCode;
        this.hp = hp;
        this.defence = defence;
    }

    public Status SetUnitStatus(UnitCode unitCode)
    {
        Status status = null;

        switch(unitCode)
        {
            case UnitCode.UC_GUARDIAN :
                status = new Status(unitCode, 100, 10);
                break;

            case UnitCode.UC_COMMANDO :
                status = new Status(UnitCode.UC_COMMANDO, 110, 8);
                break;
        }

        return status;
    }
}
