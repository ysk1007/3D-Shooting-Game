using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class ItemGun : ItemBase
{
    [Header("아이템 세팅")]
    [SerializeField] private WeaponSetting weaponSetting;

    [Header("아이템 회전 속도")]
    [SerializeField] private float moveDistance = 0.2f;
    [SerializeField] private float pinpongSpeed = 0.5f;
    [SerializeField] private float rotateSpeed = 50;

    [Header("아이템 세팅")]
    //[SerializeField] private WeaponInfoPopup weaponInfoPopup;

    WeaponSwitchSystem weaponSwitchSystem;

    ItemMemoryPool itemMemoryPool;
    private PhotonView photonView;

    WeaponBase weaponBase;
    WeaponBaseData data;

    private void Start()
    {
        
    }

    private IEnumerator Rotate()
    { 
        float y = transform.position.y;

        while (true)
        {
            // y축을 기준으로 회전
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

            // 처음 배치된 위치를 기준으로 y 위치를 위, 아래로 이동
            Vector3 position = transform.position;
            position.y = Mathf.Lerp(y, y + moveDistance, Mathf.PingPong(Time.time * pinpongSpeed, 1));
            transform.position = position;

            yield return null;
        }
    }

    public override void Use(GameObject entity)
    {

    }

    [PunRPC]
    public override void ItemSetUp(int callerViewID, string weaponBaseJson)
    {
        PhotonView callerView = PhotonView.Find(callerViewID);
        this.itemMemoryPool = callerView.GetComponent<ItemMemoryPool>();

        photonView = GetComponent<PhotonView>();
        photonView.RPC("ActivateObjectRPC", RpcTarget.AllBuffered, true);

        //int weaponCount = System.Enum.GetNames(typeof(WeaponName)).Length;
        //int randomIndex = Random.Range(0, weaponCount); // 0부터 weaponCount - 1까지의 랜덤 정수
        if (weaponBaseJson != null) {
            data = JsonUtility.FromJson<WeaponBaseData>(weaponBaseJson);
            weaponSetting = data.weaponSetting;
            GunMemoryPool.instance.SpawnGun(weaponSetting.WeaponName, this.transform);
        }
        else
            weaponSetting = GunMemoryPool.instance.SpawnGun((WeaponName)Random.Range(0, GunMemoryPool.instance.Weapons.Length), this.transform);
    }

    private void OnEnable()
    {
        StartCoroutine("Rotate");
    }

    [PunRPC]
    public override void PickUp(int index, int callerViewID)//WeaponSwitchSystem _weaponSwitchSystem)
    {
        PhotonView callerView = PhotonView.Find(callerViewID);
        this.weaponSwitchSystem = callerView.GetComponent<WeaponSwitchSystem>();
        //this.weaponSwitchSystem = _weaponSwitchSystem;
        if (weaponSwitchSystem.PickUpWeapon(weaponSetting, index))
        {
            itemMemoryPool.DeactivateItem(this.gameObject);
            GameObject gun = transform.GetComponentInChildren<WeaponBase>().gameObject;
            gun.GetComponent<WeaponBase>().MemoryPool?.DeactivateGun(gun);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().WeaponInfoPopup.SetUp(weaponSetting, this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            other.GetComponent<PlayerManager>().WeaponInfoPopup.init();
        }
    }

    // RPC를 통해 네트워크에서 비활성화 동기화
    [PunRPC]
    private void ActivateObjectRPC(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
