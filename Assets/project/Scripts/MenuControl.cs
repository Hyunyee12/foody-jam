using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuControl : MonoBehaviour
{
    public void StartGame()
    {
        // Nó sẽ load cái Scene có tên là Sample
        SceneManager.LoadScene("Sample stage");
    }
}