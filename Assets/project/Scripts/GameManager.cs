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
    public GameObject gameClearPanel; // ★ 추가됨: 게임 클리어 시 띄울 성공 패널!

    [Header("--- Game Settings ---")]
    public float timeRemaining = 60f;
    public bool isGameActive = false;

    [Header("--- Clear Settings ---")] // ★ 추가됨: 클리어 조건 설정
    public int currentIngredients = 0; // 현재 모은 최종 재료 개수
    public int maxIngredients = 5;     // 목표 재료 개수 (쌀국수 재료 총 5개)

    private bool isPaused = false;

    // --- LOGIC QUAY LẠI NƯỚC ĐI (UNDO) ---
    private Stack<float> timeHistory = new Stack<float>();

    void Start()
    {
        Time.timeScale = 1f;
        isGameActive = true;
        isPaused = false;
        currentIngredients = 0; // ★ 게임 시작 시 재료 개수 0으로 초기화

        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameClearPanel != null) gameClearPanel.SetActive(false); // ★ 성공 패널도 시작 시 숨김
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
    public void SaveStep()
    {
        timeHistory.Push(timeRemaining);
        Debug.Log("Đã lưu lại trạng thái hiện tại.");
    }

    // --- 2. HÀM QUAY LẠI NƯỚC ĐI (UNDO/REDO THEO Ý BẠN) ---
    public void RedoStep()
    {
        if (timeHistory.Count > 0)
        {
            timeRemaining = timeHistory.Pop(); 
            UpdateTimerUI();
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

    public void RestartScene() 
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
            gameOverPanel.SetActive(true); 
        }
        Time.timeScale = 0f;
    }

    // --- 4. 게임 클리어 로직 (★ 새로 추가된 부분) ---
    
    // 최종 재료가 완성될 때마다 (IngredientReporter가) 이 함수를 부릅니다.
    public void AddFinalIngredient()
    {
        if (!isGameActive) return; // 이미 게임이 끝났으면 무시

        currentIngredients++;
        Debug.Log("최종 재료 획득! 현재: " + currentIngredients + " / " + maxIngredients);

        // 5개를 다 모았다면 클리어 처리!
        if (currentIngredients >= maxIngredients)
        {
            GameClear();
        }
    }

    private void GameClear()
    {
        // 1. 게임 상태를 끝남으로 변경 (유저가 더 이상 블록을 못 움직이게)
        isGameActive = false;
        
        // 2. 시간은 정상 속도(1)로 유지 (Invoke가 시간의 영향을 받기 때문)
        Time.timeScale = 1f; 

        // 3. 콘솔창에 성공 메시지 띄우기
        Debug.Log("🎉 쌀국수 재료 5개 완성! 2초 뒤 엔딩 씬으로 이동합니다.");

        // ★ 4. "LoadEndingScene" 이라는 함수를 '2초(2f)' 뒤에 실행해라!
        Invoke("LoadEndingScene", 2f); 
    }

    // ★ 5. 2초 뒤에 실제로 불려질 씬 이동 함수 만들기
    private void LoadEndingScene()
    {
        SceneManager.LoadScene("ending UI"); 
    }
}