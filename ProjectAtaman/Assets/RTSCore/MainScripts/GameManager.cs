using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using MainLib;
using OrderLib;

// TODO Необходимо реализовать единый центр обработки всех движений и вращений (и прочего покадрового или физического)
// TODO Делегаты и подписка всех возможных апдейтов всех объектов
// TODO Необходима разбивка всех подписчиков по категориям, чтобы можно было отключать обработку некоторых групп или показывать дополнительные группы в определенных условиях
public class GameManager : MonoBehaviour
{
	// TODO Перенести в настройки самого ордера? Чтобы здесь не громоздить все настройки?
    [SerializeField] private OrderMarkScr orderMarkPref; 
    [SerializeField] private float gameSpeed = 1f;
    public float GameSpeed { get => gameSpeed; }
    public bool IsPaused { get; private set; }

    public static GameManager GM { get; private set; }
    private Controller controller;

	private Ray mouseRay;
	private RaycastHit hitMouseRay;
	[SerializeField] private ISelectable selectedSelectable;
	[SerializeField] private IInteractable selectedInteractable;
	Quaternion zeroQ = new Quaternion(0, 0, 0, 0);

    void Awake()
    {
		IsPaused = false;
        GM = this;
		controller = new Controller();
        controller.Mouse.LeftClick.started += context => LeftClick();
		controller.Mouse.RightClick.started += context => RightClick();
	}

	void OnEnable()
	{
		controller.Enable();
	}

	void OnDisable()
	{
		controller.Disable();
	}

	void LeftClick()
	{
		mouseRay = Camera.main.ScreenPointToRay(controller.Mouse.MousePosition.ReadValue<Vector2>());
		if (Physics.Raycast(mouseRay, out hitMouseRay))
		{
			GameObject hitObject = hitMouseRay.collider.gameObject;
			switch(hitObject.tag)
			{
				case "Tile":
					print("Tile");
					break;
				case "Order":
					break;
				case "Selectable": // TODO Делать приказы видимыми, после выделения
					print("Selectable");
					if (hitObject.TryGetComponent<ISelectable>(out selectedSelectable))
					{
						selectedSelectable.Select();
						GUIManagerScr.GUIManager.Select(selectedSelectable);
						selectedInteractable = selectedSelectable as IInteractable;
					}
					break;
			}
		}
		else // TODO Продумать момент с кликанием на интерфейс, чтобы не сбрасывался выбор
		{
			selectedSelectable = null;
			GUIManagerScr.GUIManager.UnSelect();
		}
	}

	void RightClick()
	{
		bool shiftPressed = controller.KeyBoard.ShiftPressed.IsPressed();
		print(shiftPressed);
		if(selectedInteractable != null)
		{
			mouseRay = Camera.main.ScreenPointToRay(controller.Mouse.MousePosition.ReadValue<Vector2>());
			if (Physics.Raycast(mouseRay, out hitMouseRay))
			{
				GameObject hitObject = hitMouseRay.collider.gameObject;
				//OrderMarkScr mark = Instantiate(orderMarkPref, controller.Mouse.MousePosition.ReadValue<Vector2>(), zeroQ);
				OrderMarkScr mark = Instantiate(orderMarkPref, mouseRay.GetPoint(10), zeroQ);
				// TODO Нужна проверка на самонаведение, чтобы нельзя было направить юнита на себя же
				switch (hitObject.tag)
				{
					case "Tile":
						print("Move");
						mark.Init(OrderType.Move);
						selectedInteractable.SetNewOrder(new Order(OrderType.Move, mark, shiftPressed));
						break;
					case "Selectable":
						print("AttackEnemy");
						mark.Init(OrderType.AttackEnemy);
						selectedInteractable.SetNewOrder(new Order(OrderType.AttackEnemy, mark, shiftPressed));
						break;
				}
			}
			else // TODO Переделать для случая, когда Тайлы станут видимы для луча, когда на них можно будет хотябы формально кликнуть, так можно получить больше инфомрации и организовать взаимодейтвие между землей и новым объектом
			{
				//Instantiate(orderMarkPref, );
			}
		}
	}
}
