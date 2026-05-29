using UnityEngine;
using UnityEngine.InputSystem;

public class BossManualController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Area Manager")]
    public TrainingAreaManager areaManager;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            Debug.LogError("BossAgent에 Rigidbody2D가 없습니다.");
        }
    }

    private void Start()
    {
        if (areaManager == null)
        {
            areaManager = GetComponentInParent<TrainingAreaManager>();
        }

        if (areaManager != null)
        {
            areaManager.ResetArea();
        }
        else
        {
            Debug.LogError("TrainingAreaManager를 찾지 못했습니다.");
        }
    }

    private void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
        {
            moveInput.y += 1f;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            moveInput.y -= 1f;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            moveInput.x -= 1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            moveInput.x += 1f;
        }

        moveInput = moveInput.normalized;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hit Player! Reset Area.");

            if (areaManager != null)
            {
                areaManager.ResetArea();
            }
        }

        if (other.gameObject.name.Contains("Wall"))
        {
            Debug.Log("Hit Wall!");
        }
    }
}