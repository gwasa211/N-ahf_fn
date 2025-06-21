using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float speed = 10f;          // 화살 속도
    private Vector2 direction;         // 날아가는 방향 (월드 좌표 기준)

    public int damage = 2;             // 화살 데미지
    public int pierce = 1;             // 남은 관통 횟수

    void Update()
    {
        // ✅ 월드 좌표계 기준으로 이동 (로컬 기준이 아님)
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    // 화살의 방향을 외부에서 지정 (예: 플레이어가 방향 결정)
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized; // 방향을 정규화해서 속도에 일관성 유지

        // 화살의 시각적 방향도 맞춰줌
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); // Z축 기준 회전
    }

    // 충돌 감지
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) // 적과 부딪혔을 때
        {
            if (other.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(damage); // 데미지 전달
                pierce--;                 // 관통 수 1 감소

                if (pierce <= 0)
                {
                    Destroy(gameObject); // 관통 다 했으면 화살 제거
                }
            }
        }
        else if (!other.isTrigger && !other.CompareTag("Player"))
        {
            // 벽이나 일반 장애물과 부딪히면 제거
            Destroy(gameObject);
        }
    }
}
