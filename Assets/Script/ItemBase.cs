using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public abstract void Use(GameObject entity);

    public abstract void PickUp(int index);

    public abstract void SetUp(MemoryPool itemMemoryPool, WeaponBase weaponBase = null);
}
