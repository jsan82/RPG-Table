using UnityEngine;
using System.Collections.Generic;

public class ObjectID : MonoBehaviour
{
    public string _id; //ID of the object
    public string _prefabName; //Prefab name of the object
    private static Dictionary<string, GameObject> _objectDictionary = new Dictionary<string, GameObject>(); //Dictionary to store objects by ID

    //Method to set the ID and add the object to the dictionary
    public void SetID(string newId, GameObject objectRef, string prefabName = null) 
    {
        if (string.IsNullOrEmpty(newId))
        {
            Debug.LogError("Emptu ID!");
            return;
        }

        _id = newId;
        _prefabName = prefabName ?? objectRef.name; 

        _prefabName = prefabName ?? objectRef.name; 


        //Check if the ID already exists in the dictionary
        if (!_objectDictionary.ContainsKey(_id))
        {   
           
            _objectDictionary.Add(_id, objectRef);
            
        }
        else
        {
            Debug.LogWarning($"ID '{_id}' already exists.");
            Destroy(objectRef);
        }

        //Check if the object reference is already in the dictionary
        if (!_objectDictionary.ContainsKey(_id))
        {
            _objectDictionary.Add(_id, objectRef);
        }

        if (!_objectDictionary.ContainsKey(_id))
    {
        _objectDictionary.Add(_id, objectRef);
    }


    }

    public string GetID() => _id;

    public string GetPrefab() => _prefabName;

    public static bool IDExists(string idToCheck) => _objectDictionary.ContainsKey(idToCheck);


    public static void printDictionary()
    {
        foreach (var kvp in _objectDictionary)
        {
            Debug.Log($"ID: {kvp.Key}");
            Debug.Log($"Prefab of ^:{kvp.Value.GetComponent<ObjectID>()._prefabName}");
            
        }
        
    }

    //Method to get the object by ID
    public static GameObject GetObjectByID(string id)
    {
        if (_objectDictionary.TryGetValue(id, out GameObject obj))
        {
            return obj;
        }
        return null;
    }

    //Method to get all objects in the dictionary
    public static Dictionary<string, GameObject> GetAllObjects()
    {
        return _objectDictionary;
    }

    public static void ClearObjectDictionary()
    {
        _objectDictionary.Clear();
        Debug.Log("Cleared Dictionary.");

    }

    //Method to delete an object from the dictionary by ID
    private void OnDestroy()
    {
        if (!string.IsNullOrEmpty(_id) && _objectDictionary.ContainsKey(_id))
        {
            _objectDictionary.Remove(_id);
        }
    }
}