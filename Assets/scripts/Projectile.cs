using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f; // 투사체 속도 (보스보다 빨라야 피하기 쫄깃합니다)
    private Vector2 moveDirection;
    private Rigidbody2D rb;
    private BossAgent bossAgent;

    public void Setup(Vector2 direction, BossAgent agent)
    {
        moveDirection = direction.normalized;
        bossAgent = agent;
        rb = GetComponent<Rigidbody2D>();

        // 투사체가 날아가는 방향으로 오브젝트 회전 (2D 화살/총알 효과)
        float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. 벽에 부딪혔다는 것은 보스가 이 투사체를 무사히 피했다는 뜻!
        if (other.gameObject.name.Contains("Wall"))
        {
            if (bossAgent != null)
            {
                bossAgent.AddReward(0.2f); // 회피 성공 보상 (+0.2)
                Debug.Log("Boss successfully dodged! Reward +0.2");
            }
            Destroy(gameObject);
        }
        // 2. 보스에게 명중했다면 (보스 에이전트의 태그를 "Agent" 혹은 이름으로 체크)
        else if (other.gameObject.name.Contains("BossAgent"))
        {
            // 패널티와 에피소드 종료는 BossAgent 내부에서 처리하므로 여기서는 파괴만 합니다.
            Destroy(gameObject);
        }
    }
}