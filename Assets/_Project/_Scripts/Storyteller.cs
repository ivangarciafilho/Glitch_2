using LLMUnity;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using TMPro;

using UnityEngine;

public class Storyteller : MonoBehaviour
{
    public TextMeshProUGUI textDisplay;
    public LLMCharacter character;

    public List<string> history_Questions;
    public List<string> history_Answers;

    public List<string> themes;
    public Vector2 delayBetweenStories;
    public float currentTimeToRead;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    => Warmup();

    private async void Warmup()
    {
        foreach (var item in history_Questions)
            character.AddPlayerMessage(item);

        foreach (var item in history_Answers)
            character.AddAIMessage(item);

        await character.Warmup();
        await character.Save("Sage");
        StartCoroutine(Tell());
    }

    private IEnumerator Tell() 
    {
        while (true)
        {
            var theme = themes[Random.Range(0, themes.Count-1)];
            var question = "Tell me anything curious i should know about :" + theme;

            currentTimeToRead = Time.time + 2f ;
            var answer = character.Chat(question, UpdateDiplay);

            while (Time.time < currentTimeToRead )
                yield return null;

            yield return new WaitForSeconds(Random.Range (delayBetweenStories.x, delayBetweenStories.y));
        }
    }



    private void UpdateDiplay( string text)
    {
        textDisplay.text = text;

        var wordCount = text.Length / 5;
        var wordsPerMinute = 200 / 60;
        var delayToStartReading = 1.5f;
        var deviation = 1.1f;
        var currentTime = Time.time;
         currentTimeToRead = ((wordCount / wordsPerMinute) + delayToStartReading) * deviation;
        currentTimeToRead= currentTime + Mathf.Clamp(currentTimeToRead, 2f, float.MaxValue);
    }

}
