using UnityEngine;
using System.Collections;

public class PlayerMeleeAttack : MonoBehaviour
{
    public float attackRange = 1f;
    public int damage = 1;
    public float attackCooldown = 0.5f;
    public LayerMask enemyLayer;

    private bool isAttacking = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && !isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                Vector2 knockback = (enemy.transform.position - transform.position).normalized;
                enemy.TakeDamage(damage, knockback); // ✅ 넉백 방향 포함
            }
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
