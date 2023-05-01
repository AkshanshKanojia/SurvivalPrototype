using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SkyboxManager : MonoBehaviour
{
    [SerializeField] float maxExposure = 1, minExposure = 0.3f,
        cycleSpeed = 1f,maxLightIntensity =1,minLightIntensity =0.5f,
        lightSpeed =1f;
    [SerializeField] Light[] lightsInScene;

    public UnityEvent OnCycleEnd;
    public UnityEvent OnCycleStart;

    bool isAnimating = false,increaseLight =false;
    void Update()
    {
        UpdateSky();
    }

    private void UpdateSky()
    {
        if (isAnimating)
        {
            bool _hasExposure = false, _hasLight = false;
            if (increaseLight)
            {
                if (RenderSettings.skybox.GetFloat("_Exposure") < maxExposure)
                {
                    RenderSettings.skybox.SetFloat("_Exposure", RenderSettings.skybox.GetFloat("_Exposure") + (cycleSpeed * Time.deltaTime));
                }
                else
                {
                    _hasExposure = true;
                }
                foreach (var light in lightsInScene)
                {
                    if (light.intensity < maxLightIntensity)
                    {
                        light.intensity += lightSpeed * Time.deltaTime;
                    }
                    else
                    {
                        _hasLight = true;
                        break;
                    }
                }
            }
            else//decrease
            {
                if (RenderSettings.skybox.GetFloat("_Exposure") > minExposure)
                {
                    RenderSettings.skybox.SetFloat("_Exposure", RenderSettings.skybox.GetFloat("_Exposure") - (cycleSpeed * Time.deltaTime));
                }
                else
                {
                    _hasExposure = true;
                }
                foreach (var light in lightsInScene)
                {
                    if (light.intensity > minLightIntensity)
                    {
                        light.intensity -= lightSpeed * Time.deltaTime;
                    }
                    else
                    {
                        _hasLight = true;
                        break;
                    }
                }
            }
            if (_hasLight && _hasExposure)
            {
                OnCycleEnd?.Invoke();
                isAnimating = false;
            }
        }
    }

    public void SetDaytime(bool _isDay)
    {
        if (isAnimating)
            return;
        increaseLight = _isDay;
        isAnimating = true;
        OnCycleStart?.Invoke();
    }
}
