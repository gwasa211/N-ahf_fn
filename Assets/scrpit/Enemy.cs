using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int maxHealth = 10;
    private int currentHealth;

    public float moveSpeed = 2f;
    public Transform target;

    public Sprite[] walkSprites;
    public Sprite[] deathSprites;
    public float frameRate = 0.2f;

    private int frameIndex = 0;
    private float frameTimer = 0f;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private bool isDead = false;

    public int damage = 1;
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (target == null && GameManager.Instance != null)
            target = GameManager.Instance.player.transform;
    }

    void Update()
    {
        if (isDead) return;

        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        }

        // 걷기 애니메이션
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            frameIndex = (frameIndex + 1) % walkSprites.Length;
            spriteRenderer.sprite = walkSprites[frameIndex];
        }
        if (target != null)
        {
            Vector2 dir = target.position - transform.position;
            spriteRenderer.flipX = dir.x < 0;
        }

    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
            StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;

        // 죽는 애니메이션
        for (int i = 0; i < deathSprites.Length; i++)
        {
            spriteRenderer.sprite = deathSprites[i];
            yield return new WaitForSeconds(frameRate);
        }
        MoneyManager.Instance.AddMoney(100);
        GameManager.Instance.AddMoney(100);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log("플레이어에게 데미지를 입힘");
            }
        }
    }
}
