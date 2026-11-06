using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 100;

    public int CurrentHealth { get; private set; }

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (damage <= 0 || CurrentHealth <= 0)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        Debug.Log($"Àû Ã¼·Â: {CurrentHealth}/{maxHealth}", this);

        if (CurrentHealth == 0)
            Destroy(gameObject);
    }
}