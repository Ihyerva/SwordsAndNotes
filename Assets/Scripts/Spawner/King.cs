using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class King : MonoBehaviour
{
    [SerializeField] private GameEvent throwSword;
    [SerializeField] private GameEvent updateScore;
    [SerializeField] private string sceneToLoad;
    private int index = 0;
    private float timer = 0f;

    private void Start()
    { 
        StartCoroutine(SpawnSword());
    }

    private void Update()
    {
        timer += Time.deltaTime;
        updateScore.Raise(this, timer);
        if(timer>Data.timer){
            Data.timer = timer;
        }
    }

    IEnumerator SpawnSword()
    {
        int firstNumber = Random.Range(0, 11);
        if (firstNumber == 1 || firstNumber == 5 || firstNumber == 8 || firstNumber == 11)
        {
        SwordEventData swordEventData = new SwordEventData(false, firstNumber);
        throwSword.Raise(this, swordEventData);
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        StartCoroutine(SpawnSword());
        }
        else{
        int secondNumber = Random.Range(0, 2);
        bool secondBool = secondNumber == 1;
        SwordEventData swordEventData = new SwordEventData(secondBool, firstNumber);
        throwSword.Raise(this, swordEventData);
        yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        StartCoroutine(SpawnSword());
        }
    }

    private void OnApplicationQuit()
    {
        Data.Save();
    }
}
