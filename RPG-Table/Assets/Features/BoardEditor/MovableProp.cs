using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Globalization;

/// <summary>
/// A component for handling a movable and interactive prop in the scene,
/// supporting dragging, rotating, scaling, and bloom/emission lighting control.
/// </summary>
public class MovableProp : MonoBehaviour
{
    /// <summary>Material used for emission and bloom effects.</summary>
    private Material bloomMaterial;
    /// <summary>Base emission color.</summary>
    private Color emissionColor;
    /// <summary>Intensity multiplier for emission and light.</summary>
    private float intensity;
    /// <summary>Flag indicating whether bloom is currently enabled.</summary>
    private bool bloomEnabled;
    /// <summary>Associated point light used to simulate bloom lighting.</summary>
    private Light pointLight;

    /// <summary>
    /// Initializes the component, including emission setup and optional creation of a child light source.
    /// </summary>
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

    /// <summary>
    /// Moves the object to a new position.
    /// </summary>
    /// <param name="newPos">The target world position.</param>
    public void OnDrag(Vector3 newPos) { transform.position = newPos; }

    /// <summary>
    /// Rotates the object around a given axis.
    /// </summary>
    /// <param name="axis">The axis to rotate around.</param>
    /// <param name="angle">The angle in degrees to rotate.</param>
    public void OnRotate(Vector3 axis, float angle) { transform.Rotate(axis, angle, Space.World); }

    /// <summary>
    /// Sets the local scale of the object.
    /// </summary>
    /// <param name="newScale">New local scale vector.</param>
    public void OnScale(Vector3 newScale) { transform.localScale = newScale; }

    /// <summary>
    /// Gets the current world position of the object.
    /// </summary>
    /// <returns>World position of the transform.</returns>
    public Vector3 GetPosition() { return transform.position; }

    /// <summary>
    /// Gets the current local scale of the object.
    /// </summary>
    /// <returns>Local scale of the transform.</returns>
    public Vector3 GetScale() { return transform.localScale; }

    /// <summary>
    /// Gets the current emission/light intensity.
    /// </summary>
    /// <returns>Current intensity value.</returns>
    public float GetIntensity() { return intensity; }

    /// <summary>
    /// Toggles the bloom/emission effect on or off.
    /// Updates material emission and light state accordingly.
    /// </summary>
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

    /// <summary>
    /// Sets the emission color for both the material and the point light.
    /// </summary>
    /// <param name="color">New emission color.</param>
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

    /// <summary>
    /// Adjusts the emission/light intensity by a delta value.
    /// </summary>
    /// <param name="power">Amount to add to current intensity (clamped to non-negative).</param>
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