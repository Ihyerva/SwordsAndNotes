using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private Vector2 direction;
    [SerializeField] private float speed = 5f;
    [SerializeField] private GameObject sharpSymbol;
    public bool isSharp { get; private set; } = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Invoke("Deactivate", 10f);
    }

    public void SetAsSharp()
    {
        isSharp = true;
        sharpSymbol.SetActive(true);
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Projectile>(out Projectile otherProjectile))
            return;
        if (isSharp != otherProjectile.isSharp)
            return;
        Deactivate();
    }
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}