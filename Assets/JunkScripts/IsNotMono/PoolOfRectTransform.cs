using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolOfRectTransform
{
    public RectTransform CanonicalRect;

    private Queue<RectTransform> _rtPool = new Queue<RectTransform>();
    private Queue<RectTransform> _shadowPool = new Queue<RectTransform>();

    public void SetCanonicalRect(RectTransform blankRect)
    {
        CanonicalRect = blankRect;
    }

    public void RefreshQueue()
    {
        _rtPool = new Queue<RectTransform>(_shadowPool);
    }

    public void SetQueue(Queue<RectTransform> newRtQueue)
    {
        _rtPool = newRtQueue;
    }

    public Queue<RectTransform> GetQueue()
    {
        return _rtPool;
    }

    public void SetActive(bool isActive)
    {
        foreach (RectTransform rectTransform in _rtPool)
        {
            rectTransform.gameObject.SetActive(isActive);
        }
    }

    public int Count()
    {
        return _rtPool.Count;
    }

    public RectTransform Dequeue()
    {
        if (_rtPool.Count > 0)
        {
            return _rtPool.Dequeue();
        }
        else
        {
            RectTransform rt = Object.Instantiate(CanonicalRect, CanonicalRect.transform.position, Quaternion.identity);
            
            _rtPool.Enqueue(rt);
            _shadowPool.Enqueue(rt);

            return Dequeue();
        }
    }
}
