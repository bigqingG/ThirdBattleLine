using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public EventSystem EventSystem { get; private set; }
    public CardGenerator CardGenerator { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ��ʼ����ϵͳ
        EventSystem = gameObject.AddComponent<EventSystem>();
        CardGenerator = gameObject.AddComponent<CardGenerator>();
    }

}
