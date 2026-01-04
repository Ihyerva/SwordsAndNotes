using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LevelButton : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private bool unlocked;
    [SerializeField] private Image[] images;
    [SerializeField] private TMP_Text[] texts;

    public void OnClick()
    {
        if(unlocked)
            SceneManager.LoadScene(levelName);
    }

    public void Unlock()
    {
        unlocked = true;
        for(int i = 0; i < images.Length; i++)
        {
            images[i].color = Color.white;
        }
        for(int i = 0; i < texts.Length; i++)
        {
            texts[i].color = Color.white;
        }
    }
}
