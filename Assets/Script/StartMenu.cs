using UnityEngine;
using UnityEngine.SceneManagement; 

public class StartMenu : MonoBehaviour
{
    // Play 버튼을 누르면 실행될 함수
    public void ClickPlay()
    {
        Time.timeScale = 1f; 

        // 유니티 수첩을 열어서 "UnlockedStage"가 몇인지 확인합니다. (기본값 1)
        int unlocked = PlayerPrefs.GetInt("UnlockedStage", 1);

        // 만약 수첩에 적힌 숫자가 2 이상이라면 (2단계가 열려있다면)
        if (unlocked >= 2)
        {
            // 2단계가 열려있는 예쁜 씬으로 보냅니다!
            SceneManager.LoadScene("stage_level 2"); 
        }
        // 수첩에 적힌 숫자가 1이라면 (아직 1단계만 열려있다면)
        else
        {
            // 1단계만 열려있는 씬으로 보냅니다!
            SceneManager.LoadScene("stage_level"); 
        }
    }
}