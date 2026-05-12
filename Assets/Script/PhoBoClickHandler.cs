using UnityEngine;

public class PhoBoClickHandler : MonoBehaviour
{
    // 여기에 'RecipeGroup' (부모 오브젝트)를 넣어줘!
    public GameObject recipeGroup; 

    void Awake() // Start보다 더 빠른 시점에 실행!
    {
        // [문제 해결 1] 게임 시작하자마자 무조건 숨기기
        if (recipeGroup != null)
        {
            recipeGroup.SetActive(false);
        }
    }

    void OnMouseDown()
    {
        // [문제 해결 2] 클릭하면 무조건 켜기
        if (recipeGroup != null)
        {
            recipeGroup.SetActive(true);
        }
    }
}