using System;
using System.Collections;
using System.Collections.Generic;

namespace HumanLib
{
	public interface IAttacked
	{
		public void BeAttacked(WarriorScr _striker);
		public void TakeHit(WarriorScr _striker);
	}

	public class Assault // TODO Нужен параметр очередности или скорости, чтобы определять порядок опроса сражений по приоритету, а не случайно
	{
		public WarriorScr Striker { get; private set; }
		public List<IAttacked> Targets { get; private set; }
		public float NextActionTime { get; private set; } = 0f;
		public bool IsAssaultContinues { get; private set; }

		public Assault(WarriorScr _striker, IAttacked _target)
		{
			Striker = _striker;
			Targets = new List<IAttacked>();
			Targets.Add(_target);
			IsAssaultContinues = true;
		}

		public Assault(WarriorScr _striker, List<IAttacked> _targets)
		{
			Striker = _striker;
			Targets = _targets;
			IsAssaultContinues = true;
		}

		public void Continue()
		{
			Striker.ContinueToAct();
		}

		public void ShiftNextActionTime(float _deltaTime)
		{
			NextActionTime = NextActionTime + _deltaTime;
		}
	}

	// * Боевые силы и возможности /////////////////////////////////////////////////////////////////////////////////
	[Serializable]
	public enum ForceCondition
	{
		unarmed
	}

	public abstract class ForceState : State
	{
		public ForceState(WarriorScr _owner) : base(_owner) { }

		//private int chanceHitFailure = 10; // Шанс неудачной атаки в процентах, зависящий от атакующего (например: споткнулся, соскользнула рука) // TODO Переделать в функцию, вычисляющую значение в зависимости от Состояния тела + Состояния ума + Снаряжения
		//private float impactForce = 1f; // Сила удара // TODO Нужен класс оружие (без оружия, нож, пика ...) и там хранить данный параметр
		//private float initiative = 5f;
		// ? Класс брони / защиты?
		// ? Уклонение
		// ? Класс вооружения? Список с указанием основного и запасного
		// ? Как подбирать чужое оружие и где это реализовать? В работу мозга разместить выбор из имеющегося оружия, а также оценку возможности и необходимости подбирать новое
		// ? Список классов способностей боевых
		// ? Уровень навыков, мастерство
		// ! Пересмотреть записи, мог упустить что-нибудь
	}

	public class UnarmedForce : ForceState
	{
		public UnarmedForce(WarriorScr _owner) : base(_owner) { }
	}	
}
