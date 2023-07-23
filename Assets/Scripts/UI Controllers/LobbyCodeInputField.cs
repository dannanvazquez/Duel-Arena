using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_InputField))]
public class LobbyCodeInputField : MonoBehaviour {
    private TMP_InputField inputField;

    private void Awake() {
        inputField = GetComponent<TMP_InputField>();
        inputField.onValidateInput += delegate (string input, int charIndex, char addedChar) { return ValidateChar(addedChar); };
    }

    public char ValidateChar(char c) {
        if (char.IsLetterOrDigit(c) && inputField.text.Length < 6) {
            return char.ToUpper(c);
        } else {
            return '\0';
        }
    }
}
