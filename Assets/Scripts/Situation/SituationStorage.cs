using ISBEP.Utility;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace ISBEP.Situation
{
    public class SituationStorage : MonoBehaviour
    {
        [Tooltip("Specify whether debug message for the situation should be displayed in the logs.")]
        public bool DebugMessages = false;
        private static readonly string CONTEXT = "Situation";
        [Tooltip("File name to export the situation under")]
        public string FileName = "test";
        [Tooltip("Folder path to store the situation in")]
        public string FolderPath = "";

        private string FilePath;

        public void SaveSituation(Dictionary<string, List<ISituationData>> situationData)
        {
            Util.DebugLog(CONTEXT, "Saving situation");
            DefiningFilepath();

            List<string> elementsData= new List<string>();
            foreach (KeyValuePair<string, List<ISituationData>> situationElementType in situationData)
            {
                string jsonElements = string.Join(",", situationElementType.Value.Select(
                        (situationElement) => { return situationElement.JsonData; }
                    ));
                elementsData.Add($"\"{situationElementType.Key}\":[{jsonElements}]");
            }
            SaveData($"{{{string.Join(",", elementsData)}}}");
        }

        private void SaveData(string export)
        {
            using StreamWriter file = new StreamWriter(FilePath, false);
            file.Write(export);
            Util.Log(CONTEXT, "Saved situation data");
        }

        /// <summary> Define the file path for the export file. </summary>
        private void DefiningFilepath()
        {
            string fileExtension = ".json";
            FilePath = Path.Join(FolderPath, string.Concat(FilterIllegalFileChars(FileName), fileExtension));
            Util.DebugLog(CONTEXT, $"Defined file path as {FilePath}");
        }

        /// <summary>
        /// Filters the specified string from illigal file name characters.
        /// </summary>
        /// <param name="unfilteredString">String to filter file name characters in.</param>
        /// <returns>Specified string with illigal file name characters replaced.</returns>
        public static string FilterIllegalFileChars(string unfilteredString)
        {
            return string.Join("", unfilteredString.Split(Path.GetInvalidFileNameChars()));
        }
    }
}
