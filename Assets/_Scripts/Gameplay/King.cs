using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class King : MonoBehaviour
{
    [SerializeField] private GameEvent throwSword;
    [SerializeField] private GameEvent updateScore;
    [SerializeField] private string sceneToLoad;
    private float timer = 0f;

    private HashSet<NoteType> sharpableNotes = new HashSet<NoteType> { 
        NoteType.C4, NoteType.D4, NoteType.F4, NoteType.G4, NoteType.A4
    };

    private List<NoteType> possibleNotes = new List<NoteType> {
        NoteType.C4, NoteType.D4, NoteType.E4, NoteType.F4, NoteType.G4, NoteType.A4, NoteType.B4
    };

    private void Start()
    { 
        StartCoroutine(SpawnSword());
    }

    private void Update()
    {
        timer += Time.deltaTime;
        updateScore.Raise(this, timer);
        Data.timer = timer;
    }

    IEnumerator SpawnSword()
    {   
        while (true)
        {
            NoteType baseNote = possibleNotes[Random.Range(0, possibleNotes.Count)];
            bool isSharp = sharpableNotes.Contains(baseNote) && Random.Range(0, 2) == 1;
            NoteType noteType = baseNote;
            if (isSharp)
            {
                noteType |= NoteType.Sharp;
            }
            throwSword.Raise(this, noteType);
            yield return new WaitForSeconds(Random.Range(0.5f, 2f));
        }
    }

    private void OnApplicationQuit()
    {
        Data.Save();
    }
}