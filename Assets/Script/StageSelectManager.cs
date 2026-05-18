using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelectManager : MonoBehaviour
{
    public void ClickBackButton()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("start UI");
    }

    public void ClickLevel1Button()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }

    // ★ 이 부분을 새로 추가해 주세요! ★
    public void ClickLevel2Button()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SecondStage"); // 2단계 씬 이름
    }
}