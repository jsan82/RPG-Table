using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.IO;

/// <summary>
/// A MonoBehaviour class that handles dice throw operations when a button is clicked.
/// </summary>
public class ButtonThrow : MonoBehaviour
{
    public string expression = "2d6";

    /// <summary>
    /// Handles the button click event, evaluates the associated dice expression, and logs the result.
    /// </summary>    
    public void makeThrow()
    {
        GameObject clickedButton = EventSystem.current.currentSelectedGameObject;

        ObjectID objectID = clickedButton.GetComponent<ObjectID>();
        if (objectID != null && objectID.GetPrefab() == "Button")
        {
            Debug.Log($"Clicked on button with ID: {objectID.GetID()}");
        }
        else
        {
            Debug.Log("Clicked on a non-button object or no ObjectID component found.");
        }

        expression = NewBehaviourScript.Instance.GetOperationsForObject(objectID.GetID());
        Debug.Log($"Expression: {expression}");
        int result = DiceExpressionEvaluator.Instance.EvaluateAndLog(expression);
        Debug.Log($"Result: {result}");

    }
}
