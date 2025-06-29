using UnityEngine;
using UnityEngine.UI;

public class PlayerCostume : MonoBehaviour
{
    public Sprite[] costumeSprites;


    public void Awake()
    {
        int costumeIndex = PlayerPrefs.GetInt("PlayerCostumeIndex", 0);
        if (costumeSprites != null && costumeSprites.Length > 0 && costumeIndex >= 0 && costumeIndex < costumeSprites.Length)
        {
            if(gameObject.GetComponent<SpriteRenderer>() != null)
            {
                var sr = GetComponent<SpriteRenderer>();
                sr.sprite = costumeSprites[costumeIndex];
            }
            else if(gameObject.GetComponent<Image>() != null)
            {
                var im = GetComponent<Image>();
                im.sprite = costumeSprites[costumeIndex];
            }
        }
    }

}
