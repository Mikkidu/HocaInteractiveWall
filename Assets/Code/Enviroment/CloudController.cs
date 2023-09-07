using UnityEngine;

public class CloudController : MonoBehaviour
{
    [SerializeField] private float _minSpeed = 5f;
    [SerializeField] private float _maxSpeed = 10f;
    [SerializeField] private float _detroyDistance;
    [SerializeField] private float _maxScale;

    private float _speed;

    

    private void Start()
    {
        transform.localScale *= Random.Range(1f, _maxScale);
        _speed = Random.Range(_minSpeed, _maxSpeed);
    }

    void Update()
    {
        if (transform.position.x > _detroyDistance)
        {
            Move();
        }
        else
        {
            DestroyObject();
        }
    }

    public void Initialize(float maxScale)
    {
        _maxScale = maxScale;
    }

    private void Move()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime);
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
