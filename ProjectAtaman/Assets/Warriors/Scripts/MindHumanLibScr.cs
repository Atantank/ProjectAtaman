using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanLib
{
	[Serializable]
	public enum MindCondition
	{
		ordinary,
		dead
	}

	public abstract class MindState : State
	{
		//private __ actionPriority; // ? Нужен ли параметр, определяющий приоритет действий (несколько на выбор)
		public abstract MindCondition Condition { get; }
		public List<Relation> relations = new List<Relation>();
		public WarriorScr enemy;

		public abstract void ChooseEnemy();

		public void BeAttacked(WarriorScr _striker)
		{
			UpdateRelations(RelationType.nonexistent, _striker, RelationType.attacking, "SaveImpact");
			FightMonitorScr.Monitor.NewFight(owner); // TODO Переделать на связь со сражением, а не в качестве реакции на противника?
		}

		public void BattlefieldInspection() // TODO Порождать ОСМОТР или иной способ сбора инфорации. Через спавн круга? или сразу все охватить? Только первые N объектов?
		{
			foreach (Collider c in Physics.OverlapSphere(owner.transform.position, 10f)) // TODO Разный радиус для разных состояний ума и тела через Get
																						 //foreach (Collider2D c in Physics2D.OverlapCircleAll(owner.transform.position, 10f))
			{
				WarriorScr tmpWarrior;
				if (c.gameObject.TryGetComponent<WarriorScr>(out tmpWarrior) && c.gameObject != owner.gameObject)
				{
					UpdateRelations(RelationType.nonexistent, tmpWarrior, RelationType.nonexistent, "SaveResponse_SaveImpact");
				}
			}
		}

		public void UpdateRelations(RelationType _impactType, WarriorScr _target, RelationType _responseType, string _key = "")
		{
			Relation newRelation = new Relation(_impactType, _target, _responseType);
			if (relations.Exists(x => x.target == _target))
			{
				int index = relations.IndexOf(relations.Find(x => x.target == _target));
				if (_key.Contains("SaveImpact"))
				{
					newRelation.impactType = relations[index].impactType;
				}
				if (_key.Contains("SaveResponse"))
				{
					newRelation.responseType = relations[index].responseType;
				}
				relations[index] = newRelation;
			}
			else
			{
				relations.Add(newRelation);
			}
		}
	}

	public class OrdinaryMind : MindState
	{
		public override MindCondition Condition { get => MindCondition.ordinary; }

		public override void ChooseEnemy()
		{
			BattlefieldInspection();

			List<Relation> potentialEnemies = new List<Relation>();
			potentialEnemies = relations.FindAll(x => x.target.Team != owner.Team && x.target.IsAlive);
			if (potentialEnemies.Count > 0) // TODO Сравнивать с определенной командой противника // TODO Нужна логика выбора оптимального противника
			{
				UpdateRelations(RelationType.attacking, potentialEnemies[0].target, potentialEnemies[0].responseType);
				enemy = potentialEnemies[0].target;
				//int index = relations.IndexOf(potentialEnemies[1]);
				//relations[index] = new Relation(ImpactType.attacking, relations[index].target, relations[index].responseType);
			}
			else
			{
				enemy = null;
				owner.AddToStory("Враги не найдены");
			}
		}
	}

	public class DeadMind : MindState
	{
		public override MindCondition Condition { get => MindCondition.dead; }
		public override void ChooseEnemy() { }
	}
}
