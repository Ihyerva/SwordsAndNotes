using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToSkip : MonoBehaviour
{

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
