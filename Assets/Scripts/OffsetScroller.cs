using UnityEngine;
using System.Collections;
using JetBrains.Annotations;

public class OffsetScroller : MonoBehaviour
{
    private Vector3 _initialPosition;

    private GameObject _secondGuy;

    private MeshRenderer _renderer;

    public Vector3 Speed = new Vector3(-.1f, 0f, 0f);

    public bool IsClone = false;

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _initialPosition = transform.position;
    }

    [UsedImplicitly]
    void Start()
    {
        if (IsClone) return;

        _secondGuy = Instantiate(gameObject);
        _secondGuy.GetComponent<OffsetScroller>().IsClone = true;
        _secondGuy.transform.position = new Vector3(_initialPosition.x + _renderer.bounds.size.x, _initialPosition.y,
            _initialPosition.z);
    }

    [UsedImplicitly]
    void Update()
    {
        transform.position += Speed * Time.deltaTime;

        if (transform.position.x + _renderer.bounds.size.x < _initialPosition.x)
        {
            transform.position += new Vector3(2 * _renderer.bounds.size.x, 0f, 0f);
        }
    }
}

