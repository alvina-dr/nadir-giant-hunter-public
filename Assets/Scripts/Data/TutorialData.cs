using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "TutorielData", menuName = "ScriptableObjects/TutorielData", order = 1)]
public class TutorialData : ScriptableObject
{
    [System.Serializable]
    public class TutorielEntry
    {
        public string TitleKey;
        public List<TutorielText> Content = new List<TutorielText>();

        public TutorielEntry(string titleKey, List<TutorielText> content)
        {
            TitleKey = titleKey;
            Content = content;
        }
    }


    [System.Serializable]
    public class TutorielText
    {
        public InputActionReference Input; //written before sentence if input needed
        [TextArea]
        public string SimpleTextKey;

        public TutorielText(InputActionReference input, string simpleTextKey)
        {
            Input = input;
            SimpleTextKey = simpleTextKey;
        }
    }

    public List<TutorielEntry> entries;
}