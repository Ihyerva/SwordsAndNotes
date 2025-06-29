using UnityEngine;

public class Sword : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject sharpSymbol;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = Vector2.left * speed;
    }

    public void SetAsSharp()
    {
        sharpSymbol.SetActive(true);
        gameObject.tag = "Sharp";
    }
}
