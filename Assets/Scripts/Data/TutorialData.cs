using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.InputSystem;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "TutorielData", menuName = "ScriptableObjects/TutorielData", order = 1)]
public class TutorialData : ScriptableObject
{
    [System.Serializable]
    public class TutorielEntry
    {
        public string TitleKey;
        public List<TutorielText> Content = new List<TutorielText>();
        public VideoClip Video;

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
        public Sprite Sprite;

        public TutorielText(InputActionReference input, string simpleTextKey, Sprite sprite)
        {
            Input = input;
            SimpleTextKey = simpleTextKey;
            Sprite = sprite;
        }
    }

    public List<TutorielEntry> entries;
}
