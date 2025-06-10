using UnityEngine;
using System.Collections;
public class Player : MonoBehaviour
{
    public float moveSpeed = 2f;
    public SpriteRenderer spriteRenderer;

    // 걷기 애니메이션
    public Sprite[] walkDown;
    public Sprite[] walkUp;
    public Sprite[] walkRight;

    // 대기 애니메이션
    public Sprite[] idleDown;
    public Sprite[] idleUp;
    public Sprite[] idleRight;

    // 칼 공격 애니메이션
    public Sprite[] swordDown;
    public Sprite[] swordUp;
    public Sprite[] swordRight;

    // 활 애니메이션
    public Sprite[] bowDown;
    public Sprite[] bowUp;
    public Sprite[] bowRight;

    // 화살 프리팹
    public GameObject arrowPrefab;

    public LayerMask enemyLayer;
    public float attackCooldown = 0.5f;
    public float shootCooldown = 0.5f;

    public float dashDistance = 2f;
    public float dashDuration = 0.15f;
    public float dodgeCooldown = 1f;

    public float shootMoveSpeed = 3f;
    public float swordMoveSpeed = 3f;

    // === 기본 스탯 ===
    public int baseMaxHealth = 10;
    public int baseSwordDamage = 3;
    public float baseSwordRange = 1f;
    public int baseBowDamage = 2;
    public int baseArrowCount = 1;
    public float baseDodgeInvincibleTime = 0.3f;

    // === 업그레이드된 최종 스탯 ===
    public int maxHealth;
    public int swordDamage;
    public float attackRange;
    public int bowDamage;
    public int arrowCount;
    public float invincibleTime;

    // === 업그레이드 인덱스 ===
    public int swordDamageIndex;
    public int swordRangeIndex;
    public int bowDamageIndex;
    public int arrowCountIndex;
    public int maxHealthIndex;
    public int dodgeTimeIndex;

    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 velocity;

    private float frameTimer = 0f;
    private int frameIndex = 0;
    private float frameRate = 0.2f;

    private Sprite[] currentAnim;
    private enum Direction { Down, Up, Right }
    private Direction currentDir = Direction.Down;

    private bool isAttacking = false;
    private bool isShooting = false;
    private bool isDodging = false;

    public int currentHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        currentAnim = walkDown;

        RecalculateStats();
        currentHealth = maxHealth;
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth); // UI 초기화
    }

    public void RecalculateStats()
    {
        var mgr = UpgradeManager.Instance;

        maxHealth = baseMaxHealth + (int)mgr.GetUpgradeValue(maxHealthIndex);
        swordDamage = baseSwordDamage + (int)mgr.GetUpgradeValue(swordDamageIndex);
        attackRange = baseSwordRange + mgr.GetUpgradeValue(swordRangeIndex);
        bowDamage = baseBowDamage + (int)mgr.GetUpgradeValue(bowDamageIndex);
        arrowCount = baseArrowCount + (int)mgr.GetUpgradeValue(arrowCountIndex);
        invincibleTime = baseDodgeInvincibleTime + mgr.GetUpgradeValue(dodgeTimeIndex);

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 마지막에 보정
    }


    void Update()
    {
        bool canAct = !isAttacking && !isShooting && !isDodging;

        if (!isDodging)
        {
            HandleInput();

            if (!isShooting && !isAttacking)
                HandleMovementAnim();
        }

        if (Input.GetKeyDown(KeyCode.S) && canAct)
            StartCoroutine(SwordAttack());

        if (Input.GetKeyDown(KeyCode.D) && canAct)
            StartCoroutine(ShootArrow());

        if (Input.GetKeyDown(KeyCode.A) && canAct)
            StartCoroutine(Dodge());

        if (Input.GetKeyDown(KeyCode.P))
        {
            GameManager.Instance.AddMoney(1000);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            UseHealthSkill();
        }

    }

    void UseHealthSkill()
    {
        int damage = 100;
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        // UI 갱신
        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);

        Debug.Log($"스킬 사용 - 체력 {damage} 감소. 현재 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Debug.Log("플레이어 사망");
            // 사망 처리 로직이 있다면 여기에 추가
        }
    }


    void FixedUpdate()
    {
        if (!isDodging)
        {
            float currentSpeed = isAttacking ? swordMoveSpeed : isShooting ? shootMoveSpeed : moveSpeed;
            rb.MovePosition(rb.position + input.normalized * currentSpeed * Time.fixedDeltaTime);
        }
    }

    void HandleInput()
    {
        input = Vector2.zero;

        if (Input.GetKey(KeyCode.LeftArrow)) input.x = -1;
        if (Input.GetKey(KeyCode.RightArrow)) input.x = 1;
        if (Input.GetKey(KeyCode.UpArrow)) input.y = 1;
        if (Input.GetKey(KeyCode.DownArrow)) input.y = -1;

        velocity = input.normalized * moveSpeed;
    }

    void HandleMovementAnim()
    {
        if (isAttacking || isShooting || isDodging) return;

        if (input.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(input.y) > Mathf.Abs(input.x))
            {
                currentDir = input.y > 0 ? Direction.Up : Direction.Down;
                spriteRenderer.flipX = false;
            }
            else
            {
                currentDir = Direction.Right;
                spriteRenderer.flipX = input.x < 0;
            }

            switch (currentDir)
            {
                case Direction.Down: currentAnim = walkDown; break;
                case Direction.Up: currentAnim = walkUp; break;
                case Direction.Right: currentAnim = walkRight; break;
            }

            Animate();
        }
        else
        {
            switch (currentDir)
            {
                case Direction.Down: currentAnim = idleDown; spriteRenderer.flipX = false; break;
                case Direction.Up: currentAnim = idleUp; spriteRenderer.flipX = false; break;
                case Direction.Right: currentAnim = idleRight; break;
            }

            Animate();
        }
    }

    void Animate()
    {
        frameTimer += Time.deltaTime;
        if (frameTimer >= frameRate)
        {
            frameTimer = 0f;
            frameIndex = (frameIndex + 1) % currentAnim.Length;
            spriteRenderer.sprite = currentAnim[frameIndex];
        }
    }

    System.Collections.IEnumerator SwordAttack()
    {
        isAttacking = true;
        velocity = Vector2.zero;

        Sprite[] attackAnim = swordDown;
        switch (currentDir)
        {
            case Direction.Down: attackAnim = swordDown; spriteRenderer.flipX = false; break;
            case Direction.Up: attackAnim = swordUp; spriteRenderer.flipX = false; break;
            case Direction.Right: attackAnim = swordRight; break;
        }

        if (currentDir == Direction.Right && input.x < 0)
            spriteRenderer.flipX = true;

        for (int i = 0; i < attackAnim.Length; i++)
        {
            spriteRenderer.sprite = attackAnim[i];
            yield return new WaitForSeconds(frameRate);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange, enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent(out Enemy enemy))
                enemy.TakeDamage(swordDamage);
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator ShootArrow()
    {
        isShooting = true;

        Sprite[] bowAnim = bowDown;
        Vector2 shootDir = Vector2.down;

        switch (currentDir)
        {
            case Direction.Down: bowAnim = bowDown; shootDir = Vector2.down; spriteRenderer.flipX = false; break;
            case Direction.Up: bowAnim = bowUp; shootDir = Vector2.up; spriteRenderer.flipX = false; break;
            case Direction.Right:
                bowAnim = bowRight;
                shootDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
                break;
        }

        // 애니메이션 재생 및 화살 발사 타이밍
        for (int i = 0; i < bowAnim.Length; i++)
        {
            spriteRenderer.sprite = bowAnim[i];

            // 🎯 세 번째 프레임(인덱스 2)에서 발사
            if (i == 2)
            {
                GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
                arrow.GetComponent<Arrow>().SetDirection(shootDir);
            }

            yield return new WaitForSeconds(frameRate);
        }

        yield return new WaitForSeconds(shootCooldown);
        isShooting = false;
    }


    System.Collections.IEnumerator Dodge()
    {
        isDodging = true;
        velocity = Vector2.zero;

        Vector2 dodgeDir = Vector2.zero;
        switch (currentDir)
        {
            case Direction.Down: dodgeDir = Vector2.up; break;
            case Direction.Up: dodgeDir = Vector2.down; break;
            case Direction.Right: dodgeDir = spriteRenderer.flipX ? Vector2.right : Vector2.left; break;
        }

        gameObject.layer = LayerMask.NameToLayer("Invincible");
        spriteRenderer.color = new Color32(0xCF, 0xCF, 0xCF, 0xFF);

        float elapsed = 0f;
        Vector2 startPos = rb.position;
        Vector2 targetPos = startPos + dodgeDir.normalized * dashDistance;

        while (elapsed < dashDuration)
        {
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, elapsed / dashDuration));
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPos);
        isDodging = false;

        yield return new WaitForSeconds(invincibleTime - dashDuration);
        gameObject.layer = LayerMask.NameToLayer("Player");
        spriteRenderer.color = Color.white;

        yield return new WaitForSeconds(dodgeCooldown);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private IEnumerator DelayedInit()
    {
        while (UpgradeManager.Instance == null)
            yield return null;

        RecalculateStats();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        GameManager.Instance.UpdateHealthUI(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            // 죽음 처리
            Debug.Log("플레이어 사망");
        }
    }

}
