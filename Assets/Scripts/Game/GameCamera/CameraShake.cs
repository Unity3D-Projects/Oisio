﻿using UnityEngine;
using System.Collections.Generic;

// will cntain all different shake effects
[CreateAssetMenu(fileName = GameConfig.CAMERA_SHAKE_NAME, menuName = "Oisio / CameraShakes", order = 1)]
public class CameraShake : ScriptableObject
{
    private static CameraShake instance;
    public static CameraShake Instance
    {
        get
        {
            if (instance == null) instance = Resources.Load(GameConfig.CAMERA_SHAKE_PATH) as CameraShake;
            return instance;
        }
    }

    [SerializeField] private Shake[] configuredShakes;
    private List<Shake> activeShakes = new List<Shake>();

    public void FrameFeed()
    {
        for(int i = 0; i < activeShakes.Count; i ++)
        {
            activeShakes[i].FrameFeed();
            if (activeShakes[i].hasFinished())
            {
                activeShakes.RemoveAt(i);
                if (i > 0) i--;
            }
        }
    }

    public void StartShake(ShakeType shake)
    {
        StartShakes(new ShakeType[] {shake});
    }

    public void StartShakes(params ShakeType[] shakes)
    {
        foreach(Shake config in configuredShakes)
            foreach(ShakeType sht in shakes)
                if (config.type.Equals(sht))
                {
                    config.Reset();
                    if (!activeShakes.Contains(config)) activeShakes.Add(config);
                }
    }

    public void StopShake(ShakeType shake)
    {
        StopShakes(new ShakeType[] {shake});
    }

    public void StopShakes(params ShakeType[] shakes)
    {
        for(int i = 0; i < activeShakes.Count; i ++)
            foreach(ShakeType sht in shakes)
                if (activeShakes[i].type.Equals(sht) && activeShakes.Contains(activeShakes[i]))
                {
                    activeShakes.Remove(activeShakes[i]);
                    if (i > 0) i--;
                }
    }

    // based on the upated active shake it will calculate a local shake point
    public Vector2 GetShakePoint()
    {
        Vector2 center = Vector2.zero;

        foreach(Shake shake in activeShakes)
        {
            Vector2 shakePoint = shake.GetShakePoint();
            if (center.sqrMagnitude < Mathf.Epsilon) center = shakePoint;

            center.x += Mathf.Abs(center.x - shakePoint.x) * Mathf.Sin(center.x);
            center.y += Mathf.Abs(center.y - shakePoint.y) * Mathf.Sin(center.y);
        }

        return center;
    }
}

// will contain data for different shakes
[System.Serializable]
public class Shake
{
    // just to keep the editor organized
    public string shakeName;

    public ShakeType type;
    public float maxAmplitude;
    public float decay;

    [HideInInspector] private float currentAmplitude;

    public void Reset()
    {
        currentAmplitude = maxAmplitude;
    }

    public void FrameFeed()
    {
        currentAmplitude -= decay * Time.deltaTime;
    }

    public Vector2 GetShakePoint()
    {
        return hasFinished() ? Vector2.zero : Random.insideUnitCircle * currentAmplitude;
    }

    public bool hasFinished()
    {
        return currentAmplitude < 0f;
    }
}

public enum ShakeType
{
    Run,
    GiantAttack,
    JumperAttack
}