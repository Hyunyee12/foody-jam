using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위해 꼭 필요함!

public class ResultUIManager : MonoBehaviour
{
    // Again 버튼을 눌렀을 때 실행될 함수 (SampleScene으로 이동)
    public void ClickAgainButton()
    {
        // 멈춰있을지도 모르는 시간을 다시 흐르게 돌려놓기
        Time.timeScale = 1f; 
        SceneManager.LoadScene("SampleScene");
    }

    // Next Stage 버튼을 눌렀을 때 실행될 함수 (start UI로 이동)
    public void ClickNextStageButton()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("start UI");
    }
}