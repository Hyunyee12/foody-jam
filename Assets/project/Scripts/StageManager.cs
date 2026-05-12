using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Gọi thư viện TextMeshPro để làm UI chữ

public class StageController : MonoBehaviour
{
    [Header("--- UI Elements ---")]
    public TMP_Text timerText;    // Kéo Text hiển thị thời gian vào đây
    public GameObject pausePanel; // Kéo Panel Pause (menu mờ mờ hiện ra khi dừng) vào đây

    private float currentTime = 0f;
    private bool isPaused = false;

    void Start()
    {
        // Đảm bảo game chạy bình thường khi mới vào màn 
        // (Phòng trường hợp bạn quit game lúc đang pause làm timeScale bị kẹt ở 0)
        Time.timeScale = 1f;

        // Ẩn bảng Pause đi lúc mới chơi
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        // Đếm thời gian liên tục nếu game không bị pause
        if (!isPaused)
        {
            currentTime += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    // --- 1. LOGIC TIMER ---
    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            // Ép kiểu thời gian ra Phút và Giây
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);

            // Format chữ hiển thị theo kiểu 00:00 (ví dụ 01:05)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // --- 2. LOGIC NÚT PAUSE / RESUME ---
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            // Đóng băng thời gian (nhân vật, quái, animation sẽ dừng lại)
            Time.timeScale = 0f;
            // Bật menu Pause lên
            if (pausePanel != null) pausePanel.SetActive(true);
        }
        else
        {
            // Trả lại thời gian bình thường
            Time.timeScale = 1f;
            // Tắt menu Pause đi
            if (pausePanel != null) pausePanel.SetActive(false);
        }
    }

    // --- 3. LOGIC NÚT REDO (CHƠI LẠI TỪ ĐẦU) ---
    public void RestartStage()
    {
        // Bắt buộc phải xả đông thời gian trước khi load lại scene
        Time.timeScale = 1f;

        // Lấy đúng tên của Scene hiện tại (đang chơi dở) và load lại nó
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }
}