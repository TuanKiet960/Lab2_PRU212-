using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;        // để dùng Button
using TMPro;                // để dùng TextMeshProUGUI

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int totalCollectibles;
    private int collectedCount;

    [Header("UI References")]
    public TextMeshProUGUI countTextUI;   // kéo thả CountText (TextMeshPro) vào đây
    public TextMeshProUGUI winTextUI;     // kéo thả WinText (TextMeshPro) vào đây
    public Button restartButton;          // kéo thả Restart (Button) vào đây

    void Awake()
    {
        // Thiết lập Singleton
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // 1️⃣ Ẩn UI Win & Restart lúc đầu
        if (winTextUI != null) winTextUI.gameObject.SetActive(false);
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(false);
            // Gán sự kiện bấm nút:
            restartButton.onClick.AddListener(RestartLevel);
        }

        // 2️⃣ Đếm tổng sao trong scene
        totalCollectibles = GameObject.FindGameObjectsWithTag("Collectible").Length;
        collectedCount = 0;

        // 3️⃣ Cập nhật số lần đầu
        UpdateCountText();
    }

    /// <summary>
    /// Gọi từ Collectible khi robot thu 1 sao
    /// </summary>
    public void CollectOne()
    {
        collectedCount++;
        UpdateCountText();

        if (collectedCount >= totalCollectibles)
            OnWin();
    }

    /// <summary>
    /// Cập nhật text "x / y"
    /// </summary>
    private void UpdateCountText()
    {
        if (countTextUI != null)
            countTextUI.text = $"{collectedCount} / {totalCollectibles}";
    }

    /// <summary>
    /// Xử lý khi thu hết sao
    /// </summary>
    private void OnWin()
    {
        // Hiện You Win
        if (winTextUI != null)
            winTextUI.gameObject.SetActive(true);

        // Hiện nút Restart
        if (restartButton != null)
            restartButton.gameObject.SetActive(true);

        // Khóa di chuyển robot (nếu có)
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var pc = player.GetComponent<PlayerController>();
            if (pc != null) pc.LockMovement();
        }
    }

    /// <summary>
    /// Reload lại scene
    /// </summary>
    public void RestartLevel()
    {
        // Load lại chính scene đang mở
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}