using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GUILib;

public class GUIManagerScr : MonoBehaviour
{
    public static GUIManagerScr GUIManager;
	[SerializeField] Text nameText;
	[SerializeField] Text storyText;
	private GUIData tmpGUIData;

    void Awake()
    {
		GUIManager = this;
    }

    public void Choose(IChosen _chosen)
    {
		tmpGUIData = _chosen.GetGUIData();
		nameText.text = tmpGUIData.Name;
		storyText.text = "";
        foreach (string s in tmpGUIData.TextList)
		{
            storyText.text += s;
        }
	}
}
