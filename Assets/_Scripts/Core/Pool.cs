using System.Collections.Generic;
using UnityEngine;

public class Pool
{
private Queue<GameObject> _poolQueue = new Queue<GameObject>();
private GameObject _prefab;
public void Preload(GameObject prefab, int count)
{
    _prefab = prefab;
    for (int i = 0; i < count; i++)
    {
        GameObject obj = GameObject.Instantiate(_prefab);
        obj.SetActive(false);
        _poolQueue.Enqueue(obj);
    }
}
public GameObject GetFromPool(Vector3 position)
{
    GameObject obj = _poolQueue.Dequeue();
    _poolQueue.Enqueue(obj);
    if (obj.activeSelf)
    {
        obj = GameObject.Instantiate(_prefab);
        _poolQueue.Enqueue(obj);
    }
    obj.SetActive(true);
    obj.transform.position = position;
    return obj;
}

}