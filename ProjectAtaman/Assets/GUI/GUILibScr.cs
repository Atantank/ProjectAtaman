using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GUILib
{
	public interface ISelectable
	{
		public void MouseClick();
		public GUIData GetDataForGUI();
	}
    
    public class GUIData
    {
        public string Name;
        public List<string> TextList;

        public GUIData(string _name, List<string> _textList)
        {
            Name = _name;
            TextList = _textList;
        }
    }
}