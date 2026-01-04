using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject[] levels;
    private int currentLevel = 0;

    private void Awake()
    {
        currentLevel = Data.currentLevel;
    }

    private void Start()
    {
        for(int i = 0; i <= currentLevel; i++)
        {
            levels[i].GetComponent<LevelButton>().Unlock();
        }
    }

}
