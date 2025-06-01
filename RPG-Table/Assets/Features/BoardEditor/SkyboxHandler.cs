using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class SkyboxHandler : MonoBehaviour
{
    public Material skyboxMaterialTemplate;
<<<<<<< HEAD
    public string skyboxTexturePath;
=======
>>>>>>> e91458933ac7029391988ba4b9ffac29c4b2ced8

    // Start is called before the first frame update
    void Start()
    {
        //ChangeSkybox("[P A T H]"); //comment if not testing
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeSkybox(string texturePath)
    {
<<<<<<< HEAD
        skyboxTexturePath = texturePath;
        StartCoroutine(LoadSkyboxFromPath(texturePath));
    }
    public void ClearSkybox()
    {
        RenderSettings.skybox = null;
        DynamicGI.UpdateEnvironment();
    }
=======
        StartCoroutine(LoadSkyboxFromPath(texturePath));
    }
>>>>>>> e91458933ac7029391988ba4b9ffac29c4b2ced8


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
