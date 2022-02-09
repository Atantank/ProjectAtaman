using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using HumanLib;
using GUILib;
using OrderLib;

// ! Необходимо добавить интерфейс для паттерна Компоновщик при создании подразделений
public class WarriorScr : MonoBehaviour, ISelectable, IOrder, IAttacked
{
	// * Составные части и состояния ///////////////////////////////////////////////////////////////////////////////////////
	[SerializeField] private BodyState body; // Оказывает влияние на дух, разум, действия
	[SerializeField] private SpiritState spirit; // Оказывает влияние на разум и действия
	[SerializeField] private MindState mind; // Собирает всю информацию, анализирует, запускает действия
	[SerializeField] private ActionState action; // Действия, с учетом тела, духа и 
	[SerializeField] private ForceState force; // ? Может раскидать по Телу, Мозгу и Духу? Сильно зависит от них, зачем тогда выделять? Оставить только снаряжение?

	// * Условия функционирования //////////////////////////////////////////////////////////////////////////////////////////
	//[SerializeField] private Order order; // TODO Нужен список приказов или лучше очередь, а так же методы работы с ними (отмена, добавление? удаление определенного)
	// TODO Добавить класс условий внешней среды (осадки, температура)
	// TODO Добавить класс поверхности под ногами, чтобы соединить с условиями и повлиять на действия воина

	// * Видимые параметры воина ///////////////////////////////////////////////////////////////////////////////////////////
	public bool IsAlive { get => body.Condition != BodyCondition.dead; }
	public ActionStatus Status { get => action.VisibleStatus; } // Со стороны мы видем не само действие, а то, чем оно кажется. Воин может притвориться и обмануть врага
	[SerializeField] private int team;
    public int Team { get => team; }
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private Color spriteColor;
    [SerializeField] private int viewDirection;
	public int ViewDirection { get => (int)transform.localEulerAngles.z; }

	// * Вспомогательные переменные ////////////////////////////////////////////////////////////////////////////////////////
	[SerializeField] private List<string> hisStory = new List<string>();

    void Awake()
	{
		spriteRenderer.color = spriteColor;
		transform.localEulerAngles = new Vector3(0, 0, viewDirection);
		body = new GoodBody(this);
		spirit = new GoodSpirit(this, body);
		action = new IdleAction(this, body, spirit);
		mind = new OrdinaryMind(this, body, spirit);
		force = new UnarmedForce(this);
	}
	
	void Start()
    {
		SetOrder(new Order(OrderType.AttackEnemy, null));
    }

	// * Обработка взаимодействий объекта /////////////////////////////////////////////////////////////////////////////////////
	void OnTriggerEnter(Collider _collider)
    {}

	public void MouseClick() // TODO Добавить как минимум индивидуальный звуковой отклик при выборе юнита
	{}

	// * Действия и реакции //////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void SetOrder(Order _order) // TODO Перенести в класс приказа
	{
		mind.SetOrder(_order);
		action.ContinueToAct(); // TODO Провести анализ на повторение действия, может вызывать только из одного места, чтобы не было двойного срабатывания?
	}

	public void ContinueToAct() // TODO Добавить работу мозга на этом этапе
	{
		action.ContinueToAct();
	}

	public void BeAttacked(WarriorScr _striker) // ? А нужно ли отдельно это делать? Может обойтись только самим фактом удара?
	{
		mind.BeAttacked(_striker);
	}

	public void TakeHit(WarriorScr _striker)
	{
		int damage = action.HitReaction(_striker); // TODO Сначала передать в мозг, там решить, что делать, может не стоит просто реагировать?
		if (damage > 0)
		{
			body.TakeDamage(damage); // TODO Нужна система получения урона и последствий
		}
	}

	// * Управление состояниями //////////////////////////////////////////////////////////////////////////////////////////////////////////
	public void ChangeBodyCondition(BodyState _bodyCondition)
	{
		body = _bodyCondition;
	}

	public void ChangeMindCondition(MindState _mindState)
	{
		mind = _mindState;
	}

	public void ChangeSpiritCondition(SpiritState _spiritState)
	{
		spirit = _spiritState;
	}

	public void ChangeAction(ActionState _action)
	{
		action = _action;
	}

	public void ChangeForce(ForceState _force) // ? Может она всегда вычисляется на лету и не требует смены подобным образом?
	{
		force = _force;
	}

	// * История воина ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
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

	// * Служебное ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public GUIData GetDataForGUI()
	{
		return new GUIData(this.name, hisStory);
	}
}
