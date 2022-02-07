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

	public abstract class ActionState : State
	{
		public abstract ActionStatus Status { get; }

		public abstract void ContinueToAct(GameObject _object = null);

		public void SetOrder()
		{
			owner.ChangeAction(new Fighting());
		}

		public int EnemyHitReaction(WarriorScr _striker) // TODO Реализовать ответные действия по обратной ссылке на нападающего
		{
			return 1;
		}

		private float TurningSpeed { get; }

		protected void MoveTo() // TODO Нужно передавать коэффециэнт сопротивления внешней среды, здесь он будет влиять на скорость // TODO Нужна либо своя система координат на карте, либо метод конвертации одного в другое
		{
			//owner.transform
		}

		protected void LookAt()
		{
			//owner.transform
		}
	}

	public class Idle : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.idle; }

		public override void ContinueToAct(GameObject _object = null) { }
	}

	public class Walking : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.walking; }

		public override void ContinueToAct(GameObject _object = null)
		{ }
	}

	public class Running : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.running; }

		public override void ContinueToAct(GameObject _object = null)
		{ }
	}

	public class Fighting : ActionState
	{
		private WarriorScr enemy;
		public override ActionStatus Status { get => ActionStatus.fighting; }

		public override void ContinueToAct(GameObject _object) // TODO Нужно передавать вектор направления удара по отношению жертвы, чтобы понять был ли удар со спины
		{
			if (_object)
			{
				enemy = _object.GetComponent<WarriorScr>();
			}

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
				owner.ChangeAction(new Idle());
			}
		}
	}

	public class NothingCan : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.nothingCan; }

		public override void ContinueToAct(GameObject _object = null) { }
	}
}
