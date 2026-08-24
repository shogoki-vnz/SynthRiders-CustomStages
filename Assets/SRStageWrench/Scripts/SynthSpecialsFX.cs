using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SynthSpecialsFX : MonoBehaviour
{

    public bool isEnabled = true;

    [System.Serializable]
    public struct SpecialFXRenderers
    {
        [SerializeField]
        public Renderer fxRenderer;
        public ParticleSystem particles;
        public Material Material;
        public string colorFieldToReplace;
    }

    [Header("Colors")]
    public SpecialFXRenderers[] specialFX;

    [Header("Events")]
    [SerializeField]
    public UnityEvent SpecialStartFX;
    [SerializeField]
    public UnityEvent SpecialEndFX;
    [SerializeField]
    public UnityEvent SpecialFailFX;


    public void SetColors(Color colorTo)
	{
        if (isEnabled)
		{
            foreach (SpecialFXRenderers FX in specialFX)
            {
                if (FX.fxRenderer)
                {
                    FX.fxRenderer.sharedMaterial.SetColor(Shader.PropertyToID(FX.colorFieldToReplace), colorTo);
                }
                if (FX.particles)
                {
                    ParticleSystem.MainModule mainMod = FX.particles.main;
                    mainMod.startColor = colorTo;
                }
            }
        }
    }

    public void fireStart()
	{
        if (isEnabled) { SpecialStartFX.Invoke(); }
	}

    public void fireEnd()
    {
        if (isEnabled) { SpecialEndFX.Invoke(); }
    }

    public void fireFailed()
	{
        if (isEnabled) { SpecialFailFX.Invoke(); }
    }
}
