using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCollider : MonoBehaviour
{
    [SerializeField]
    private Collider attackCollider;                    // 근접 공격 충돌 감지
    private float damage;

    private void Awake()
    {
        attackCollider = GetComponent<Collider>();
        attackCollider.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartCollider(float damage)
    {
        this.damage = damage;
        attackCollider.enabled = true;

        StartCoroutine("DisablebyTime", 0.1f);
    }

    private IEnumerator DisablebyTime(float time)
    {
        yield return new WaitForSeconds(time);

        attackCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Status>().DecreaseHP(damage);
        }
    }
}
