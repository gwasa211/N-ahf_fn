using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f;
    private Vector2 direction;

    public int damage = 2;
    public int pierce = 1;

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1) Enemy
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            Vector2 knockback = direction.normalized;
            enemy.TakeDamage(damage, knockback);
            pierce--;
        }
        // 2) SkeletonEnemy
        else if (other.TryGetComponent<SkeletonEnemy>(out var skelEnemy))
        {
            Vector2 knockback = direction.normalized;
            skelEnemy.TakeDamage(damage, knockback);
            pierce--;
        }
        // 3) 그 외 비-트리거 콜라이더(벽 등) 맞으면 파괴
        else if (!other.isTrigger && !other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

        // 관통력 소모 후 파괴
        if (pierce <= 0)
        {
            Destroy(gameObject);
        }
    }

}
