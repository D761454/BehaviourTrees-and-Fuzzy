using UnityEngine;

[CreateAssetMenu(fileName = "PickupData", menuName = "ScriptableObjects/HealthPickup", order = 1)]

public class HealthPickUp : BasePickupEffect
{
    [SerializeField]
    float healthRestore = 5f;

    public override void ApplyEffect(GameObject target)
    {
        target.GetComponent<Health>().ApplyHealing(healthRestore);
    }        
}
