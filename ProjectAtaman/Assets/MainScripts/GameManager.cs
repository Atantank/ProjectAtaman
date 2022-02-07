using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GUILib;

public class GameManager : MonoBehaviour
{
    public static GameManager GM { get; private set; }
    public bool IsPaused { get; private set; }
    public float GameSpeed { get; private set; }
	private Ray mouseRay;
	private RaycastHit hitMouseRay;

    void Awake()
    {
		GameSpeed = 1f;
		IsPaused = false;
        GM = this;
    }

    void Update()
    {
		ClickAnalyzer();
    }

	void ClickAnalyzer()
	{
		if (Input.GetKeyDown(KeyCode.Mouse0))
		{
			mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(mouseRay, out hitMouseRay))
			{
                IChosen chosen;
				if (hitMouseRay.collider.gameObject.TryGetComponent<IChosen>(out chosen))
                {
                    chosen.MouseClick();
                    GUIManagerScr.GUIManager.Choose(chosen);
                }
			}
            else
            {} // ? Нужно ли обрабатывать пустые клики здесь?
        }
    }
}
