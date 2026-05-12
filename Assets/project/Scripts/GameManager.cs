using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic; // Bắt buộc để dùng List/Stack

public class GameManager : MonoBehaviour
{
    [Header("--- UI Settings ---")]
    public TMP_Text timerText;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("--- Game Settings ---")]
    public float timeRemaining = 60f;
    public bool isGameActive = false;

    private bool isPaused = false;

    // --- LOGIC QUAY LẠI NƯỚC ĐI (UNDO) ---
    // Cấu trúc để lưu dữ liệu một nước đi (ví dụ: điểm hoặc vị trí)
    private Stack<float> timeHistory = new Stack<float>();
    // Nếu bạn có điểm hoặc danh sách vị trí đồ ăn, hãy tạo thêm Stack ở đây

    void Start()
    {
        Time.timeScale = 1f;
        isGameActive = true;
        isPaused = false;

        if (pausePanel != null) pausePanel.SetActive(false);
    }

    void Update()
    {
        if (isGameActive && !isPaused)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                UpdateTimerUI();
            }
            else
            {
                GameOver();
            }
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    // --- 1. LƯU NƯỚC ĐI ---
    // Bạn phải gọi hàm này ngay TRƯỚC khi người chơi thực hiện thao tác (như đổi chỗ đồ ăn)
    public void SaveStep()
    {
        timeHistory.Push(timeRemaining);
        // Lưu thêm vị trí các món ăn vào đây nếu cần
        Debug.Log("Đã lưu lại trạng thái hiện tại.");
    }

    // --- 2. HÀM QUAY LẠI NƯỚC ĐI (UNDO/REDO THEO Ý BẠN) ---
    // Gán hàm này vào nút bấm "Redo" trên UI của bạn
    public void RedoStep()
    {
        if (timeHistory.Count > 0)
        {
            timeRemaining = timeHistory.Pop(); // Lấy lại thời gian lúc trước khi đi
            UpdateTimerUI();

            // Ở đây bạn cần thêm code để đưa các món ăn về vị trí cũ
            Debug.Log("Đã quay lại nước đi trước!");
        }
        else
        {
            Debug.Log("Không còn nước đi nào để quay lại.");
        }
    }

    // --- 3. CÁC HÀM CƠ BẢN ---
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
    }

    public void RestartScene() // Chơi lại từ đầu cả màn
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("start UI");
    }

    public void GameOver()
    {
        isGameActive = false;
        timeRemaining = 0;
        Debug.Log("Game Over!");

        if (gameOverPanel != null) 
        {
            gameOverPanel.SetActive(true); // 숨겨뒀던 게임 종료 창 띄우기!
        }
        Time.timeScale = 0f;
    }
}