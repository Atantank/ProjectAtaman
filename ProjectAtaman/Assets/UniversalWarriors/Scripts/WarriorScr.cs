using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HumanLib;
using FightLib;
using GUILib;

// ! Необходимо добавить интерфейс для паттерна Компоновщик при создании подразделений
public class WarriorScr : MonoBehaviour, IChosen
{
	// * Составные части и состояния ///////////////////////////////////////////////////////////////////////////////////////
	private BodyState body;
	private MindState mind;
	private ActionState action;
	// TODO Добавить Дух

	// * Внешние параметры живого существа /////////////////////////////////////////////////////////////////////////////////
	public bool IsAlive { get => body.Condition != BodyCondition.dead; }
	public ActionStatus Status { get => action.Status; }
	[SerializeField] private int team;
    public int Team { get => team; }
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Color spriteColor;
    [SerializeField] private int viewDirection;
	//public int ViewDirection { get => (int)transform.localEulerAngles.z; }

	// * Управление? //////////////////////////////////////////////////////////////////////////////////////////////////
	private Command command;

	// * Разобрать! //////////////////////////////////////////////////////////////////////////////////////////////////
	// TODO Вынести все в отдельный класс
	//private int chanceHitFailure = 10; // Шанс неудачной атаки в процентах, зависящий от атакующего (например: споткнулся, соскользнула рука) // TODO Переделать в функцию, вычисляющую значение в зависимости от Состояния тела + Состояния ума + Снаряжения
	//private float impactForce = 1f; // Сила удара // TODO Нужен класс оружие (без оружия, нож, пика ...) и там хранить данный параметр
	//private float initiative = 5f;

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
		DirectReceiveInformation(new Command(CommandType.AttackEnemyUnit, null));
    }

	/*public void Init(string _name, int _team, int _viewDirection, Sprite _sprite, Color _color) // TODO Добавить перегрузку без повторений с возможностью задавать необязательные параметры
    {
		this.name = _name;
		team = _team;
		viewDirection = _viewDirection;
		spriteRenderer.sprite = _sprite;
		spriteRenderer.color = _color;
	}*/

	// * Обработка входящей информации /////////////////////////////////////////////////////////////////////////////////////
	// ! Применить паттерн Команда
	void OnTriggerEnter(Collider _collider)
    {}

	public void DirectReceiveInformation(Command _command) // Прямое получение информации
	{
		command = _command;
		switch(command.Type)
		{
			case CommandType.AttackEnemyUnit:
				mind.ChooseEnemy();
				if (mind.enemy)
				{
					action.SetOrder(); // TODO Нужна передача приказа и его обработка на той стороне
					action.ContinueToAct(mind.enemy.gameObject); // TODO Передавать объект при инициализации, а не так
					mind.enemy.BeAttacked(this); // TODO В дальнейшем необходимо переделать, чтобы противник не сразу мог понять кто его атакует, эту информацию надо получить самому
					FightMonitorScr.Monitor.NewFight(this);
				}
				break;
			case CommandType.Stop:
				ChangeAction(new Idle()); // TODO Прописать обнуления всех параметров, участвовавших в действиях (например "враг")
				break;
		}
	}

	public void MouseClick()
	{}

	public GUIData GetGUIData()
	{
		return new GUIData(this.name, hisStory);
	}

	// * Действия //////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void BeAttacked(WarriorScr _striker)
    {
		mind.UpdateRelations(RelationType.nonexistent, _striker, RelationType.attacking, "SaveImpact");
		FightMonitorScr.Monitor.NewFight(this);
	}

	public void ContinueToAct() // TODO Переделать в Действие, а воин сам уже должен выбирать, какое именно совершить
	{
		action.ContinueToAct();
	}

	public void TakeHit(WarriorScr _striker) // TODO Реализовать ответные действия по обратной ссылке на нападающего
    {
		body.TakeDamage(1); // TODO Нужна система получения урона и последствий
	}

	// * Служебные //////////////////////////////////////////////////////////////////////////////////////////////////////////
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
}
