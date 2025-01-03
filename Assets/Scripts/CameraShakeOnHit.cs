using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShakeOnHit : MonoBehaviour
{
    public static CameraShakeOnHit Instance { get; private set; }

    public CinemachineVirtualCamera impactCamera;
    private float shakeTimer;
    private float startingIntensity;
    private float shakeTimerTotal;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        impactCamera = GetComponent<CinemachineVirtualCamera>();
    }


    public void TriggerImpact(float shakeIntensity, float time)
    {
         
        CinemachineBasicMultiChannelPerlin perlin = impactCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        perlin.m_AmplitudeGain = shakeIntensity;
        startingIntensity = shakeIntensity;
        shakeTimer = time;
        shakeTimerTotal = time;
    }

    public void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin perlin = impactCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
                perlin.m_FrequencyGain = 2f;
                perlin.m_AmplitudeGain = Mathf.Lerp(startingIntensity, 0f,  1 - (shakeTimer / shakeTimerTotal));
            }
        }
    }


}
