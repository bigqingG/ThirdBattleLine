using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CardViewManager : MonoBehaviour
{
    #region �ɴ���
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

    //    // �ӳ��л�ȡ�򴴽�����ͼ
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

    //    // ��ʼ����ͼ
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
    [SerializeField] private int defaultMaxSize = 20; // Ĭ��ÿ�ش�С

    // ���ģ�Prefab -> ר����
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

        // ��ѡ��Ԥ���س���Prefab�ĳأ���CardGenerator��SO�б�
        //PreloadPoolsFromSOs(); // ���·�����
    }

    // ��ȡ��ͼ����̬����cardSO.cardPrefab
    public CardViewController GetView(RuntimeCard card)
    {
        if (card?.sourceData?.cardPrefab == null)
        {
            Debug.LogError($"CardSO {card?.sourceData?.name} missing cardPrefab! Fallback to default.");
            return null; // ����Ĭ��Prefab����
        }

        var prefab = card.sourceData.cardPrefab;
        if (!prefabPools.TryGetValue(prefab, out var pool))
        {
            // �޳أ������³�
            pool = CreatePoolForPrefab(prefab);
            prefabPools[prefab] = pool;
        }

        var view = pool.Get();
        view.Initialize(card); // �����ݣ�����UpdateDisplay
        return view;
    }

    // ������ͼ��������ͼ��Prefab key
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
            // ���ף����ٷǳ���ͼ
            Destroy(view.gameObject);
        }
    }

    // ������ΪPrefab������
    private ObjectPool<CardViewController> CreatePoolForPrefab(GameObject prefab)
    {
        return new ObjectPool<CardViewController>(
            createFunc: () => {
                var instance = Instantiate(prefab);
                var cardView = instance.GetComponent<CardViewController>();
                if (cardView == null) cardView = instance.AddComponent<CardViewController>(); // ȷ������
                return cardView;
            },
            actionOnGet: view => view.gameObject.SetActive(true),
            actionOnRelease: view => {
                view.gameObject.SetActive(false);
                view.Cleanup(); // ������á�ֹͣ����
            },
            actionOnDestroy: view => Destroy(view.gameObject),
            maxSize: defaultMaxSize,
            collectionCheck: true // Debug�ã�Release�����й©
        );
    }

    // ��չ����CardSO�б�Ԥ�ȳأ���Ϸ����ʱ���ã�e.g., ��Deck���أ�
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
            // Ԥȡ2������
            for (int i = 0; i < 2; i++) pool.Get();
        }
    }
}
