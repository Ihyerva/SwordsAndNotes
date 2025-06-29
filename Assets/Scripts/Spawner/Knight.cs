using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Knight : MonoBehaviour
{
    [SerializeField] private int currentLevel;
    [SerializeField] private int[] locations;
    [SerializeField] private float[] times;
    [SerializeField] private bool[] isSharp;
    [SerializeField] private GameEvent throwSword;
    private int index = 0;

    private void Start()
    {

        StartCoroutine(SpawnSword());
    }


    IEnumerator SpawnSword()
    {
        if(index < locations.Length){
            yield return new WaitForSeconds(times[index]);
            var eventData = new SwordEventData(isSharp[index], locations[index]);
            throwSword.Raise(this, eventData);
            index++;
            StartCoroutine(SpawnSword());
        }
        else if(Data.currentLevel<currentLevel){
            yield return new WaitForSeconds(10);
            Data.currentLevel=currentLevel;
            Data.Save();
            SceneManager.LoadScene("LevelSelection");
        }
    }


}
