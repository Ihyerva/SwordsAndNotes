using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject notePrefab; // Order must match allNotes!
    private bool levelStarted = false;
    private static readonly string[] allNotes = new string[] {
        "D4","D#4","E4","F4","F#4","G4","G#4","A4","A#4","B4",
        "C5","C#5","D5","D#5","E5","F5","F#5","G5","G#5","A5","A#5","B5"
    };

    private Transform[] SwordSpawnPoints;
    private Transform[] NoteSpawnPoints;

    private void Awake()
    {
        if (transform.childCount < 2)
        {
            Debug.LogError($"Spawner GameObject '{gameObject.name}' does not have enough children! childCount={transform.childCount}");
        }
        else
        {
            NoteSpawnPoints = transform.GetChild(0).GetComponentsInChildren<Transform>().Skip(1).ToArray();
            SwordSpawnPoints = transform.GetChild(1).GetComponentsInChildren<Transform>().Skip(1).ToArray();
            Debug.Log($"NoteSpawnPoints found: {NoteSpawnPoints.Length}");
            Debug.Log($"SwordSpawnPoints found: {SwordSpawnPoints.Length}");
            if (SwordSpawnPoints.Length == 0)
            {
                Debug.LogError("No SwordSpawnPoints found! Check the hierarchy under the second child of the Spawner GameObject.");
            }
            if (NoteSpawnPoints.Length == 0)
            {
                Debug.LogError("No NoteSpawnPoints found! Check the hierarchy under the first child of the Spawner GameObject.");
            }
        }
    }

    private void Start()
    {
        // ... existing code ...
    }

    public void SpawnSword(Component sender, object data)
    {
        var eventData = data as SwordEventData;
        if (eventData == null)
        {
            Debug.LogError("Data is not of type SwordEventData!");
            return;
        }
        bool isSharp = eventData.isSharp;
        int location = eventData.location;
        Debug.Log($"SpawnSword called with location: {location}, isSharp: {isSharp}");
        if (SwordSpawnPoints == null)
        {
            Debug.LogError("SwordSpawnPoints is null!");
            return;
        }
        if (sword == null)
        {
            Debug.LogError("Sword prefab is not assigned!");
            return;
        }
        Debug.Log($"SwordSpawnPoints length: {SwordSpawnPoints.Length}");
        int index = location;
        if (index < 0 || index >= SwordSpawnPoints.Length)
        {
            Debug.LogError($"Index {index} is out of range for SwordSpawnPoints (length {SwordSpawnPoints.Length})");
            return;
        }
        if (SwordSpawnPoints[index] == null)
        {
            Debug.LogError($"SwordSpawnPoints[{index}] is null!");
            return;
        }
        // You can use isSharp here to modify the sword if needed
        GameObject swordInstance = Instantiate(sword, SwordSpawnPoints[index].position, sword.transform.rotation);
        if(isSharp){
            swordInstance.GetComponent<Sword>().SetAsSharp();
        }

    }

    public void SpawnNote(Component sender, object data)
    {
        string noteName = data.ToString();
        Debug.Log(noteName);
        GameObject note = Instantiate(notePrefab, new Vector2(0,0), Quaternion.identity);
        if(noteName.Contains("#")){
            note.GetComponent<Note>().SetAsSharp();
            noteName = noteName.Replace("#", "");
        }
        if(note != null){
        switch(noteName){
            case "D4":
            if(note != null){
                note.transform.position = NoteSpawnPoints[0].position;
            }
                break; 
            case "E4":
            if(note != null){
                note.transform.position = NoteSpawnPoints[1].position;
            }
                break;
            case "F4":
            if(note != null){
                note.transform.position = NoteSpawnPoints[2].position;
            }
                break;      
            case "G4":
            if(note != null){
                note.transform.position = NoteSpawnPoints[3].position;
            }
                break;      
            case "A4":
            if(note != null){
                note.transform.position = NoteSpawnPoints[4].position;
            }
                break;  
            case "B4":
            if(note != null){
                note.transform.position = NoteSpawnPoints[5].position;
            }
                break;  
            case "C5":
            if(note != null){
                note.transform.position = NoteSpawnPoints[6].position;
            }
                break;  
            case "D5":
            if(note != null){
                note.transform.position = NoteSpawnPoints[7].position;
            }
                break;
            case "E5":
            if(note != null){
                note.transform.position = NoteSpawnPoints[8].position;
            }
                break;
            case "F5":
            if(note != null){
                note.transform.position = NoteSpawnPoints[9].position;
            }
                break;
            case "G5":
            if(note != null){
                note.transform.position = NoteSpawnPoints[10].position;
            }
            break;
            default:
            if(note != null){
                Destroy(note);
            }
            break;
            }
        }
    }

    
}
