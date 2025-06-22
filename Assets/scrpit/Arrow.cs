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
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                Vector2 knockback = direction.normalized;
                enemy.TakeDamage(damage, knockback); // ✅ 넉백 포함
                pierce--;

                if (pierce <= 0)
                    Destroy(gameObject);
            }
        }
        else if (!other.isTrigger && !other.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
