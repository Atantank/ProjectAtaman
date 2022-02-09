using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanLib
{
	[Serializable]
	public enum ActionStatus
	{
		idle,
		walking,
		running,
		fighting,
		nothingCan // Конечное, из этого состояния уже не выйти
	}

	public abstract class ActionState : State // Родительский класс для всех вариантов действий
	{
		// * Абстрактные составляющие
		public abstract ActionStatus Status { get; }
		public abstract ActionStatus VisibleStatus { get; }
		public abstract void ContinueToAct();

		// * Внутренние параметры
		private float TurningSpeed { get; }
		protected BodyState body;
		protected SpiritState spirit;

		// * Конструктор
		public ActionState(WarriorScr _owner, BodyState _body, SpiritState _spirit) : base(_owner)
		{
			body = _body;
			spirit = _spirit;
		}

		// * Активные действия
		public int HitReaction(WarriorScr _striker) // TODO Реализовать ответные действия по обратной ссылке на нападающего
		{
			return 1;
		}

		// Служебные функции
		protected void MoveTo() // TODO Нужно передавать коэффециэнт сопротивления внешней среды, здесь он будет влиять на скорость // TODO Нужна либо своя система координат на карте, либо метод конвертации одного в другое
		{
			//owner.transform
		}

		protected void LookAt()
		{
			//owner.transform
		}
	}

	// * Варианты действий //////////////////////////////////////////////////////////////////////////////
	public class IdleAction : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.idle; }
		public override ActionStatus VisibleStatus { get => ActionStatus.idle; }

		public IdleAction(WarriorScr _owner, BodyState _body, SpiritState _spirit) : base(_owner, _body, _spirit) { }
		
		public override void ContinueToAct() { }
	}

	public class WalkingAction : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.walking; }
		public override ActionStatus VisibleStatus { get => ActionStatus.walking; }
		
		public WalkingAction(WarriorScr _owner, BodyState _body, SpiritState _spirit) : base(_owner, _body, _spirit) { }
		
		public override void ContinueToAct()
		{ }
	}

	public class RunningAction : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.running; }
		public override ActionStatus VisibleStatus { get => ActionStatus.running; }
		
		public RunningAction(WarriorScr _owner, BodyState _body, SpiritState _spirit) : base(_owner, _body, _spirit) { }
		
		public override void ContinueToAct()
		{ }
	}

	public class FightingAction : ActionState
	{
		private WarriorScr enemy;
		private Assault assault;
		public override ActionStatus Status { get => ActionStatus.fighting; }
		public override ActionStatus VisibleStatus { get => ActionStatus.fighting; }

		public FightingAction(WarriorScr _owner, WarriorScr _enemy, BodyState _body, SpiritState _spirit) : base(_owner, _body, _spirit)
		{
			enemy = _enemy;
			assault = new Assault(owner, enemy);
			ObserverScr.Observer.NewAssault(assault);
		}

		public override void ContinueToAct() // TODO Нужно передавать вектор направления удара по отношению жертвы, чтобы понять был ли удар со спины
		{
			assault.ShiftNextActionTime(1f);
			int hitTry = UnityEngine.Random.Range(0, 101);
			if (hitTry <= 10) // TODO Реализовать корректную работу параметра chanceHitFailure
			{
				owner.AddToStory("Ударить не получилось"); // TODO Добавить Enum с причинами неудачи удара и рандом по нему в числовом виде ИЛИ Класс? и обращаться просто к нему?
			}
			else
			{
				enemy.TakeHit(owner); // TODO Нужен класс Тип удара и Удар?
			}
			if (!enemy.IsAlive)
			{
				owner.ChangeAction(new IdleAction(owner, body, spirit));
			}
		}
	}

	public class NothingCanAction : ActionState // Конечная, ничего не поделаешь
	{
		public override ActionStatus Status { get => ActionStatus.nothingCan; }
		public override ActionStatus VisibleStatus { get => ActionStatus.nothingCan; }
		public NothingCanAction(WarriorScr _owner) : base(_owner, null, null) { }
		public override void ContinueToAct() { }
	}
}
