using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MainLib;
using FightLib;

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
		protected BodyState body;
		protected SpiritState spirit;
		private float turnSpeed { get => body.TurnSpeed; }
		private float walkSpeed { get => body.WalkSpeed; }
		private float runSpeed { get => body.RunSpeed; }

		// * Конструктор
		public ActionState(HumanScr _owner, BodyState _body, SpiritState _spirit) : base(_owner)
		{
			body = _body;
			spirit = _spirit;
		}

		// * Активные действия
		// TODO Сделать абстрактной, чтобы для каждого вида деятельности был свой способ реагирования
		// TODO Реализовать ответные действия по обратной ссылке на нападающего
		public int HitReaction(IInteractable _striker) 
		{
			return 1;
		}

		// Служебные функции
		// TODO Нужно передавать коэффециэнт сопротивления внешней среды, здесь он будет влиять на скорость // TODO Нужна либо своя система координат на карте, либо метод конвертации одного в другое
		protected void MoveTo() 
		{
			//owner.transform
		}

		protected void LookAt()
		{
			//sowner.transform.Rotate(0, 0, -1 * Time.deltaTime * turnSpeed);
		}
	}

	// * ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// * Варианты действий //////////////////////////////////////////////////////////////////////////////////////////////
	public class IdleAction : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.idle; }
		public override ActionStatus VisibleStatus { get => ActionStatus.idle; }

		public IdleAction(HumanScr _owner, BodyState _body, SpiritState _spirit) : base(_owner, _body, _spirit) { }
		
		public override void ContinueToAct() { }
	}

	public class WalkingAction : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.walking; }
		public override ActionStatus VisibleStatus { get => ActionStatus.walking; }
		
		public WalkingAction(HumanScr _owner, BodyState _body, SpiritState _spirit) : base(_owner, _body, _spirit) { }
		
		public override void ContinueToAct()
		{ }
	}

	public class RunningAction : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.running; }
		public override ActionStatus VisibleStatus { get => ActionStatus.running; }
		
		public RunningAction(HumanScr _owner, BodyState _body, SpiritState _spirit) : base(_owner, _body, _spirit) { }
		
		public override void ContinueToAct()
		{ }
	}

	public class AttackingAction : ActionState
	{
		private IDamageable target;
		private Assault assault;
		public override ActionStatus Status { get => ActionStatus.fighting; }
		public override ActionStatus VisibleStatus { get => ActionStatus.fighting; }

		public AttackingAction(HumanScr _owner, IDamageable _target, BodyState _body, SpiritState _spirit) : base(_owner, _body, _spirit)
		{
			target = _target;
			assault = new Assault(owner, target);
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
				target.TakeHit(owner); // TODO Нужен класс Тип удара и Удар?
			}
			if (target.CanBeTarget())
			{
				owner.ChangeAction(new IdleAction(owner, body, spirit));
			}
		}
	}

	public class NothingCanAction : ActionState // Конечная, ничего не поделаешь
	{
		public override ActionStatus Status { get => ActionStatus.nothingCan; }
		public override ActionStatus VisibleStatus { get => ActionStatus.nothingCan; }
		public NothingCanAction(HumanScr _owner) : base(_owner, null, null) { }
		public override void ContinueToAct() { }
	}
}
