using UnityEngine;

public class Player : MonoBehaviour
{
    public float moveSpeed = 2f;
    public SpriteRenderer spriteRenderer;

    // 걷기 애니메이션
    public Sprite[] walkDown;
    public Sprite[] walkUp;
    public Sprite[] walkRight;

    // 대기 애니메이션
    // 대기(Idle) 애니메이션
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
    public float attackRange = 1f;
    public int swordDamage = 1;
    public float attackCooldown = 0.5f;
    public float shootCooldown = 0.5f;

    public float dashDistance = 2f;
    public float dashDuration = 0.15f;
    public float invincibleTime = 0.3f;
    public float dodgeCooldown = 1f;

    public float shootMoveSpeed = 3f; // 활 쏘는 중 이동 속도

    public float swordMoveSpeed = 3f; // 칼 휘두를 때 이동 속도



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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        currentAnim = walkDown;
    }

    void Update()
    {
        bool canAct = !isAttacking && !isShooting && !isDodging;

        if (!isDodging)
        {
            HandleInput();

            if (!isShooting && !isAttacking)
                HandleMovementAnim(); // 걷기 애니메이션은 공격/활 중 제외
        }

        if (Input.GetKeyDown(KeyCode.S) && canAct)
            StartCoroutine(SwordAttack());

        if (Input.GetKeyDown(KeyCode.D) && canAct)
            StartCoroutine(ShootArrow());

        if (Input.GetKeyDown(KeyCode.A) && canAct)
            StartCoroutine(Dodge());
    }


    void FixedUpdate()
    {
        if (!isDodging)
        {
            float currentSpeed;

            if (isAttacking)
                currentSpeed = swordMoveSpeed;
            else if (isShooting)
                currentSpeed = shootMoveSpeed;
            else
                currentSpeed = moveSpeed;

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
        if (isAttacking || isShooting || isDodging)
            return;

        if (input.sqrMagnitude > 0.01f)
        {
            // === 이동 방향이 가장 클 때만 방향 갱신 ===
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

            // 이동 애니메이션 선택
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
            // 대기 중엔 방향을 바꾸지 않음
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

    System.Collections.IEnumerator ShootArrow()
    {
        isShooting = true;

        Sprite[] bowAnim = bowDown;
        Vector2 shootDir = Vector2.down;

        switch (currentDir)
        {
            case Direction.Down:
                bowAnim = bowDown;
                shootDir = Vector2.down;
                spriteRenderer.flipX = false;
                break;

            case Direction.Up:
                bowAnim = bowUp;
                shootDir = Vector2.up;
                spriteRenderer.flipX = false;
                break;

            case Direction.Right:
                bowAnim = bowRight;
                shootDir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
                break;
        }

        for (int i = 0; i < bowAnim.Length; i++)
        {
            spriteRenderer.sprite = bowAnim[i];
            yield return new WaitForSeconds(frameRate);
        }

        GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
        arrow.GetComponent<Arrow>().SetDirection(shootDir);

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
            case Direction.Right:
                dodgeDir = spriteRenderer.flipX ? Vector2.right : Vector2.left;
                break;
        }

        gameObject.layer = LayerMask.NameToLayer("Invincible");

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

        yield return new WaitForSeconds(invincibleTime - dashDuration);
        gameObject.layer = LayerMask.NameToLayer("Player");

        yield return new WaitForSeconds(dodgeCooldown);
        isDodging = false;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
