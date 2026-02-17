using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Pool;

public class DamageTextManager : MonoBehaviour
{
    public static DamageTextManager Instance;


    [BoxGroup("预制体组件绑定")]
    [Required("必须绑定的预制体TextDamagetUp")]
    [SerializeField] TextDamagePopup damageTextPrefab;

    private ObjectPool<TextDamagePopup> _pool;

    private void Awake()
    {
        if(Instance !=null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        Enemy.OnDamageTaken += OnEnemyHit;

        _pool = new ObjectPool<TextDamagePopup>(
            createFunc: () => Instantiate<TextDamagePopup>(damageTextPrefab),
            actionOnGet:(popup)=>popup.gameObject.SetActive(true),//拿出来的时候
            actionOnRelease:(popup)=>popup.gameObject.SetActive(false),//收回来的时候
            actionOnDestroy:(popup)=>Destroy(popup.gameObject),//池子爆满时干嘛
            collectionCheck:false,//关闭重复性能检查
            defaultCapacity:20,//默认池子20个
            maxSize:50 //最多允许同频200个
            );
    }

    private void OnDestroy()
    {
        Enemy.OnDamageTaken -= OnEnemyHit;
    }

    public void OnEnemyHit(int damage,Vector3 enemPos)
    {
        TextDamagePopup popup = _pool.Get();
        popup.SetUp(damage, enemPos, ReleaseToPool);
    }


    private void ReleaseToPool(TextDamagePopup popupToReturn)
    {
        _pool.Release(popupToReturn);
    }
}
