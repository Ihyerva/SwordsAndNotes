using UnityEngine;

public class Note : MonoBehaviour
{
    public bool isSharp;
    [SerializeField] private GameObject sharpSymbol;
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
        }
    }

    private void Start()
    {
        Destroy(gameObject, 10f);
    }

    public void SetAsSharp()
    {
        isSharp = true;
        sharpSymbol.SetActive(true);
        gameObject.tag = "Sharp";
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = Vector2.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag(gameObject.tag))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
