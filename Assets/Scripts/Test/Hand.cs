using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Hand : MonoBehaviour
{
    [SerializeField] private int maxHandSize = 10;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private SplineContainer splineContainer;

    private List<GameObject> handCards = new List<GameObject>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Draw();
        }
    }
    public void Draw()
    {
        if (handCards.Count >= maxHandSize) return;

        GameObject newCard = Instantiate(cardPrefab, spawnPoint.position, spawnPoint.rotation);
        handCards.Add(newCard);
        UpdateHandCardsPosition();
    }

    public void UpdateHandCardsPosition()
    {
        if (handCards.Count == 0) return;

        float cardSpacing = 1f / maxHandSize;
        float firstCardPosition=0.5f-(handCards.Count-1)*cardSpacing/2;
        Spline spline = splineContainer.Spline;

        for (int i = 0; i < handCards.Count; i++)
        {
            float p=firstCardPosition+i* cardSpacing;
            Vector3 splinePosition = spline.EvaluatePosition(p);
            Vector3 forward = spline.EvaluateTangent(p);
            Vector3 up =spline.EvaluateUpVector(p);
            Quaternion rotation = Quaternion.LookRotation(up, Vector3.Cross(up, forward).normalized);
            handCards[i].transform.DOMove(splinePosition, 0.25f);
            handCards[i].transform.DORotateQuaternion(rotation, 0.25f);
        }

    }

}
