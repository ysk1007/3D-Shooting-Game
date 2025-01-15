using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    public enum ItemType { HealthItem, MagazineItem, DropGunItem, Gold, Exp}

    public abstract void Use(GameObject entity);

    public abstract void PickUp(int index, int callerViewID);

    public abstract void ItemSetUp(int callerViewID, string weaponBaseJson = null);// WeaponBase weaponBase = null);

    public ItemType itemType;
}
