using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class LensFlareTweener : MonoBehaviour
{
    //public static float time = 2f;
    public float speedCoeff = 1f;
    public Vector2 noiseRange;
    private float noiseCoeff;
    //public static float waitTime = 0.5f;
    //private bool hasWaited = false;
    //private bool isLerping = false;
    public float targetIntensity = 2;
    [SerializeField] private LensFlareComponentSRP flare;
    //private float currentTime = 0;

    public void OnEnable()
    {
        if(flare == null)
            flare = GetComponent<LensFlareComponentSRP>();
        flare.intensity = targetIntensity;

        noiseCoeff = Random.Range(noiseRange.x, noiseRange.y);
    }

    public void Update()
    {
        flare.intensity = Mathf.Sin(
            (Time.time + noiseCoeff) * speedCoeff) * targetIntensity;

        // currentTime -= Time.deltaTime;

        // if(currentTime <= 0)
        // {
        //     if(!hasWaited && flare.intensity >= targetIntensity)
        //     {
        //         isLerping = false;
        //         hasWaited = true;
        //         currentTime = waitTime;
        //     }
        //     if(hasWaited && flare.intensity >= targetIntensity)
        //     {
        //         currentTime = time;
        //         hasWaited = false;
        //         isLerping = true;
        //     }
        // }

        // if(isLerping)
        // {
        //     Mathf.Sin
        // }
    }
}
