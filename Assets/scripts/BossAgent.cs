using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.InputSystem;

public class BossAgent : Agent
{
    [Header("References")]
    public Transform playerTarget;
    public TrainingAreaManager areaManager;

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Reward")]
    public float stepPenalty = -0.001f;
    public float hitReward = 1.0f;
    public float wallPenalty = -0.2f;

    [Header("Boundary")]
    public float xLimit = 5.8f;
    public float yLimit = 3.8f;
    public float outOfBoundsPenalty = -1.0f;

    private Rigidbody2D rb;

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();

        if (areaManager == null)
        {
            areaManager = GetComponentInParent<TrainingAreaManager>();
        }

        if (rb == null)
        {
            Debug.LogError("BossAgent에 Rigidbody2D가 없습니다.");
        }

        if (playerTarget == null)
        {
            Debug.LogError("PlayerTarget이 연결되지 않았습니다.");
        }
    }

    public override void OnEpisodeBegin()
    {
        if (areaManager != null)
        {
            areaManager.ResetArea();
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector2 bossPos = transform.localPosition;
        Vector2 playerPos = playerTarget.localPosition;

        Vector2 directionToPlayer = (playerPos - bossPos).normalized;
        float distanceToPlayer = Vector2.Distance(bossPos, playerPos);

        // 관찰값 총 7개
        sensor.AddObservation(bossPos.x);
        sensor.AddObservation(bossPos.y);
        sensor.AddObservation(playerPos.x);
        sensor.AddObservation(playerPos.y);
        sensor.AddObservation(directionToPlayer.x);
        sensor.AddObservation(directionToPlayer.y);
        sensor.AddObservation(distanceToPlayer);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        AddReward(stepPenalty);

        float moveX = actions.ContinuousActions[0];
        float moveY = actions.ContinuousActions[1];

        Vector2 moveDirection = new Vector2(moveX, moveY);

        if (moveDirection.magnitude > 1f)
        {
            moveDirection = moveDirection.normalized;
        }

        rb.linearVelocity = moveDirection * moveSpeed;

        // 벽 밖으로 나가면 큰 패널티 후 에피소드 종료
        Vector2 pos = transform.localPosition;

        if (Mathf.Abs(pos.x) > xLimit || Mathf.Abs(pos.y) > yLimit)
        {
            AddReward(outOfBoundsPenalty);
            Debug.Log("Out of bounds! Penalty " + outOfBoundsPenalty);
            EndEpisode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger with: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            AddReward(hitReward);
            Debug.Log("ML-Agent Hit Player! Reward +" + hitReward);
            EndEpisode();
        }
        else if (other.gameObject.name.Contains("Wall"))
        {
            AddReward(wallPenalty);
            Debug.Log("ML-Agent Hit Wall! Penalty " + wallPenalty);
        }
        else if (other.CompareTag("Projectile")) // 투사체 태그 체크
        {
            AddReward(-0.5f); // 투사체 맞으면 큰 패널티 (-0.5)
            Debug.Log("ML-Agent Hit by Projectile! Penalty -0.5");
            EndEpisode(); // 맞으면 즉시 이번 판 종료 (강한 학습 효과)
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        float moveX = 0f;
        float moveY = 0f;

        if (Keyboard.current.wKey.isPressed)
        {
            moveY += 1f;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            moveY -= 1f;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            moveX -= 1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            moveX += 1f;
        }

        Vector2 input = new Vector2(moveX, moveY).normalized;

        continuousActions[0] = input.x;
        continuousActions[1] = input.y;
    }
}