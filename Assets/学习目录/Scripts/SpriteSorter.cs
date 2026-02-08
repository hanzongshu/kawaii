using UnityEngine;

public class SpriteSorter : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

   
    void Update()
    {
        _spriteRenderer.sortingOrder = -(int)(transform.position.y * 10);
    }
}
