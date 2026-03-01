using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DitheMod : MonoBehaviour
{
    public Material material;
    public float lerpDuration = 2f;   // Tiempo del lerp ida y vuelta

    public void TriggerEffect(float targetValue)
    {
        StopAllCoroutines();
        StartCoroutine(EffectRoutine(targetValue));
    }

    private IEnumerator EffectRoutine(float targetValue)
    {
        float originalValue = material.GetFloat("_ColorResolucion");

        // --- LERP DE ORIGINAL → TARGET ---
        float t = 0;
        while (t < lerpDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(originalValue, targetValue, t / lerpDuration);
            material.SetFloat("_ColorResolucion", v);
            yield return null;
        }

        // Opcional: quedarte 1 segundo en el valor
        yield return new WaitForSeconds(1f);

        // --- LERP DE TARGET → ORIGINAL ---
        t = 0;
        while (t < lerpDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(targetValue, originalValue, t / lerpDuration);
            material.SetFloat("_ColorResolucion", v);
            yield return null;
        }

        // Asegurar valor final exacto
        material.SetFloat("_ColorResolucion", originalValue);
    }
}

