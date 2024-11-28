using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Point : MonoBehaviour
{
    [SerializeField] private CanvasGroup group;

    [SerializeField] private bool destroyParent;
    [SerializeField] private float visibleTimer = 5;

    public float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        float procent = timer / visibleTimer;

        group.alpha = 1-procent;

        if (procent >= 1) Destroy(destroyParent?transform.parent.gameObject : gameObject);
    }
}
