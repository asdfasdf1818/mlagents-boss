using UnityEngine;

public class TrainingAreaManager : MonoBehaviour
{
    public enum AreaMode
    {
        TrainingRandom,
        CaptureFixed
    }

    [Header("Mode")]
    public AreaMode mode = AreaMode.TrainingRandom;

    [Header("Objects")]
    public Transform bossAgent;
    public Transform playerTarget;

    [Header("Random Spawn Range")]
    public Vector2 bossMin = new Vector2(-3f, -3f);
    public Vector2 bossMax = new Vector2(3f, 3f);
    public Vector2 playerMin = new Vector2(-3f, -3f);
    public Vector2 playerMax = new Vector2(3f, 3f);

    [Header("Fixed Capture Positions")]
    public Vector2 fixedBossPosition = new Vector2(-3f, 0f);
    public Vector2 fixedPlayerPosition = new Vector2(3f, 0f);

    private Rigidbody2D bossRb;
    private Rigidbody2D playerRb;

    private void Awake()
    {
        if (bossAgent != null)
        {
            bossRb = bossAgent.GetComponent<Rigidbody2D>();
        }

        if (playerTarget != null)
        {
            playerRb = playerTarget.GetComponent<Rigidbody2D>();
        }
    }

    public void ResetArea()
    {
        if (bossAgent == null || playerTarget == null)
        {
            Debug.LogError("TrainingAreaManager에 BossAgent 또는 PlayerTarget이 연결되지 않았습니다.");
            return;
        }

        if (mode == AreaMode.TrainingRandom)
        {
            ResetRandom();
        }
        else if (mode == AreaMode.CaptureFixed)
        {
            ResetFixed();
        }

        StopPhysics();
    }

    private void ResetRandom()
    {
        Vector2 bossPos = new Vector2(
            Random.Range(bossMin.x, bossMax.x),
            Random.Range(bossMin.y, bossMax.y)
        );

        Vector2 playerPos = new Vector2(
            Random.Range(playerMin.x, playerMax.x),
            Random.Range(playerMin.y, playerMax.y)
        );

        bossAgent.localPosition = bossPos;
        playerTarget.localPosition = playerPos;
    }

    private void ResetFixed()
    {
        bossAgent.localPosition = fixedBossPosition;
        playerTarget.localPosition = fixedPlayerPosition;
    }

    private void StopPhysics()
    {
        if (bossRb != null)
        {
            bossRb.linearVelocity = Vector2.zero;
            bossRb.angularVelocity = 0f;
        }

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        TargetMover mover = playerTarget.GetComponent<TargetMover>();
        if (mover != null)
        {
            mover.SetRandomDirection(); // 리셋될 때마다 새로운 방향으로 출발하게 만듭니다.
        }
    }
}