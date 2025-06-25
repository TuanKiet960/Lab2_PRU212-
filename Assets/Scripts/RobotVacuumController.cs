using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class RobotVacuumController : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    [Tooltip("Tốc độ di chuyển (units/giây) của robot")]
    public float moveSpeed = 2f;

    // Độ lệch max (độ) để xoay vector phản xạ
    [Tooltip("Giá trị max độ lệch (random) khi bounce (ví dụ 30 độ)")]
    public float maxBounceAngle = 20f;

    private Vector2 moveDirection;
    private Rigidbody2D rb;

    // Để theo dõi thời gian va chạm gần nhất (nếu muốn loại bỏ tình trạng bounce liên tục)
    private float lastCollisionTime = -1f;
    private float collisionCooldown = 0.1f; // 0.1 giây

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Chọn hướng ngẫu nhiên ban đầu
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;
    }

    void FixedUpdate()
    {
        Vector2 newPosition = rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Lấy thời điểm hiện tại
        float now = Time.time;

        // Nếu quá gần lần va chạm trước (robot có thể đang kẹt), chọn hướng hoàn toàn mới
        if (now - lastCollisionTime < collisionCooldown)
        {
            // Chọn một góc random (0°–360°) và đặt làm moveDirection
            float randAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            moveDirection = new Vector2(Mathf.Cos(randAngle), Mathf.Sin(randAngle)).normalized;
        }
        else
        {
            // Lấy normal bề mặt va chạm
            Vector2 normal = collision.contacts[0].normal;

            // Phản xạ
            Vector2 reflectDir = Vector2.Reflect(moveDirection, normal).normalized;

            // Tính một độ lệch random trong [-maxBounceAngle, +maxBounceAngle] (đơn vị độ)
            float randomAngleOffset = Random.Range(-maxBounceAngle, maxBounceAngle);

            // Chuyển sang radian để xoay
            float offsetRad = randomAngleOffset * Mathf.Deg2Rad;

            // Xoay vector phản xạ đi offsetRad
            float cos = Mathf.Cos(offsetRad);
            float sin = Mathf.Sin(offsetRad);
            Vector2 newDir = new Vector2(
                reflectDir.x * cos - reflectDir.y * sin,
                reflectDir.x * sin + reflectDir.y * cos
            ).normalized;

            moveDirection = newDir;
        }

        // Ghi lại thời điểm va chạm này
        lastCollisionTime = now;
    }
}