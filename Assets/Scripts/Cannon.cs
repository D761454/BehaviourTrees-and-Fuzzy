using UnityEngine;

public class Cannon : MonoBehaviour
{
    public float m_MaxAmmo{ get; private set; }
    public float m_Ammo { get; private set; }

    float m_FireTime = 5f;
    public float m_FireTimer { get; private set; }
    float m_TurnRate = 40f;

    float m_FireDistance = 5f;
    float m_FireAngle = 3f;

    Transform m_Target;

    public GameObject m_CannonBall;

    public float AmmoRatio { get { return m_Ammo / m_MaxAmmo; } }

    public void Start()
    {
        m_FireTimer = 0f;
        m_MaxAmmo = 10f;
        m_Ammo = m_MaxAmmo;
    }

    public void AimAndFire()
    {
        Aim();
        Fire();
    }

    public void SetTarget(Transform target)
    { 
        m_Target = target; 
    }

    void Aim()
    {
        if (m_Target == null) return;

        Vector2 lineBetween = (m_Target.position - transform.position).normalized;

        float currentAngle = Mathf.Atan2(transform.up.y, transform.up.x);
        float targetAngle = Mathf.Atan2(lineBetween.y, lineBetween.x);
        float deltaAngle = Mathf.DeltaAngle(Mathf.Rad2Deg * currentAngle, Mathf.Rad2Deg * targetAngle);

        float maxTurnPerFrame = m_TurnRate * Time.fixedDeltaTime;
        deltaAngle = Mathf.Clamp(deltaAngle, -maxTurnPerFrame, maxTurnPerFrame);

        float newAngle = (Mathf.Rad2Deg * currentAngle) +  deltaAngle;

        transform.rotation = Quaternion.Euler(0, 0, newAngle - 90);
    }

    void Fire()
    {
        if (m_FireTimer > 0)
        {
            m_FireTimer -= Time.deltaTime;
            return;
        }

        if(m_Target != null && m_Ammo > 0)
        {
            Vector2 lineBetween = (m_Target.position - transform.position);

            if(lineBetween.magnitude < m_FireDistance)
            {
                lineBetween.Normalize();
                float angle = Mathf.Acos(Vector2.Dot(transform.up, lineBetween));

                if (angle <= m_FireAngle)
                {
                    Rigidbody2D rb = Instantiate(m_CannonBall, (Vector2)transform.position, Quaternion.identity).GetComponent<Rigidbody2D>();
                    Physics2D.IgnoreCollision(rb.GetComponent<Collider2D>(), transform.root.GetComponent<Collider2D>(), true);
                    rb.linearVelocity = lineBetween * 10f;
                    m_FireTimer = m_FireTime;
                    m_Ammo--;
                }
            }
        }
    }

    public void Reload()
    {
        m_Ammo = m_MaxAmmo;
    }

    public void ResetTime(){
        if (m_FireTimer != 0f){
            m_FireTimer = 0f;
        }
    }
}
