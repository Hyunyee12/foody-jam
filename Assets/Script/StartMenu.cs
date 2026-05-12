using UnityEngine;
using UnityEngine.SceneManagement; // 씬 이동을 위한 필수 코드!

public class StartMenu : MonoBehaviour
{
    // Play 버튼을 누르면 실행될 함수
    public void ClickPlay()
    {
        // 괄호 안에 친구가 작업 중인 메인 씬 이름을 정확히 적어주면 돼
        SceneManager.LoadScene("SampleScene"); 
    }
}