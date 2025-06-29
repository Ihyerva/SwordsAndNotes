using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class LevelButton : MonoBehaviour
{
    [SerializeField] private string levelName;
    [SerializeField] private bool unlocked;
    public void OnClick()
    {
        Debug.Log("LevelButton clicked: " + levelName);
        if(unlocked)
            SceneManager.LoadScene(levelName);
    }

    public void Unlock()
    {
        unlocked = true;
        GetComponent<Image>().color = new Color(255f/255f, 155f/255f, 0f/255f);
        if (transform.childCount > 0)
        {
            var firstChild = transform.GetChild(0).GetComponent<Image>();
            if (firstChild != null)
                firstChild.color = Color.white;
        }
        if (transform.childCount > 1)
        {
            var secondChildTMP = transform.GetChild(1).GetComponent<TMP_Text>();
            if (secondChildTMP != null)
                secondChildTMP.color = new Color(255f/255f, 155f/255f, 0f/255f);
        }
        if (transform.childCount > 2)
        {
            var thirdChildTMP = transform.GetChild(2).GetComponent<TMP_Text>();
            if (thirdChildTMP != null)
                thirdChildTMP.color = new Color(255f/255f, 155f/255f, 0f/255f);
        }
        if (transform.childCount > 3)
        {
            var fourthChildTMP = transform.GetChild(3).GetComponent<TMP_Text>();
            if (fourthChildTMP != null)
                fourthChildTMP.color = new Color(255f/255f, 155f/255f, 0f/255f);
        }
    }
}
