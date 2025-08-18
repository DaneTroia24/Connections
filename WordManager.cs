using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;


// A Serialized Class to add wordsets (groups of 4 words that relate)
[System.Serializable]
public class WordSet
{
    public List<string> words;
}

/*
*   This class selects the words on START or RESET and 
*   Applies them to the text for each button.
*   - Ensures no duplicates
*/
public class WordManager : MonoBehaviour
{
    [Header("Word Pools")]
    public List<WordSet> allWordSets; // List of wordset objects
    private List<int> usedIndices = new List<int>(); // To track used wordsets
    private List<WordSet> selectedWordSets = new List<WordSet>(); // To store the selected word sets

    // Selects 4 wordsets (no duplicates)
    public List<string> Get16UniqueWords()
    {
        if (allWordSets.Count < 4)
        {
            return new List<string>();
        }

        // Clear old lists
        selectedWordSets.Clear();
        usedIndices.Clear();

        // Select 4 unique word sets
        for (int i = 0; i < 4; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, allWordSets.Count);
            } while (usedIndices.Contains(randomIndex)); // Ensure the index hasn't been used

            // Add the selected word set to the list and mark it as used
            selectedWordSets.Add(allWordSets[randomIndex]);
            usedIndices.Add(randomIndex);
        }

        // Combine the words from the 4 selected word sets into one list
        List<string> words = new List<string>();
        foreach (WordSet wordSet in selectedWordSets)
        {
            words.AddRange(wordSet.words); // Add all 4 words from each set
        }

        return words;
    }

    // Assigns the 16 unique words to the buttons
    public void AssignWordsToButtons(List<Button> buttons, List<string> words)
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            // Find the text component under the button
            TextMeshProUGUI tmp = buttons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null)
            {
                tmp.text = words[i];
            }
        }
    }
}
