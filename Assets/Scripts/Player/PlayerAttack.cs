using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement), typeof(PlayerHealth))]
public class PlayerAttack : MonoBehaviour
{
    [SerializeField, Min(1)] private int damage = 20;
    [SerializeField, Min(0f)] private float attackCooldown = 0.4f;
    [SerializeField, Min(0f)] private float attackDistance = 0.8f;
    [SerializeField, Min(0.01f)] private float attackRadius = 0.6f;
    [SerializeField] private Transform attackVisual;
    [SerializeField, Min(0.01f)] private float attackVisualDuration = 0.1f;

    private float attackVisualTimeLeft;
    private PlayerMovement movement;
    private PlayerHealth health;
    private float nextAttackTime;
    private Camera mainCamera;

    private void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        health = GetComponent<PlayerHealth>();
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (attackVisual != null && attackVisualTimeLeft > 0f)
        {
            attackVisualTimeLeft -= Time.deltaTime;

            if (attackVisualTimeLeft <= 0f)
                attackVisual.gameObject.SetActive(false);
        }

        if (Mouse.current == null || health.CurrentHealth <= 0)
            return;

        if (movement.IsInvincible || Time.time < nextAttackTime)
            return;

        if (!Mouse.current.leftButton.wasPressedThisFrame)
            return;

        Vector2 direction = GetAttackDirection();

        if (direction == Vector2.zero)
            return;

        nextAttackTime = Time.time + attackCooldown;

        Vector2 center = (Vector2)transform.position
        + direction * attackDistance;

        if (attackVisual != null)
        {
            attackVisual.position =
                new Vector3(center.x, center.y, transform.position.z);

            attackVisual.localScale = Vector3.one * (attackRadius * 2f);
            attackVisual.gameObject.SetActive(true);
            attackVisualTimeLeft = attackVisualDuration;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(center, attackRadius);
        var damagedEnemies = new HashSet<EnemyHealth>();

        foreach (Collider2D hit in hits)
        {
            EnemyHealth enemy = hit.GetComponentInParent<EnemyHealth>();

            if (enemy != null && damagedEnemies.Add(enemy))
                enemy.TakeDamage(damage);
        }

        Debug.Log("기본 공격", this);
    }

    private void OnDrawGizmosSelected()
    {
        Vector2 direction = Application.isPlaying
            ? GetAttackDirection()
            : Vector2.down;

        Vector2 center = (Vector2)transform.position
            + direction * attackDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, attackRadius);
    }

    private Vector2 GetAttackDirection()
    {
        if (mainCamera == null || Mouse.current == null)
            return Vector2.zero;

        Vector2 screenPosition = Mouse.current.position.ReadValue();
        float depth = transform.position.z - mainCamera.transform.position.z;

        Vector3 mouseWorld = mainCamera.ScreenToWorldPoint(
            new Vector3(screenPosition.x, screenPosition.y, depth));

        return ((Vector2)mouseWorld - (Vector2)transform.position).normalized;
    }

}