using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic; // Bắt buộc để dùng List/Stack
using System.Collections; // ★ 코루틴(애니메이션)을 위해 꼭 추가해야 합니다!

public class GameManager : MonoBehaviour
{
    [Header("--- UI Settings ---")]
    public TMP_Text timerText;
    public GameObject pausePanel;
    public GameObject gameOverPanel;

    [Header("--- Game Settings ---")]
    public float timeRemaining = 60f;
    public bool isGameActive = false;
    private float maxTime; 

    [Header("--- Clear Settings ---")]
    public int currentIngredients = 0; 
    public int maxIngredients = 5;     

    // ★ 추가됨: 빨려 들어갈 쌀국수 그릇의 위치!
    [Header("--- Ending Animation ---")]
    public Transform bowlTransform; 

    [Header("--- Level Complete Scenes ---")] 
    public string sceneName1Star = "1star UI";
    public string sceneName2Star = "2star UI";
    public string sceneName3Star = "3star UI";

    private bool isPaused = false;
    private Stack<float> timeHistory = new Stack<float>();

    void Start()
    {
        Time.timeScale = 1f;
        isGameActive = true;
        isPaused = false;
        currentIngredients = 0; 
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

    public void SaveStep()
    {
        timeHistory.Push(timeRemaining);
        Debug.Log("Đã lưu lại trạng thái hiện tại.");
    }

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

        Debug.Log("🎉 쌀국수 재료 5개 완성! 그릇으로 날아갑니다!");

        // ★ 기존 Invoke를 지우고, 멋진 애니메이션 코루틴을 실행합니다!
        StartCoroutine(SuckIntoBowlAnimation());
    }

    // ★ 새롭게 추가된 마법의 애니메이션 함수
    // ★ 새롭게 수정된 마법의 애니메이션 함수 (원본은 놔두고 복제본만 날려보내기!)
    private IEnumerator SuckIntoBowlAnimation()
    {
        // 1. 화면에 있는 'FinalBlock' 태그를 가진 모든 블록을 찾습니다. (양옆 원본들도 포함됨)
        GameObject[] allFinalBlocks = GameObject.FindGameObjectsWithTag("FinalBlock");
        
        // ★ 진짜 퍼즐판 위에 있는 블록들만 담을 새로운 바구니(리스트)를 만듭니다.
        List<GameObject> boardBlocks = new List<GameObject>();

        // 2. 찾은 블록들의 이름을 하나씩 검사합니다.
        for (int i = 0; i < allFinalBlocks.Length; i++)
        {
            // 이름에 "(Clone)"이 들어있다면? -> 퍼즐판에서 합쳐져서 새로 태어난 진짜 블록!
            if (allFinalBlocks[i].name.Contains("(Clone)"))
            {
                boardBlocks.Add(allFinalBlocks[i]); // 바구니에 담습니다.
            }
        }

        // 위치와 크기를 기억해둘 배열 (진짜 블록 개수만큼만 만듦)
        Vector3[] startPositions = new Vector3[boardBlocks.Count];
        Vector3[] startScales = new Vector3[boardBlocks.Count];

        for (int i = 0; i < boardBlocks.Count; i++)
        {
            startPositions[i] = boardBlocks[i].transform.position;
            startScales[i] = boardBlocks[i].transform.localScale;
            
            // 날아갈 때 다른 블록이랑 안 부딪히게 충돌체를 꺼줍니다.
            if(boardBlocks[i].GetComponent<Collider2D>() != null)
                boardBlocks[i].GetComponent<Collider2D>().enabled = false;
        }

        float timer = 0f;
        float animationDuration = 1.5f; // 날아가는 시간 (1.5초)

        // 3. 1.5초 동안 '진짜 블록'들만 서서히 그릇으로 이동 + 축소 + 회전시킵니다.
        while (timer < animationDuration)
        {
            timer += Time.deltaTime;
            float percent = timer / animationDuration;
            float curve = Mathf.SmoothStep(0f, 1f, percent); // 부드러운 감속 효과

            for (int i = 0; i < boardBlocks.Count; i++)
            {
                if (boardBlocks[i] != null && bowlTransform != null)
                {
                    boardBlocks[i].transform.position = Vector3.Lerp(startPositions[i], bowlTransform.position, curve);
                    boardBlocks[i].transform.localScale = Vector3.Lerp(startScales[i], Vector3.zero, curve);
                    boardBlocks[i].transform.Rotate(0, 0, 1000f * Time.deltaTime);
                }
            }
            yield return null; // 다음 프레임까지 대기
        }

        // 4. 애니메이션이 끝나면 별 화면으로 넘어갑니다.
        LoadStarScene();
    }

    // ★ 걸린 시간을 계산해서 별 씬으로 넘어가는 함수
    private void LoadStarScene()
    {
        float timeTaken = maxTime - timeRemaining; 

        // ★ 결과 창으로 넘어가기 직전에 현재 씬의 이름을 "LastPlayedStage"라는 메모장에 적어둡니다!
        string currentSceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastPlayedStage", currentSceneName);
        PlayerPrefs.Save();

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