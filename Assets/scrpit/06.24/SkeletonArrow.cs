using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D), typeof(SpriteRenderer))]
public class SkeletonArrow : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 15f;
    public float lifeTime = 5f;

    [Header("Damage")]
    public int damage = 1;
    public float knockbackForce = 1.5f;

    Rigidbody2D rb;
    Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        // Dynamic + 중력 제거
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;

        // Trigger 콜라이더
        col.isTrigger = true;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    /// <summary>
    /// 한 번만 호출: 방향 설정 + 회전 + velocity 적용
    /// </summary>
    public void Init(Vector2 dir)
    {
        Vector2 n = dir.normalized;
        rb.velocity = n * speed;

        float angle = Mathf.Atan2(n.y, n.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out var player))
        {
            Vector2 knock = rb.velocity.normalized * knockbackForce;
            player.TakeDamage(damage, knock);
            Destroy(gameObject);
            return;
        }
        if (!other.isTrigger)
            Destroy(gameObject);
    }
}
