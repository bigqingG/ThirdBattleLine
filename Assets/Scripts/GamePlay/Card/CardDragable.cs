using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragable : MonoBehaviour,IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("��ק����")]
    [SerializeField] private RectTransform playZone; // ��������Inspector��ֵ�����Զ�Find��
    [SerializeField] private float dragScale = 1f; // ��קʱ�Ŵ���
    [SerializeField] private float returnDuration = 0.3f; // ���ض���ʱ��
    [SerializeField] private float destroyDelay = 2f; // ���ƺ������ӳ�

    private RectTransform rectTransform;
    private Canvas canvas;
    private CardViewController cardView; // �������boundCard��Cleanup
    private RuntimeCard boundCard; // ����ʱ��������
    private Vector2 originalPosition; // ��קǰԭλ�ã�anchoredPosition��
    private bool isDragging = false;

    private void Awake()
    {
        //playZone = GameObject.Find("Canvas/PlayZone")?.GetComponent<RectTransform>();
        rectTransform = GetComponent<RectTransform>();
        canvas = FindObjectOfType<Canvas>();
        cardView = GetComponent<CardViewController>(); // ��ȡ��ͼ������

        //if (playZone == null)
        //{
        //    playZone = GameObject.Find("PlayZone")?.GetComponent<RectTransform>();
        //    if (playZone == null) Debug.LogError("PlayZone not found! ����Canvas�´���PlayZone GameObject��");
        //}
    }

    // ��ק��ʼ�����Ȩ�ޣ���¼λ�ã��Ŵ���
    public void OnPointerDown(PointerEventData eventData)
    {
        if (cardView == null || cardView.BoundCard == null) return; // ��ͼδ��ʼ��
        boundCard = cardView.BoundCard;
        if (boundCard.owner == null || !boundCard.owner.isHuman) return; // ֻ�������������ק

        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        rectTransform.localScale = Vector3.one * dragScale;
        Debug.Log($"��ʼ��ק����: {boundCard.sourceData.cardData.displayName}");
    }

    // ��ק�У�������꣨��Ļ��ת���ص㣩
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint
        );
        rectTransform.anchoredPosition = localPoint;
    }

    // ��ק������������򣬴�����ƻ򷵻�
    public void OnEndDrag(PointerEventData eventData)
    {
        //if (!isDragging) return;
        //isDragging = false;
        ////rectTransform.localScale = Vector3.one; // �ָ���С

        //Vector2 endScreenPos = eventData.position;
        //bool inPlayZone = RectTransformUtility.RectangleContainsScreenPoint(playZone, endScreenPos, canvas.worldCamera);

        //if (inPlayZone)
        //{
        //    // �ڳ��������̶����ɿ�λ�ã����Գ��ƣ����������
        //    Vector2 finalLocalPos;
        //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //        playZone,
        //        endScreenPos,
        //        canvas.worldCamera,
        //        out finalLocalPos
        //    );
        //    rectTransform.SetParent(playZone, false); // �Ƶ���������worldPositionStays=false���ֱ������꣩
        //    rectTransform.anchoredPosition = finalLocalPos;

        //    // ģ����ƣ�����PlayerController��
        //    if (boundCard.owner.TryPlayCard(boundCard))
        //    {
        //        Debug.Log($"���Ƴɹ�: {boundCard.sourceData.cardData.displayName}");
        //        StartCoroutine(DestroyAfterDelay(destroyDelay));
        //    }
        //    else
        //    {
        //        // ����ʧ�ܣ�����ԭλ
        //        ReturnToOriginal();
        //    }
        //}
        //else
        //{
        //    // ���ڳ�����������ԭλ
        //    ReturnToOriginal();
        //}
    }

    // ����ԭλ�ö���Э��
    private void ReturnToOriginal()
    {
        // �����û�ԭ����������������������anchoredPosition��Ч
        if (cardView != null && cardView.transform.parent != null)
        {
            rectTransform.SetParent(cardView.transform.parent, false);
        }
        StartCoroutine(AnimateReturn(originalPosition, returnDuration));
    }

    private IEnumerator AnimateReturn(Vector2 targetPos, float duration)
    {
        Vector2 startPos = rectTransform.anchoredPosition;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        rectTransform.anchoredPosition = targetPos;
    }

    // �ӳ�����Э�̣������������٣�������ͼ�����س�
    private IEnumerator DestroyAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (boundCard != null)
        {
            boundCard.DestroyCard(); // ��������¼�����
        }
        if (cardView != null)
        {
            cardView.Cleanup(); // ����¼�
            CardViewManager.Instance.ReturnView(cardView); // ������ͼ��
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cardView == null || cardView.BoundCard == null) return; // ��ͼδ��ʼ��
        boundCard = cardView.BoundCard;
        if (boundCard.owner == null || !boundCard.owner.isHuman) return; // ֻ�������������ק

        isDragging = true;
        originalPosition = rectTransform.anchoredPosition;
        //rectTransform.localScale = Vector3.one * dragScale;
        Debug.Log($"��ʼ��ק����: {boundCard.sourceData.cardData.displayName}");
    }


    ///Gizmo���Ƴ�������
    private void OnDrawGizmos()
    {
        if (playZone != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(playZone.position, playZone.sizeDelta);
        }
    }
}
