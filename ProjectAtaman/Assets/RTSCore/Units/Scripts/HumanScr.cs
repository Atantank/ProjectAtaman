using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HumanLib;
using MainLib;
using GUILib;
using OrderLib;

// ! Необходимо добавить интерфейс для паттерна Компоновщик при создании подразделений
public class HumanScr : MonoBehaviour, ISelectable, IDamageable, IInteractable
{
	// * Составные части и состояния ////////////////////////////////////////////////////////////////////////////////////
	private BodyState body; // Пассивно оказывает влияние на дух, разум, действия
	private SpiritState spirit; // Пассивно оказывает влияние на разум и действия
	private MindState mind; // Активно собирает всю информацию, анализирует, запускает действия
	private ActionState action; // Исполнительная система. Действия, с учетом состояний тела, духа и установок (идущих от разума)
	[SerializeField] private GameObject viewDirectionArrow;
	[SerializeField] private GameObject SelectionSprite;

	// * Условия функционирования ///////////////////////////////////////////////////////////////////////////////////////
	[SerializeField] private List<OrderType> possibleOrderTypes;
	//[SerializeField] private Order order; // TODO Нужен список приказов или лучше очередь, а так же методы работы с ними (отмена, добавление? удаление определенного)
	// TODO Добавить класс условий внешней среды (осадки, температура)
	// TODO Добавить класс поверхности под ногами, чтобы соединить с условиями и повлиять на действия воина

	// * Видимые извне параметры ////////////////////////////////////////////////////////////////////////////////////////
	// TODO Необходимо все это собрать в класс, и он должен быть у всех объектов в игре, чтобы все прочие объекты могли смотреть или иным способом изучать цель и получать информацию о ней
	public bool IsAlive { get => body.Condition != BodyCondition.dead; }
	public ActionStatus VisibleStatus { get => action.VisibleStatus; } // Со стороны мы видем не само действие, а то, чем оно кажется. Воин может притвориться и обмануть врага
    public int VisibleTeam { get => team; }
	public int ViewDirection { get => (int)viewDirectionArrow.transform.localEulerAngles.z; }

	// * Невидимые извне параметры //////////////////////////////////////////////////////////////////////////////////////
	// ? Соединить все в один класс ИЛИ распределить по составляющим? ///////////////////////////////////////////////////
	[SerializeField] private int team;
	[SerializeField] private SpriteRenderer bodySpriteRenderer;
	[SerializeField] private Color spriteColor;
    [SerializeField] private int viewDirectionStartAngle;

	// * Вспомогательные переменные /////////////////////////////////////////////////////////////////////////////////////
	[SerializeField] private List<string> hisStory = new List<string>();
	
	// * ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// * Базовые функции ////////////////////////////////////////////////////////////////////////////////////////////////
	void Awake()
	{
		SelectionSprite.SetActive(false);
		bodySpriteRenderer.color = spriteColor;
		viewDirectionArrow.transform.Rotate(Vector3.right, viewDirectionStartAngle, Space.Self);
		// transform.localEulerAngles = new Vector3(0, 0, viewDirection);
		body = new GoodBody(this);
		spirit = new GoodSpirit(this, body);
		action = new IdleAction(this, body, spirit);
		mind = new OrdinaryMind(this, body, spirit, action);
	}
	
	void Start()
    {
		
    }

	// * ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// * Интерфейс ISelectable //////////////////////////////////////////////////////////////////////////////////////////
	public void Select() // TODO Добавить как минимум индивидуальный звуковой отклик при выборе юнита
	{
		SelectionSprite.SetActive(true);
	}

	public void UnSelect()
	{
		SelectionSprite.SetActive(false);
	}

	public GUIData GetDataForGUI()
	{
		return new GUIData(this.name, hisStory);
	}

	public IInteractable GetInteractable()
	{
		return this;
	}

	// * Интерфейс IControllable ////////////////////////////////////////////////////////////////////////////////////////
	public void SetNewOrder(Order _order) // TODO Перенести в класс приказа
	{
		mind.SetNewOrder(_order);
		ContinueToAct(); // TODO Провести анализ на повторение действия, может вызывать только из одного места, чтобы не было двойного срабатывания?
	}

	// * Интерфейс IDamageable //////////////////////////////////////////////////////////////////////////////////////////
	public bool CanBeTarget()
	{
		bool result;
		if (/*team != _team && */IsAlive)
			result = true;
		else
			result = false;
		return result;
	}

	public void BeAttacked(IInteractable _striker) // ? А нужно ли отдельно это делать? Может обойтись только самим фактом удара?
	{
		mind.BeAttacked(_striker);
	}

	public void TakeHit(IInteractable _striker)
	{
		mind.HitReaction(_striker);
	}

	// * Интерфейс IInteractable ////////////////////////////////////////////////////////////////////////////////////////
	public void ContinueToAct() // TODO Добавить работу мозга на этом этапе
	{
		action.ContinueToAct();
	}

	public bool TryGetDamageable(out IDamageable _target)
	{
		_target = this;
		return true;
	}

	public List<OrderType> GetPossibleOrderTypes()
	{
		return possibleOrderTypes;
	}

	// * ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// * Управление состояниями /////////////////////////////////////////////////////////////////////////////////////////
	public void ChangeBodyState(BodyState _bodyState)
	{
		body = _bodyState;
	}

	public void ChangeMindState(MindState _mindState)
	{
		mind = _mindState;
	}

	public void ChangeSpiritState(SpiritState _spiritState)
	{
		spirit = _spiritState;
	}

	public void ChangeAction(ActionState _action)
	{
		action = _action;
	}

	// * ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// * История существа ///////////////////////////////////////////////////////////////////////////////////////////////
	public void AddToStory(string _text)
    {
		hisStory.Add(_text);
	}

	public void PrintStory()
	{
		foreach(string s in hisStory)
		{
			print(s);
		}
	}
}
