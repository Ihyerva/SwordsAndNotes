using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private List<GameObject> hearts = new List<GameObject>();

    void Start()
    {
        UpdateHearts(3);
    }


    public void OnHealthReduced(Component sender, object data)
    {
        int newHealth = (int)data;
        UpdateHearts(newHealth);
    }

    private void UpdateHearts(int health)
    {

        for (int i = hearts.Count - 1; i >= 0; i--)
        {
            hearts[i].SetActive(i < health);
        }

    }
} 