using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private string gameOverSceneName;
    [SerializeField] private GameEvent HealthReduced;
    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = startingHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered");
        if (other.CompareTag("Flat") || other.CompareTag("Sharp"))
        {
            Debug.Log("health reduced");
            Destroy(other.gameObject);
            ReduceHealth(1);
        }
    }

    private void ReduceHealth(int amount)
    {
        currentHealth -= amount;
        if (HealthReduced != null) HealthReduced.Raise(this, currentHealth);
        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(gameOverSceneName);
            Data.Save();
        }
    }

}
