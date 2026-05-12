using UnityEngine;

public class IngredientReporter : MonoBehaviour
{
    void Start()
    {
        // 1. 내가 태어났다고 콘솔창에 강력하게 외치기!
        Debug.Log("🔔 [" + gameObject.name + "] 완성됨! 매니저 찾는 중...");

        GameManager gm = FindObjectOfType<GameManager>();
        
        if (gm != null)
        {
            gm.AddFinalIngredient(); // 찾았으면 보고하기
        }
        else
        {
            // 매니저가 없으면 빨간색 에러로 알려주기
            Debug.LogError("🚨 GameManager를 씬에서 찾을 수가 없어요!!");
        }
    }
}