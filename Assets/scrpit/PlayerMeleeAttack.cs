using UnityEngine;
using System.Collections;

public class PlayerMeleeAttack : MonoBehaviour
{
    public float attackRange = 1f;          // 칼 휘두르는 범위
    public int damage = 1;                  // 공격력
    public float attackCooldown = 0.5f;     // 공격 간격
    public LayerMask enemyLayer;           // 공격 대상 레이어

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

        // 적 탐색 및 데미지
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // 공격 애니메이션 삽입 가능
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        // 에디터에서 공격 범위 시각화
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
