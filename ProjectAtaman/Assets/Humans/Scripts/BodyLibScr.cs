using System;
using System.Collections;
using System.Collections.Generic;

namespace HumanLib
{
	[Serializable]
	public enum BodyCondition
	{
		good,
		bad,
		dying,
		dead
	}

	public abstract class BodyState : State // Родительский класс для всех состояний тела
	{
		// * Абстрактные составляющие
		public abstract BodyCondition Condition { get; }
		public abstract float WalkSpeed { get; }
		public abstract float RunSpeed { get; }
		public abstract float TurnSpeed { get; }

		public abstract void TakeDamage(int _damage); // TODO Вред должно быть разного типа (заражение, удар, вывих, усталость)
		
		// * Общие данные
		protected List<Ailment> ailments = new List<Ailment>(); // Список недугов

		// * Конструктор
		public BodyState(HumanScr _owner) : base (_owner) { }
	}
	
	// * ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// * Варианты состояний тела ////////////////////////////////////////////////////////////////////////////////////////
	class GoodBody : BodyState
	{
		public override BodyCondition Condition { get => BodyCondition.good; }
		public override float WalkSpeed { get; }
		public override float RunSpeed { get; }
		public override float TurnSpeed { get; }

		public GoodBody(HumanScr _owner) : base(_owner) { }

		public override void TakeDamage(int _damage)
		{
			owner.AddToStory("Получил удар");
			owner.ChangeBodyState(new BadBody(owner));
		}
	}

	class BadBody : BodyState
	{
		public override BodyCondition Condition { get => BodyCondition.bad; }
		public override float WalkSpeed { get; }
		public override float RunSpeed { get; }
		public override float TurnSpeed { get; }

		public BadBody(HumanScr _owner) : base(_owner) { }

		public override void TakeDamage(int _damage)
		{
			owner.AddToStory("Смертельно ранен");
			owner.ChangeAction(new NothingCanAction(owner));
			owner.ChangeBodyState(new DyingBody(owner));
		}
	}

	class DyingBody : BodyState // TODO Нужно добавить угасание жизни со временем
	{
		public override BodyCondition Condition { get => BodyCondition.dying; }
		public override float WalkSpeed { get; }
		public override float RunSpeed { get; }
		public override float TurnSpeed { get; }

		public DyingBody(HumanScr _owner) : base(_owner) { }
		
		public override void TakeDamage(int _damage)
		{
			owner.AddToStory("Умер");
			owner.ChangeAction(new NothingCanAction(owner));
			owner.ChangeBodyState(new DeadBody(owner));
			owner.ChangeMindState(new DeadMind(owner));
		}
	}

	class DeadBody : BodyState
	{
		public override BodyCondition Condition { get => BodyCondition.dead; }
		public override float WalkSpeed { get; }
		public override float RunSpeed { get; }
		public override float TurnSpeed { get; }
		public DeadBody(HumanScr _owner) : base(_owner) { }
		public override void TakeDamage(int _damage) { }
	}

	// * ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// * Недуги /////////////////////////////////////////////////////////////////////////////////////////////////////////
	// TODO Вынести в отдельную либу
	[Serializable]
	public enum AilmentType
	{
		ill,
		wounded, // Ранен
		crippled // Покалечен
	}

	[Serializable]
	public class Ailment
	{
		public string Name { get; }
		public AilmentType Type { get; }
		public string Description { get; }

		public Ailment(string _name, AilmentType _type, string _description)
		{
			Name = _name;
			Type = _type;
			Description = _description;
		}

		public static List<Ailment> CommonAilments = new List<Ailment> // TODO Дополнить список // TODO Переделать, чтобы легче было использовать. Или добавить функцию
        {
			new Ailment("Bleeding", AilmentType.wounded, "_description_") // TODO Встроить поддержку нескольких языков
        };
	}
}
