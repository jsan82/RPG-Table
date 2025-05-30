using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;

public class SkyboxHandler : MonoBehaviour
{
    public Material skyboxMaterialTemplate;
    public string skyboxTexturePath;

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
        StartCoroutine(LoadSkyboxFromPath(texturePath));
    }


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
