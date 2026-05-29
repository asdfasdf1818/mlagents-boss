using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManualController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Shooting")]
    public GameObject projectilePrefab; // 투사체 프리팹 프리셋
    public BossAgent bossAgent;          // 보스에게 골인 신호를 주기 위한 참조

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Camera mainCamera;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main; // 마우스 좌표 변환용 메인 카메라 찾기
    }

    private void Update()
    {
        // 1. 이동 입력 (기존 유지)
        moveInput = Vector2.zero;
        if (Keyboard.current.wKey.isPressed) moveInput.y += 1f;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1f;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1f;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1f;
        moveInput = moveInput.normalized;

        // 2. 마우스 좌클릭 입력 감지하여 투사체 발사!
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            ShootProjectile();
        }
    }

    private void FixedUpdate()
    {
        if (rb != null)
        {
            rb.linearVelocity = moveInput * moveSpeed;
        }

        // 벽 밖 탈출 방지 오버레이
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Clamp(pos.x, -5.4f, 5.4f);
        pos.y = Mathf.Clamp(pos.y, -3.4f, 3.4f);
        transform.localPosition = pos;
    }

    private void ShootProjectile()
    {
        if (projectilePrefab == null || bossAgent == null)
        {
            Debug.LogWarning("PlayerManualController에 프리팹이나 BossAgent가 연결되지 않았습니다.");
            return;
        }

        // 마우스 스크린 좌표를 게임 속 2D 월드 좌표로 변환
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Vector3 worldMousePos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 10f));

        // 플레이어 위치에서 마우스 위치를 향하는 방향 벡터 계산
        Vector2 shootDirection = (Vector2)(worldMousePos - transform.position);

        // 투사체 생성 및 세팅 연동
        GameObject projObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile proj = projObj.GetComponent<Projectile>();
        if (proj != null)
        {
            proj.Setup(shootDirection, bossAgent);
        }
    }
}