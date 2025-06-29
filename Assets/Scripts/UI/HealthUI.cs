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

        while (hearts.Count > health)
        {
            Destroy(hearts[hearts.Count - 1]);
            hearts.RemoveAt(hearts.Count - 1);
        }
    }
} 