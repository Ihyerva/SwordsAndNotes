using UnityEngine;
using UnityEngine.UI;

public class CostumeMenuButtons : MonoBehaviour
{
    public Sprite costumeSprite;
    [SerializeField] private int index = 0;
    [SerializeField] private int requirment;
    [SerializeField] private GameObject costumeMenu;
     private bool isUnlocked = false;
    void Start()
    {
        if((int)Data.timer >= requirment)
        {
            Unlock();
        }
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            costumeMenu.SetActive(false);
        }
    }

    public void SelectCostume()
    {
        PlayerPrefs.SetInt("PlayerCostumeIndex", index);
        PlayerPrefs.Save();
    }

    public void Unlock()
    {
        isUnlocked = true;
        GetComponent<Image>().color = Color.white;
    }

    public void OnClick()
    {
        if(isUnlocked)
        {
            SelectCostume();
        }
    }
} 