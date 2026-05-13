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
    // (gameClearPanel은 씬 이동을 하므로 지웠습니다. 필요하다면 다시 넣으셔도 됩니다.)

    [Header("--- Game Settings ---")]
    public float timeRemaining = 60f;
    public bool isGameActive = false;
    private float maxTime; // ★ 추가됨: 처음 설정한 전체 시간을 기억할 변수

    [Header("--- Clear Settings ---")]
    public int currentIngredients = 0; // 현재 모은 최종 재료 개수
    public int maxIngredients = 5;     // 목표 재료 개수 (쌀국수 재료 총 5개)

    [Header("--- Level Complete Scenes ---")] // ★ 추가됨: 인스펙터에서 적어줄 씬 이름들
    public string sceneName1Star = "1star UI";
    public string sceneName2Star = "2star UI";
    public string sceneName3Star = "3star UI";

    private bool isPaused = false;

    // --- LOGIC QUAY LẠI NƯỚC ĐI (UNDO) ---
    private Stack<float> timeHistory = new Stack<float>();

    void Start()
    {
        Time.timeScale = 1f;
        isGameActive = true;
        isPaused = false;
        currentIngredients = 0; 
        
        // ★ 게임이 시작될 때 설정된 시간을 최대 시간(maxTime)으로 저장해둡니다.
        // (인스펙터에서 45초로 바꾸든 60초로 바꾸든 알아서 똑똑하게 계산됩니다!)
        maxTime = timeRemaining; 

        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
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

    // --- 4. 게임 클리어 로직 ---
    
    public void AddFinalIngredient()
    {
        if (!isGameActive) return; 

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
        isGameActive = false;
        Time.timeScale = 1f; 

        Debug.Log("🎉 쌀국수 재료 5개 완성! 2초 뒤 결과 씬으로 이동합니다.");

        // ★ 2초 뒤에 씬을 계산해서 이동시키는 함수를 부릅니다.
        Invoke("LoadStarScene", 2f); 
    }

    // ★ 5. 걸린 시간을 계산해서 별 씬으로 넘어가는 함수
    private void LoadStarScene()
    {
        // 걸린 시간 계산 = (처음에 주어진 시간 - 현재 남은 시간)
        float timeTaken = maxTime - timeRemaining; 

        if (timeTaken <= 10f)
        {
            Debug.Log(timeTaken + "초 걸림! 별 3개 획득!");
            SceneManager.LoadScene(sceneName3Star);
        }
        else if (timeTaken <= 15f)
        {
            Debug.Log(timeTaken + "초 걸림! 별 2개 획득!");
            SceneManager.LoadScene(sceneName2Star);
        }
        else
        {
            Debug.Log(timeTaken + "초 걸림! 별 1개 획득!");
            SceneManager.LoadScene(sceneName1Star);
        }
    }
}