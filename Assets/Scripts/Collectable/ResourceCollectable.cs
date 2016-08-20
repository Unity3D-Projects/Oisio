﻿using UnityEngine;

public abstract class ResourceCollectable : MonoBehaviour, Collectable, Chargable
{
    private float minCollectPercentage = Mathf.Epsilon;

    #region Collectable implementation
    public Vector3 WorlPos
    {
        get
        {
            return transform.position;
        }
    }

    public abstract CollectableType type { get; }

    public virtual void Collect(Collector collector)
    {
        if (current.Equals(ChargableState.Charging)) return;

        percentage -= Time.deltaTime/collectionTime;

        if (percentage < minCollectPercentage) collector.CompleteCollection(type);
    }
    #endregion

    #region Chargable implementation
    public ChargableState current
    {
        get 
        {
            return state;
        }
    }
    private ChargableState state;
    #endregion


    [SerializeField] private CollectableStatusBar statusBar;

    [SerializeField] protected float collectionTime;
    [SerializeField] protected float reloadTime;

    protected float percentage = 1f;

    protected virtual void Awake()
    {
        statusBar.Init(percentage);
    }

    protected virtual void CheckStatus()
    {
        if (percentage < minCollectPercentage && current.Equals(ChargableState.Charged))
        {
            state = ChargableState.Charging;
        }
        else
        {
            percentage += Time.deltaTime/reloadTime;
            if (percentage > 1) state = ChargableState.Charged;
        }

        percentage = Mathf.Clamp01(percentage);
    }

    protected virtual void Update()
    {
        CheckStatus();
        statusBar.SetBarView(percentage, current);
    }
}