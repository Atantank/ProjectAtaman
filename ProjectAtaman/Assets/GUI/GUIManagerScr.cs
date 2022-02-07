using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GUILib;

public class GUIManagerScr : MonoBehaviour
{
    public static GUIManagerScr GUIManager;
	[SerializeField] private Text nameText;
	[SerializeField] private Text largeText;
	private GUIData tmpGUIData;

    void Awake()
    {
		GUIManager = this;
    }

    public void Choose(IChosen _chosen)
    {
		tmpGUIData = _chosen.GetDataForGUI();
		nameText.text = tmpGUIData.Name;
		largeText.text = "";
        foreach (string s in tmpGUIData.TextList)
		{
            largeText.text += s + "\n";
        }
	}
}
