using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;

/*
*   Class to handle all of the game's logic
*   - Starts / Resets the game
*   - Connects words to buttons
*   - Handles correct / incorrect attempts
*   - Changes colors of buttons depending on action done
*/

public class GameManager : MonoBehaviour
{
    private int CorrectSets = 0;

    [Header("Button References")]
    public List<Button> buttons;
    public List<string> currentSelection; // List to track which words are currently selected

    [Header("Script References")]
    [SerializeField] private AttemptManager attemptManager;
    [SerializeField] private ButtonLocationManager buttonLocationManager;
    [SerializeField] private WordManager wordManager;

    public void SetupGame()
    {
        List<string> wordSet = wordManager.Get16UniqueWords(); // Get 16 unique words from WordManager
        wordManager.AssignWordsToButtons(buttons, wordSet); // Assign words to buttons
        
        buttonLocationManager.ShuffleAndRepositionButtons(); // Shuffle button locations on start
        
        SetupButtons(); // Setup button click listeners
    }

    public void ResetGame()
    {
        attemptManager.Start(); // Reset attempts to 0

        ResetSelection(); // Clear current selection


        // Re-enable all buttons and reset their colors
        foreach (Button button in buttons)
        {
            button.interactable = true; 
            button.GetComponent<Image>().color = Color.white;
        }

        buttonLocationManager.ResetUnsolvedList();

        CorrectSets = 0; // Reset sets completed to 0

        SetupGame(); // Setup the new game
    }

    
    void SetupButtons()
    {
        foreach (Button button in buttons)
        {
            TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>();
            string word = tmp.text; // Get the word from the TextMeshPro component

            button.onClick.RemoveAllListeners(); // Clear old listeners
            button.onClick.AddListener(() => OnButtonClicked(word, button)); // Add click logic
        }
    }

    void OnButtonClicked(string word, Button button)
    {
        if (currentSelection.Contains(word))
        {
            // Deselect the word
            currentSelection.Remove(word);
            button.GetComponent<Image>().color = Color.white;
        }
        else
        {
            if (currentSelection.Count < 4)
            {
                // Select the word
                currentSelection.Add(word);
                button.GetComponent<Image>().color = Color.grey;
            }
        }
    }

    public void CheckSelection()
    {
        if (currentSelection.Count != 4)
        {
            return;
        }

        // Check if the current selection matches any word set in WordManager
        bool isMatch = false;
        foreach (var wordSet in wordManager.allWordSets)
        {
            if (IsMatchingSet(currentSelection, wordSet.words))
            {
                isMatch = true;
                break;
            }
        }

        if (isMatch) // Set matches
        {
            List<Button> solvedButtons = new List<Button>();
            foreach (string word in currentSelection)
            {
                Button button = buttons.Find(b =>
                {
                    TextMeshProUGUI tmp = b.GetComponentInChildren<TextMeshProUGUI>();
                    return tmp != null && tmp.text == word;
                });

                if (button != null)
                {
                    solvedButtons.Add(button);
                }
            }

            // Mark group as solved
            MarkGroupAsSolved();
            CorrectSets++;
            
            // Pass solved buttons to ButtonLocationManager
            buttonLocationManager.MarkButtonsAsSolved(solvedButtons);
        }
        else // Set doesn't match
        {
            attemptManager.IncreaseAttempt();
            StartCoroutine(IncorrectSelection());
        }
    }

    bool IsMatchingSet(List<string> selection, List<string> wordSet)
    {
        return new HashSet<string>(selection).SetEquals(wordSet); // Compare sets (order doesn't matter)
    }

    void MarkGroupAsSolved()
    {
        foreach (string word in currentSelection)
        {
            // Find the button corresponding to this word
            Button button = buttons.Find(b =>
            {
                TextMeshProUGUI tmp = b.GetComponentInChildren<TextMeshProUGUI>();
                return tmp != null && tmp.text == word;
            });

            if (button != null)
            {
                button.interactable = false;    // Disable button

                switch (CorrectSets)
                {
                    case 0:
                        button.GetComponent<Image>().color = new Color(1f, 0f, 1f, 1f);
                        break;
                    case 1:
                        button.GetComponent<Image>().color = new Color(0f, 0f, 1f, 1f);
                        break;
                    case 2:
                        button.GetComponent<Image>().color = new Color(1f, 47f / 51f, 0.015686275f, 1f);
                        break;
                    case 3:
                        button.GetComponent<Image>().color = new Color(1f, 0f, 0f, 1f);
                        break;
                }
            }
        }
        currentSelection.Clear();
    }

    IEnumerator IncorrectSelection()
    {
        foreach (string word in currentSelection)
        {
            Button button = buttons.Find(b =>
            {
                TextMeshProUGUI tmp = b.GetComponentInChildren<TextMeshProUGUI>();
                return tmp != null && tmp.text == word;
            });

            if (button != null)
            {
                button.GetComponent<Image>().color = Color.black; // Show as incorrect
            }
        }
        yield return new WaitForSeconds(0.5f);
        ResetSelection();
    }

    public void ResetSelection()
    {
        foreach (string word in currentSelection)
        {
            Button button = buttons.Find(b => 
            {
                TextMeshProUGUI tmp = b.GetComponentInChildren<TextMeshProUGUI>();
                return tmp != null && tmp.text == word;
            });
            if (button != null)
            {
                button.GetComponent<Image>().color = Color.white; // Reset button color
            }
        }
        currentSelection.Clear();
    }

    public void QuitToDesktop()
    {
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
