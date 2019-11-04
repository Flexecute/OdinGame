using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Transform FirePoint { get; set; }

    public virtual void Attack(Vector3 direction)
    {
    }

    public virtual float getAttackRate()
    {
        return 1f;
    }

    public virtual int getShootableLayerMask() {
        return 0;
    }

    public virtual void setFirePoint(Transform newFirePoint, GameObject player)
    {
        FirePoint = newFirePoint;
    }

}
