using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HumanLib;
using GUILib;
using OrderLib;

// ! Необходимо добавить интерфейс для паттерна Компоновщик при создании подразделений
public class WarriorScr : MonoBehaviour, IChosen, IOrder
{
	// * Составные части и состояния ///////////////////////////////////////////////////////////////////////////////////////
	[SerializeField] private BodyState body;
	[SerializeField] private MindState mind;
	[SerializeField] private ActionState action;
	// TODO Добавить Дух

	// * Внешние условия ///////////////////////////////////////////////////////////////////////////////////////////////////
	[SerializeField] private Order order; // Текущий приказ
	// TODO Добавить класс условий внешней среды (осадки, температура)
	// TODO Добавить класс поверхности под ногами, чтобы соединить с условиями и повлиять на действия воина

	// * Видимые параметры воина ///////////////////////////////////////////////////////////////////////////////////////////
	public bool IsAlive { get => body.Condition != BodyCondition.dead; }
	public ActionStatus Status { get => action.Status; } // TODO У статусов должно быть поле с информацией, как эта деятельность выглядит со стороны) Если думает, то со стороны должно казаться, что просто стоит
	[SerializeField] private int team;
    public int Team { get => team; }
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Color spriteColor;
    [SerializeField] private int viewDirection;
	//public int ViewDirection { get => (int)transform.localEulerAngles.z; }

	// * Вспомогательные переменные ////////////////////////////////////////////////////////////////////////////////////////
	[SerializeField] private List<string> hisStory = new List<string>();

    void Awake()
	{
		spriteRenderer.color = spriteColor;
		transform.localEulerAngles = new Vector3(0, 0, viewDirection);
		body = new GoodBody();
		body.SetOwner(this);
		mind = new OrdinaryMind();
		mind.SetOwner(this);
		action = new Idle();
		action.SetOwner(this);
	}
	
	void Start()
    {
		SetOrder(new Order(OrderType.AttackEnemy, null));
    }

	/*public void Init(string _name, int _team, int _viewDirection, Sprite _sprite, Color _color) // TODO Добавить перегрузку без повторений с возможностью задавать необязательные параметры
    {
		this.name = _name;
		team = _team;
		viewDirection = _viewDirection;
		spriteRenderer.sprite = _sprite;
		spriteRenderer.color = _color;
	}*/

	// * Обработка взаимодействий объекта /////////////////////////////////////////////////////////////////////////////////////
	void OnTriggerEnter(Collider _collider)
    {}

	public void MouseClick() // TODO Добавить как минимум индивидуальный звуковой отклик при выборе юнита
	{}

	// * Действия и реакции //////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetOrder(Order _order) // TODO Перенести в класс приказа
	{
		order = _order;
		switch (order.Type)
		{
			case OrderType.AttackEnemy: // ? Связать с образом мысли / характером в Mind: Hunting, AttackingEnemy 
				mind.ChooseEnemy();
				if (mind.enemy)
				{
					action.SetOrder(); // TODO Нужна передача приказа и его обработка на той стороне
					action.ContinueToAct(mind.enemy.gameObject); // TODO Передавать объект при инициализации, а не так
					mind.enemy.BeAttacked(this); // TODO В дальнейшем необходимо переделать, чтобы противник не сразу мог понять кто его атакует, эту информацию надо получить самому
					FightMonitorScr.Monitor.NewFight(this);
				}
				break;
			case OrderType.Stop:
				ChangeAction(new Idle()); // TODO Прописать обнуления всех параметров, участвовавших в действиях (например "враг")
				break;
		}
	}

	public void ContinueToAct() // TODO Переделать в Действие, а воин сам уже должен выбирать, какое именно совершить // TODO Добавить работу мозга на этом этапе
	{
		action.ContinueToAct();
	}

	public void BeAttacked(WarriorScr _striker)
	{
		mind.BeAttacked(_striker);
	}

	public void TakeHit(WarriorScr _striker)
	{
		int damage = action.EnemyHitReaction(_striker);
		if (damage > 0)
		{
			body.TakeDamage(damage); // TODO Нужна система получения урона и последствий
		}
	}

	// * Управление состояниями //////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void ChangeBodyCondition(BodyState _bodyCondition)
	{
		body = _bodyCondition;
		body.SetOwner(this);
	}

	public void ChangeAction(ActionState _actionState)
	{
		action = _actionState;
		action.SetOwner(this);
	}

	public void ChangeMindCondition(MindState _mindState)
	{
		mind = _mindState;
		mind.SetOwner(this);
	}

	// * История воина ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void AddToStory(string _text)
    {
		hisStory.Add(_text);
	}

	void PrintStory()
	{
		foreach(string s in hisStory)
		{
			print(s);
		}
	}

	// * Служебное ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public GUIData GetDataForGUI()
	{
		return new GUIData(this.name, hisStory);
	}
}
