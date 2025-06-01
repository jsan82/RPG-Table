using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Unique identifier component for GameObjects with dictionary management
/// </summary>
/// <remarks>
/// Tracks objects in a central dictionary using unique IDs.
/// Provides static methods for object lookup and management.
/// Integrates with CardAreaSaver for dictionary storage.
/// </remarks>
public class ObjectID : MonoBehaviour
{
    /// <summary>The unique identifier for this object</summary>
    public string _id;

    /// <summary>The prefab name this object was created from</summary>
    public string _prefabName; 


    /// <summary>
    /// Sets the object's ID and registers it in the global dictionary
    /// </summary>
    /// <param name="newId">Unique identifier to assign</param>
    /// <param name="objectRef">Reference to the GameObject</param>
    /// <param name="prefabName">Optional prefab name (uses object name if null)</param>
    /// <remarks>
    /// Will destroy the object if ID already exists in the dictionary
    /// </remarks>
    public void SetID(string newId, GameObject objectRef, string prefabName = null)
    {
        if (string.IsNullOrEmpty(newId))
        {
            Debug.LogError("Emptu ID!");
            return;
        }

        _id = newId;
        _prefabName = prefabName ?? objectRef.name;


        if (!CardAreaSaver._objectDictionary.ContainsKey(_id))
        {
            CardAreaSaver._objectDictionary.Add(_id, objectRef);
        }
        else
        {
            Debug.LogWarning($"ID '{_id}' already exists. Destroying the object.");
            Destroy(objectRef);
        }

        //Check if the object reference is already in the dictionary
        if (!CardAreaSaver._objectDictionary.ContainsKey(_id))
        {
            CardAreaSaver._objectDictionary.Add(_id, objectRef);
        }

    }


    /// <summary>Gets the object's unique ID</summary>
    public string GetID() => _id;

    /// <summary>Gets the object's prefab name</summary>
    public string GetPrefab() => _prefabName;

    /// <summary>
    /// Checks if an ID exists in the global dictionary
    /// </summary>
    /// <param name="idToCheck">ID to verify</param>
    public static bool IDExists(string idToCheck) => CardAreaSaver._objectDictionary.ContainsKey(idToCheck);

    /// <summary>
    /// Debug utility to print all objects in dictionary
    /// </summary>
    public static void printDictionary()
    {
        foreach (var kvp in CardAreaSaver._objectDictionary)
        {
            Debug.Log($"ID: {kvp.Key}");
            Debug.Log($"Prefab of ^:{kvp.Value.GetComponent<ObjectID>()._prefabName}");

        }

    }

    /// <summary>
    /// Retrieves a GameObject by its ID
    /// </summary>
    /// <param name="id">ID to look up</param>
    /// <returns>GameObject if found, null otherwise</returns>
    public static GameObject GetObjectByID(string id)
    {
        if (CardAreaSaver._objectDictionary.TryGetValue(id, out GameObject obj))
        {
            return obj;
        }
        return null;
    }


    /// <summary>
    /// Gets all objects in the global dictionary
    /// </summary>
    /// <returns>Dictionary of all registered objects</returns>
    public static Dictionary<string, GameObject> GetAllObjects()
    {
        return CardAreaSaver._objectDictionary;
    }


    /// <summary>
    /// Removes an object from the dictionary by ID
    /// </summary>
    /// <param name="id">ID of object to remove</param>
    /// <remarks>
    /// Also destroys the GameObject instance
    /// </remarks>
    public static void RemoveObjectByID(string id)
    {
        if (CardAreaSaver._objectDictionary.ContainsKey(id))
        {
            Destroy(CardAreaSaver._objectDictionary[id]);
            CardAreaSaver._objectDictionary.Remove(id);
            Debug.Log($"Removed object with ID: {id}");
        }
        else
        {
            Debug.LogWarning($"No object found with ID: {id}");
        }
    }
    public static void Clear_objectDictionary()
    {
        //CardAreaSaver._objectDictionary = new Dictionary<string, GameObject>();
        Debug.Log("Cleared Dictionary.");

    }

    //Method to delete an object from the dictionary by ID
    // private void OnDestroy()
    // {
    //     if (!string.IsNullOrEmpty(_id) && CardAreaSaver._objectDictionary.ContainsKey(_id))
    //     {
    //         CardAreaSaver._objectDictionary.Remove(_id);
    //     }
    // }
}