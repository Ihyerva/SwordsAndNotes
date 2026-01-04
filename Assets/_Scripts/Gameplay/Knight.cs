using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Knight : MonoBehaviour
{
    [SerializeField] private int currentLevel;
    [SerializeField] private NoteType[] swords;
    [SerializeField] private float[] times;
    [SerializeField] private GameEvent throwSword;

    private void Start()
    {
        StartCoroutine(ThrowSword());
    }

    IEnumerator ThrowSword()
    {
        for(int index = 0; index < swords.Length; index++)
        {
            yield return new WaitForSeconds(times[index]);
            throwSword.Raise(this, swords[index]);
        }
        yield return new WaitForSeconds(10);
        if(Data.currentLevel<currentLevel){
            Data.currentLevel=currentLevel;
            Data.Save();
        }
        SceneManager.LoadScene("LevelSelection");   
    }


}
