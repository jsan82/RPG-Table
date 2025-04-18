using UnityEngine;
using UnityEngine.UI;

public class EditableButton : MonoBehaviour
{
    public InputField editInputField; // Pole do wpisywania tekstu
    private Button button; // Referencja do przycisku
    private Text buttonText; // Tekst przycisku

    void Start()
    {
        // Inicjalizacja komponentów
        button = GetComponent<Button>();
        buttonText = button.GetComponentInChildren<Text>();

        // Ukryj InputField na starcie
        editInputField.gameObject.SetActive(false);

        // Nasłuchuj kliknięcia przycisku
        button.onClick.AddListener(StartEditing);
    }

    public void StartEditing()
    {
        // Pokazuje InputField i ustawia w nim aktualny tekst przycisku
        editInputField.gameObject.SetActive(true);
        editInputField.text = buttonText.text;
    }

    public void SaveText()
    {
        // Zapisuje tekst z InputField do przycisku i chowa InputField
        buttonText.text = editInputField.text;
        editInputField.gameObject.SetActive(false);
    }
}