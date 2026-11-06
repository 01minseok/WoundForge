using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerMovement), typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField] private Slider healthBar;
    [SerializeField, Min(0f)] private float hitInvincibilityDuration = 0.5f;

    public int CurrentHealth { get; private set; }

    private PlayerMovement movement;
    private Rigidbody2D rb;
    private float invincibleUntil;

    private void UpdateHealthBar()
    {
        if (healthBar == null)
            return;

        healthBar.maxValue = maxHealth;
        healthBar.value = CurrentHealth;
    }

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
        CurrentHealth = maxHealth;
        UpdateHealthBar();
    }


    public void TakeDamage(int damage)
    {
        if (damage <= 0 || CurrentHealth <= 0)
            return;

        if (movement.IsInvincible || Time.time < invincibleUntil)
            return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        UpdateHealthBar();
        invincibleUntil = Time.time + hitInvincibilityDuration;

        Debug.Log($"체력: {CurrentHealth}/{maxHealth}", this);

        if (CurrentHealth == 0)
        {
            movement.enabled = false;
            rb.linearVelocity = Vector2.zero;
            Debug.Log("플레이어 사망", this);
        }
    }
}