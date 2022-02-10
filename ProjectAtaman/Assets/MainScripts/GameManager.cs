using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GUILib;

public class GameManager : MonoBehaviour
{
    public static GameManager GM { get; private set; }
    public bool IsPaused { get; private set; }
    [SerializeField] private float gameSpeed = 1f;
    public float GameSpeed { get => gameSpeed; }
	private Ray mouseRay;
	private RaycastHit hitMouseRay;

    void Awake()
    {
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
                ISelectable selected;
				if (hitMouseRay.collider.gameObject.TryGetComponent<ISelectable>(out selected))
                {
                    selected.MouseClick();
                    GUIManagerScr.GUIManager.Select(selected);
                }
			}
            else
            {} // ? Нужно ли обрабатывать пустые клики здесь?
        }
    }
}
