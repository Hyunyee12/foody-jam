using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 꼭 필요함!

public class ResultUIManager : MonoBehaviour
{
    // Again 버튼을 눌렀을 때 실행될 함수 (SampleScene으로 이동)
    // Again 버튼을 눌렀을 때 실행될 함수
    public void ClickAgainButton() // (함수 이름은 다를 수 있습니다)
    {
        Time.timeScale = 1f;

        // ★ 수첩에서 방금 플레이했던 스테이지 이름을 읽어옵니다. (만약 없다면 기본값으로 "SampleScene"을 엽니다)
        string lastStage = PlayerPrefs.GetString("LastPlayedStage", "SampleScene");
        
        // 읽어온 스테이지로 다시 이동합니다!
        UnityEngine.SceneManagement.SceneManager.LoadScene(lastStage);
    }

    // Next Stage 버튼을 눌렀을 때 실행될 함수 (start UI로 이동)
    // Next Stage 버튼을 눌렀을 때 실행될 함수
    // Next Stage 버튼을 눌렀을 때 실행될 함수
    public void ClickNextStageButton()
    {
        Time.timeScale = 1f;

        // ★ 유니티의 메모장에 "UnlockedStage는 이제 2다!" 라고 영구 저장합니다.
        PlayerPrefs.SetInt("UnlockedStage", 2);
        PlayerPrefs.Save(); 

        SceneManager.LoadScene("stage_level 2");
    }
}