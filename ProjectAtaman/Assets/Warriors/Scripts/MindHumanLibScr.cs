using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using OrderLib;
using EquipmentLib;

namespace HumanLib
{
	[Serializable]
	public enum MindCondition // TODO Необходимо дополнить
	{
		ordinary,
		dead
	}

	public abstract class MindState : State // Главный мозг с основными мыслительными процессами
	{
		// * Абстрактные составляющие для дочерних классов
		public abstract MindCondition Condition { get; }
		public abstract void SetOrder(Order _order);
		protected abstract void ChooseEnemy();

		// * Общие переменные для всех типов мышления
		public Order Order { get; protected set; }
		public WarriorScr Enemy { get; protected set; }
		protected List<Relation> Relations = new List<Relation>();
		protected BodyState body;
		protected SpiritState spirit;

		// * Конструктор
		public MindState(WarriorScr _owner, BodyState _body, SpiritState _spirit) : base(_owner)
		{
			body = _body;
			spirit = _spirit;
		}

		// * Мыслительные процессы
		protected void BattlefieldInspection() // ? Через спавн круга? или сразу все охватить? Только первые N объектов?
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

		protected void UpdateRelations(RelationType _impactType, WarriorScr _target, RelationType _responseType, string _key = "")
		{
			Relation newRelation = new Relation(_impactType, _target, _responseType);
			if (Relations.Exists(x => x.target == _target))
			{
				int index = Relations.IndexOf(Relations.Find(x => x.target == _target));
				if (_key.Contains("SaveImpact"))
				{
					newRelation.impactType = Relations[index].impactType;
				}
				if (_key.Contains("SaveResponse"))
				{
					newRelation.responseType = Relations[index].responseType;
				}
				Relations[index] = newRelation;
			}
			else
			{
				Relations.Add(newRelation);
			}
		}

		// * Обработка входящей информации
		public void BeAttacked(WarriorScr _striker)
		{
			UpdateRelations(RelationType.nonexistent, _striker, RelationType.attacking, "SaveImpact");
			if (!Enemy) // TODO Необходимо не просто записать во враги, а произвести аналитическую деятельность после этого события
			{
				Enemy = _striker;
			}
			//FightMonitorScr.Monitor.NewFight(owner); // TODO Переделать на связь со сражением, а не в качестве реакции на противника?
		}
	}

	// * Варианты состояний ума //////////////////////////////////////////////////////////////////////////////
	public class OrdinaryMind : MindState
	{
		public override MindCondition Condition { get => MindCondition.ordinary; }

		public OrdinaryMind(WarriorScr _owner, BodyState _body, SpiritState _spirit) : base(_owner, _body, _spirit) { }

		public override void SetOrder(Order _order)
		{
			Order = _order;
			switch (Order.Type)
			{
				case OrderType.AttackEnemy: // ? Связать с образом мысли / характером в Mind: Hunting, AttackingEnemy 
					ChooseEnemy();
					if (Enemy)
					{
						owner.ChangeAction(new FightingAction(owner, Enemy, body, spirit));
					}
					break;
				case OrderType.Stop:
					owner.ChangeAction(new IdleAction(owner, body, spirit)); // TODO Прописать обнуления всех параметров, участвовавших в действиях (например "враг")
					break;
			}
		}

		protected override void ChooseEnemy()
		{
			BattlefieldInspection();

			List<Relation> potentialEnemies = new List<Relation>();
			potentialEnemies = Relations.FindAll(x => x.target.Team != owner.Team && x.target.IsAlive);
			if (potentialEnemies.Count > 0) // TODO Сравнивать с определенной командой противника // TODO Нужна логика выбора оптимального противника
			{
				UpdateRelations(RelationType.attacking, potentialEnemies[0].target, potentialEnemies[0].responseType);
				Enemy = potentialEnemies[0].target;
				//int index = relations.IndexOf(potentialEnemies[1]);
				//relations[index] = new Relation(ImpactType.attacking, relations[index].target, relations[index].responseType);
			}
			else
			{
				Enemy = null;
				owner.AddToStory("Враги не найдены");
			}
		}
	}

	public class DeadMind : MindState // Тупик, никакие действия уже не сделать
	{
		public override MindCondition Condition { get => MindCondition.dead; }
		public DeadMind(WarriorScr _owner) : base(_owner, null, null) { }
		public override void SetOrder(Order _order) { }
		protected override void ChooseEnemy() { }
	}
}
