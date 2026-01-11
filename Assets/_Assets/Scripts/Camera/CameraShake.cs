using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public IEnumerator Shake(float duration = 0.1f, float magnitude = 0.05f)
    {
        Vector3 originalPos = transform.localPosition;
        float time = 0f;

        while (time < duration)
        {
            float shakeX = Mathf.Sin(time * 60f) * magnitude;   // dao động qua lại nhanh
            float shakeY = Mathf.Cos(time * 45f) * magnitude * 0.5f; // nhẹ hơn trục Y

            transform.localPosition = originalPos + new Vector3(shakeX, shakeY, 0);

            time += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }

}
