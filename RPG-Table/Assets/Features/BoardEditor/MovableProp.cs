using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

public class MovableProp : MonoBehaviour
{
    private Material bloomMaterial;
    private Color emissionColor;
    private float intensity;
    private bool bloomEnabled;

    private Light pointLight;

    // Start is called before the first frame update
    void Start()
    {
        emissionColor = Color.white;
        intensity = 0.0f;
        bloomEnabled = false;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            bloomMaterial = renderer.material;
        }

        pointLight = GetComponentInChildren<Light>();
        if (pointLight == null)
        {
            GameObject lightObj = new GameObject("BloomLight");
            lightObj.transform.SetParent(transform);
            lightObj.transform.localPosition = Vector3.zero;

            pointLight = lightObj.AddComponent<Light>();
            pointLight.type = LightType.Point;
            pointLight.range = 5f;
            pointLight.intensity = 0f;
            pointLight.color = emissionColor;
            pointLight.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnDrag(Vector3 newPos) { transform.position = newPos; }
    public void OnRotate(Vector3 axis, float angle) { transform.Rotate(axis, angle, Space.World); }
    public void OnScale(Vector3 newScale) { transform.localScale = newScale; }
    public Vector3 GetPosition() { return transform.position; }
    public Vector3 GetScale() { return transform.localScale; }
    public float GetIntensity() { return intensity; }

    public void ToggleBloom()
    {
        if (bloomMaterial == null) return;

        bloomEnabled = !bloomEnabled;

        if (bloomEnabled)
        {
            bloomEnabled = true;
            bloomMaterial.EnableKeyword("_EMISSION");
            bloomMaterial.SetColor("_EmissionColor", emissionColor * intensity);

            pointLight.enabled = true;
            pointLight.intensity = intensity;
        }
        else
        {
            bloomEnabled = false;
            bloomMaterial.DisableKeyword("_EMISSION");

            pointLight.enabled = false;
        }
    }

    public void SetEmissionColor(Color color)
    {
        emissionColor = color;
        if (bloomMaterial != null && bloomMaterial.IsKeywordEnabled("_EMISSION"))
        {
            bloomMaterial.SetColor("_EmissionColor", emissionColor * intensity);
        }

        if (pointLight != null)
        {
            pointLight.color = emissionColor;
        }
    }

    public void SetIntensity(float power)
    {
        intensity = Mathf.Max(0, intensity+power);
        if (bloomMaterial != null && bloomMaterial.IsKeywordEnabled("_EMISSION"))
        {
            bloomMaterial.SetColor("_EmissionColor", emissionColor * intensity);
        }

        if (pointLight != null && pointLight.enabled)
        {
            pointLight.intensity = intensity;
        }
    }
}