using UnityEngine;
using System.Collections;

public class PlayerMeleeAttack : MonoBehaviour
{
    public float attackRange = 1f;          // Į �ֵθ��� ����
    public int damage = 1;                  // ���ݷ�
    public float attackCooldown = 0.5f;     // ���� ����
    public LayerMask enemyLayer;           // ���� ��� ���̾�

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

        // �� Ž�� �� ������
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (Collider2D hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        // ���� �ִϸ��̼� ���� ����
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void OnDrawGizmosSelected()
    {
        // �����Ϳ��� ���� ���� �ð�ȭ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
