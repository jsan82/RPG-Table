using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

/// <summary>
/// Handles dynamic loading and application of a skybox texture at runtime.
/// </summary>
public class SkyboxHandler : MonoBehaviour
{
    /// <summary>
    /// Template material used for setting the loaded skybox texture.
    /// </summary>
    public Material skyboxMaterialTemplate;
    public string skyboxTexturePath;

/*    void Start() //comment if not testing
    {
        ChangeSkybox("[P A T H]"); 
    }*/

    /// <summary>
    /// Changes the current skybox using the texture at the specified path.
    /// </summary>
    /// <param name="texturePath">Absolute path to the texture file.</param>
    public void ChangeSkybox(string texturePath)
    {
        skyboxTexturePath = texturePath;
        StartCoroutine(LoadSkyboxFromPath(texturePath));
    }
    public void ClearSkybox()
    {
        RenderSettings.skybox = null;
        DynamicGI.UpdateEnvironment();
    }

    /// <summary>
    /// Coroutine to load a skybox texture from a file path and apply it as the current skybox.
    /// </summary>
    /// <param name="path">Absolute path to the texture file.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    private IEnumerator LoadSkyboxFromPath(string path)
    {
        if (!File.Exists(path))
        {
            yield break;
        }

        string uri = "file:///" + path.Replace("\\", "/");

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(uri);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            yield break;
        }

        Texture texture = DownloadHandlerTexture.GetContent(request);

        Material skyboxMaterial = new Material(skyboxMaterialTemplate);
        skyboxMaterial.SetTexture("_MainTex", texture);

        RenderSettings.skybox = skyboxMaterial;
        DynamicGI.UpdateEnvironment();
    }
}
