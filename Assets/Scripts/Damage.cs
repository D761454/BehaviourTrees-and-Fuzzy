using System;
using UnityEngine;
using UnityEngine.Assertions;

public class ApplyDamage : MonoBehaviour
{
    public Action OnDamageDealt;

    [SerializeField]
    int m_Damage;

    [SerializeField]
    bool m_DestroyOnApply = false;

    [SerializeField]
    LayerMask m_CanDamage = 0;

#if DEBUG
    private void Start()
    {
        Assert.AreNotEqual(m_CanDamage, 0);
    }
#endif

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((m_CanDamage.value & 1 << collision.gameObject.layer) != 0)
        {
            Health mv = collision.gameObject.GetComponent<Health>();
            if (mv != null)
            {
                mv.ApplyDamage(m_Damage);
                OnDamageDealt?.Invoke();
            }
        }
        if (m_DestroyOnApply)
        {
            Destroy(gameObject);
        }
    }
}
