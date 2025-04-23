using UnityEngine;
using System.Collections.Generic;

public class ObjectID : MonoBehaviour
{
    [SerializeField] private string _id; // Pole prywatne z atrybutem SerializeField
    private static List<string> _idList = new List<string>(); // Wspólna lista dla wszystkich instancji
    
    public void SetID(string newId)
    {
        if (string.IsNullOrEmpty(newId))
        {
            Debug.LogError("Podano puste ID!");
            return;
        }

        if (!_idList.Contains(newId))
        {
            _id = newId;
            _idList.Add(_id);
            Debug.Log($"Dodano nowe ID: {_id} | Liczba ID: {_idList.Count}");
        }
        else
        {
            Debug.LogWarning($"ID '{newId}' już istnieje!");
            // Możesz tutaj wygenerować unikalne ID:
            // _id = $"{newId}_{System.Guid.NewGuid().ToString("N").Substring(0, 4)}";
        }
    }
    
    public string GetID() => _id; // Skrócona składnia

    public static bool IDExists(string idToCheck) => _idList.Contains(idToCheck);
}