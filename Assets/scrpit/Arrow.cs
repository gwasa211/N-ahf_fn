using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;
    public float lifeTime = 3f;
    public LayerMask enemyLayer;

    private Vector2 direction;

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if ((enemyLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }
}
