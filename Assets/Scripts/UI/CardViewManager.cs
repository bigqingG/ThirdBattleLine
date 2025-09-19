using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CardViewManager : MonoBehaviour
{
    #region 旧代码
    //public static CardViewManager Instance { get; private set; }

    //public GameObject cardViewPrefab;
    //private Dictionary<RuntimeCard, CardViewController> activeViews = new Dictionary<RuntimeCard, CardViewController>();
    //private Queue<CardViewController> viewPool = new Queue<CardViewController>();

    //private void Awake()
    //{
    //    if (Instance != null && Instance != this)
    //    {
    //        Destroy(gameObject);
    //        return;
    //    }

    //    Instance = this;
    //    DontDestroyOnLoad(gameObject);
    //}

    //public CardViewController GetView(RuntimeCard card)
    //{
    //    if (activeViews.TryGetValue(card, out var existingView))
    //        return existingView;

    //    // 从池中获取或创建新视图
    //    CardViewController view;
    //    if (viewPool.Count > 0)
    //    {
    //        view = viewPool.Dequeue();
    //        view.gameObject.SetActive(true);
    //    }
    //    else
    //    {
    //        var newObj = Instantiate(cardViewPrefab);
    //        view = newObj.GetComponent<CardViewController>();
    //    }

    //    // 初始化视图
    //    view.Initialize(card);
    //    activeViews[card] = view;

    //    return view;
    //}

    //public void ReturnView(RuntimeCard card)
    //{
    //    if (activeViews.TryGetValue(card, out var view))
    //    {
    //        view.Cleanup();
    //        view.gameObject.SetActive(false);
    //        viewPool.Enqueue(view);
    //        activeViews.Remove(card);
    //    }
    //}
    #endregion
    public static CardViewManager Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] private int defaultMaxSize = 20; // 默认每池大小

    // 核心：Prefab -> 专属池
    private Dictionary<GameObject, ObjectPool<CardViewController>> prefabPools = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 可选：预加载常见Prefab的池（从CardGenerator的SO列表）
        //PreloadPoolsFromSOs(); // 见下方方法
    }

    // 获取视图：动态基于cardSO.cardPrefab
    public CardViewController GetView(RuntimeCard card)
    {
        if (card?.sourceData?.cardPrefab == null)
        {
            Debug.LogError($"CardSO {card?.sourceData?.name} missing cardPrefab! Fallback to default.");
            return null; // 或用默认Prefab兜底
        }

        var prefab = card.sourceData.cardPrefab;
        if (!prefabPools.TryGetValue(prefab, out var pool))
        {
            // 无池：创建新池
            pool = CreatePoolForPrefab(prefab);
            prefabPools[prefab] = pool;
        }

        var view = pool.Get();
        view.Initialize(card); // 绑定数据，触发UpdateDisplay
        return view;
    }

    // 回收视图：基于视图的Prefab key
    public void ReturnView(CardViewController view)
    {
        if (view == null || view.BoundCard?.sourceData?.cardPrefab == null) return;

        var prefab = view.BoundCard.sourceData.cardPrefab;
        if (prefabPools.TryGetValue(prefab, out var pool))
        {
            pool.Release(view);
        }
        else
        {
            // 兜底：销毁非池视图
            Destroy(view.gameObject);
        }
    }

    // 辅助：为Prefab创建池
    private ObjectPool<CardViewController> CreatePoolForPrefab(GameObject prefab)
    {
        return new ObjectPool<CardViewController>(
            createFunc: () => {
                var instance = Instantiate(prefab);
                var cardView = instance.GetComponent<CardViewController>();
                if (cardView == null) cardView = instance.AddComponent<CardViewController>(); // 确保基类
                return cardView;
            },
            actionOnGet: view => view.gameObject.SetActive(true),
            actionOnRelease: view => {
                view.gameObject.SetActive(false);
                view.Cleanup(); // 清空引用、停止粒子
            },
            actionOnDestroy: view => Destroy(view.gameObject),
            maxSize: defaultMaxSize,
            collectionCheck: true // Debug用，Release建检查泄漏
        );
    }

    // 扩展：从CardSO列表预热池（游戏启动时调用，e.g., 从Deck加载）
    public void PreloadPoolsFromSOs(List<CardSO> sos)
    {
        var uniquePrefabs = new HashSet<GameObject>();
        foreach (var so in sos)
        {
            if (so?.cardPrefab != null) uniquePrefabs.Add(so.cardPrefab);
        }
        foreach (var prefab in uniquePrefabs)
        {
            var pool = CreatePoolForPrefab(prefab);
            prefabPools[prefab] = pool;
            // 预取2个热身
            for (int i = 0; i < 2; i++) pool.Get();
        }
    }
}
