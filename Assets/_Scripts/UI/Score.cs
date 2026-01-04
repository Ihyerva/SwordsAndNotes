using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Score : MonoBehaviour
{
    private float score = 0;
    

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "LevelSelection") // Change "LevelSelection" to your actual scene name if needed
        {
            UpdateScore(this, Data.timer);
        }
    }

    public void  UpdateScore(Component sender, object data){
        score = (float)data;
        GetComponent<TextMeshProUGUI>().text = score.ToString("F2");
    }
}
