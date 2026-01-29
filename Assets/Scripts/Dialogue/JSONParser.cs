using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Newtonsoft.Json;

public class JSONParser : MonoBehaviour
{
    private Dictionary<string, Dictionary<string, Dictionary<string, string>>> dataBank = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

    void Start()
    {
        LoadFromJson();
    }

    private void LoadFromJson()
    {
        TextAsset[] files = Resources.LoadAll<TextAsset>("Dialogue");
        foreach (TextAsset file in files)
        {
            var dict = ParseJsonToDictionary(file.text);
            foreach (var outerKey in dict.Keys)
            {
                var innerDict = dict[outerKey];
                var keysToUpdate = new List<string>(innerDict.Keys);
                foreach (var innerKey in keysToUpdate)
                {
                    //Replace '--' with an en dash
                    if (innerDict[innerKey] != null && innerDict[innerKey].Contains("--"))
                    {
                        innerDict[innerKey] = innerDict[innerKey].Replace("--", "—");
                    }
                }
            }
            dataBank[file.name] = dict;
        }
    }

    private Dictionary<string, Dictionary<string, string>> ParseJsonToDictionary(string jsonString)
    {
        try
        {
            var parsedData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jsonString);
            return parsedData;
        }
        catch (JsonException ex)
        {
            Debug.LogError("Error parsing JSON: " + ex.Message);
            return null;
        }
    }

    public string[] FindByID(string sheetName, string ID, int variation)
    {
        if (dataBank.ContainsKey(sheetName) && dataBank[sheetName].ContainsKey(ID))
        {
            //Iterate through the values under this ID until we find the requested variation, or else the greatest number smaller than it
            int greatestInt = -1;
            foreach (var value in dataBank[sheetName][ID].Values)
            {
                if (int.TryParse(value, out int parsedInt))
                {
                    if (parsedInt <= variation && parsedInt > greatestInt)
                    {
                        greatestInt = parsedInt;
                    }
                }
            }

            //Add the lines for the chosen variation
            List<string> lines = new List<string>();
            bool correctPart = false;
            foreach (var kvp in dataBank[sheetName][ID])
            {
                if (kvp.Value == "" && correctPart)
                    break;

                if (correctPart)
                    lines.Add(kvp.Value);

                if (kvp.Value == "" + greatestInt)
                    correctPart = true;
            }
            return lines.ToArray();
        }
        else
        {
            Debug.LogWarning($"No data found for sheet {sheetName} and variation {variation}");
            return null;
        }
    }
}