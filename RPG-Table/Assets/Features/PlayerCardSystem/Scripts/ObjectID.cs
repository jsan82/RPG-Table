using UnityEngine;
using System.Collections.Generic;

public class ObjectID : MonoBehaviour
{

    public string _id;
    public string _prefabName; // Dodajemy nowe pole do przechowywania oryginalnej nazwy prefaba
    private static Dictionary<string, GameObject> _objectDictionary = new Dictionary<string, GameObject>();

    public void SetID(string newId, GameObject objectRef, string prefabName) // prefabName jest opcjonalny

    {
        if (string.IsNullOrEmpty(newId))
        {
            Debug.LogError("Podano puste ID!");
            return;
        }

        _id = newId;

        _prefabName = !string.IsNullOrEmpty(prefabName) ? prefabName : objectRef.name; // Używamy prefabName jeśli podano, w przeciwnym razie nazwę obiektu
        
        if (!_objectDictionary.ContainsKey(_id))
        {   
            

            _objectDictionary.Add(_id, objectRef);
            Debug.Log($"Dodano nowe ID: {_id} | Prefab: {_prefabName} | Liczba obiektów: {_objectDictionary.Count}");
        }
        else
        {
            Debug.LogWarning($"ID '{_id}' już istnieje! Przypisane do: {_objectDictionary[_id].name}");

            Destroy(objectRef);
        }

        if (!_objectDictionary.ContainsKey(_id))
    {
        _objectDictionary.Add(_id, objectRef);
    }

    }

    public string GetID() => _id;

    public string GetPrefab() => _prefabName; // Zwracamy zachowaną nazwę prefaba

    public static bool IDExists(string idToCheck) => _objectDictionary.ContainsKey(idToCheck);

    public void printDictionary()
    {
        foreach (var kvp in _objectDictionary)
        {

            Debug.Log($"ID: {kvp.Key}, Obiekt: {kvp.Value.GetComponent<ObjectID>().GetPrefab()}");
        }
        

    }

    public static GameObject GetObjectByID(string id)
    {
        if (_objectDictionary.TryGetValue(id, out GameObject obj))
        {
            return obj;
        }
        return null;
    }

    public static Dictionary<string, GameObject> GetAllObjects()
    {

        return _objectDictionary;

    }

    public static void ClearObjectDictionary()
    {

        _objectDictionary.Clear();

        Debug.Log("Wyczyszczono objectDictionary.");
    }

    private void OnDestroy()
    {
        // Przy usuwaniu obiektu, usuwamy jego ID ze słownika
        if (!string.IsNullOrEmpty(_id) && _objectDictionary.ContainsKey(_id))
        {
            _objectDictionary.Remove(_id);
        }
    }
}