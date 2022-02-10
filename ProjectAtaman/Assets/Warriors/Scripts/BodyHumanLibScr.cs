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
		protected abstract float MoveSpeed { get; } // TODO Реализовать разные способы передвижения: бег, ходьба, ползание (в зависимости от возможснотей тела)
		public abstract void TakeDamage(int _damage); // TODO Вред должно быть разного типа (заражение, удар, вывих, усталость)
		
		// * Общие данные
		protected List<Ailment> ailments = new List<Ailment>(); // Список недугов

		// * Конструктор
		public BodyState(WarriorScr _owner) : base (_owner) { }
	}

	// * Варианты состояний тела //////////////////////////////////////////////////////////////////////////////
	class GoodBody : BodyState
	{
		public override BodyCondition Condition { get => BodyCondition.good; }
		protected override float MoveSpeed { get; }

		public GoodBody(WarriorScr _owner) : base(_owner) { }

		public override void TakeDamage(int _damage)
		{
			owner.AddToStory("Получил удар");
			owner.ChangeBodyState(new BadBody(owner));
		}
	}

	class BadBody : BodyState
	{
		public override BodyCondition Condition { get => BodyCondition.bad; }
		protected override float MoveSpeed { get; }

		public BadBody(WarriorScr _owner) : base(_owner) { }

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
		protected override float MoveSpeed { get; }

		public DyingBody(WarriorScr _owner) : base(_owner) { }
		
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
		protected override float MoveSpeed { get; }
		public DeadBody(WarriorScr _owner) : base(_owner) { }
		public override void TakeDamage(int _damage) { }
	}

	// * Недуги //////////////////////////////////////////////////////////////////////////////
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
