using UnityEngine;

public class TargetMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    [Header("Auto Shooting (AI Training)")]
    public GameObject projectilePrefab; // 총알 프리팹
    public BossAgent bossAgent;          // 타겟이 될 보스 에이전트
    public float shootInterval = 1.5f;   // 1.5초마다 자동으로 발사 (밸런스 조절 가능)
    private float shootTimer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        SetRandomDirection();
    }

    private void Update()
    {
        // 컴퓨터가 스스로 총알을 쏘는 타이머 시스템
        if (projectilePrefab != null && bossAgent != null)
        {
            shootTimer += Time.deltaTime;
            if (shootTimer >= shootInterval)
            {
                ShootAutoProjectile();
                shootTimer = 0f; // 타이머 초기화
            }
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }

        // 벽 밖 탈출 방지 오버레이
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Clamp(pos.x, -5.4f, 5.4f);
        pos.y = Mathf.Clamp(pos.y, -3.4f, 3.4f);
        transform.localPosition = pos;
    }

    public void SetRandomDirection()
    {
        // 리셋될 때마다 새로운 랜덤 방향 벡터 설정
        moveDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void ShootAutoProjectile()
    {
        // 현재 보스가 있는 위치를 실시간으로 조준하여 방향 계산
        Vector2 shootDirection = (Vector2)(bossAgent.transform.position - transform.position);

        // 총알 생성 및 세팅 연동
        GameObject projObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile proj = projObj.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Setup(shootDirection, bossAgent);
        }
    }
}