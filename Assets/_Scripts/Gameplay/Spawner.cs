using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject notePrefab;
    private Pool _notePool;
    private Pool _swordPool;
    private Transform[] SwordSpawnPoints;
    private Transform[] NoteSpawnPoints;

    private static readonly Dictionary<NoteType, int> noteToIndex = new Dictionary<NoteType, int>
    {
        // All notes of the same pitch class spawn at the same position (4th octave positions)
        {NoteType.D3, 0}, {NoteType.D4, 0}, {NoteType.D5, 0},
        {NoteType.E2, 1}, {NoteType.E3, 1}, {NoteType.E4, 1}, {NoteType.E5, 1},
        {NoteType.F2, 2}, {NoteType.F3, 2}, {NoteType.F4, 2}, {NoteType.F5, 2},
        {NoteType.G2, 3}, {NoteType.G3, 3}, {NoteType.G4, 3}, {NoteType.G5, 3},
        {NoteType.A2, 4}, {NoteType.A3, 4}, {NoteType.A4, 4},
        {NoteType.B2, 5}, {NoteType.B3, 5}, {NoteType.B4, 5},
        {NoteType.C3, 6}, {NoteType.C4, 6}, {NoteType.C5, 6}
    };

    private void Awake()
    {
        NoteSpawnPoints = transform.GetChild(0).GetComponentsInChildren<Transform>().Skip(1).ToArray();
        SwordSpawnPoints = transform.GetChild(1).GetComponentsInChildren<Transform>().Skip(1).ToArray();
    
    }
    private void Start()
    {
        _notePool = new Pool();
        _notePool.Preload(notePrefab, 25);

        _swordPool = new Pool();
        _swordPool.Preload(sword, 15);
    }

    public void SpawnSword(Component sender, object data)
    {
        NoteType noteType = (NoteType)data;
        bool isSharp = noteType.HasFlag(NoteType.Sharp);
        NoteType baseType = noteType & ~NoteType.Sharp;
        
        if (!noteToIndex.TryGetValue(baseType, out int location))
        {
            return;
        }

        if (SwordSpawnPoints[location] == null)
        {
            return;
        }

        GameObject swordInstance = _swordPool.GetFromPool(SwordSpawnPoints[location].position);
        
        if(isSharp)
            swordInstance.GetComponent<Projectile>().SetAsSharp();
    }

    public void SpawnNote(Component sender, object data)
    {
        NoteType noteType = (NoteType)data;
        GameObject note = _notePool.GetFromPool(Vector3.zero);
        NoteType baseType = noteType & ~NoteType.Sharp;
        bool isSharp = (noteType & NoteType.Sharp) != 0;
        if (isSharp)
        {
            note.GetComponent<Projectile>().SetAsSharp();
        }
        if (noteToIndex.TryGetValue(baseType, out int index))
        {
            note.transform.position = NoteSpawnPoints[index].position;
        }
        else
        {
            Destroy(note);
        }
    }
}