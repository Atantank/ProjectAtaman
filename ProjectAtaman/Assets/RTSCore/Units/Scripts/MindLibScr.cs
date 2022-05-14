using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using EquipmentLib;
using MainLib;
using OrderLib;

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
		public abstract void SetNewOrder(Order _order);
		protected abstract void ChooseEnemy();

		// * Общие переменные для всех типов мышления
		public Order Order { get; protected set; }
		public IDamageable Target { get; protected set; }
		protected List<Relation> Relations = new List<Relation>();
		protected BodyState body;
		protected SpiritState spirit;
		protected ActionState action;

		// * Конструктор
		public MindState(HumanScr _owner, BodyState _body, SpiritState _spirit, ActionState _action) : base(_owner)
		{
			body = _body;
			spirit = _spirit;
			action = _action;
		}

		// * Мыслительные процессы
		protected void UpdateRelations(RelationType _impactType, IDamageable _target, RelationType _responseType, string _key = "")
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

		protected void BattlefieldInspection() // ? Через спавн круга? или сразу все охватить? Только первые N объектов?
		{
			foreach (Collider c in Physics.OverlapSphere(owner.transform.position, 10f)) // TODO Разный радиус для разных состояний ума и тела через Get
			//foreach (Collider2D c in Physics2D.OverlapCircleAll(owner.transform.position, 10f))
			{
				HumanScr tmpHuman; // ! Работает только против людей
				if (c.gameObject.TryGetComponent<HumanScr>(out tmpHuman) && c.gameObject != owner.gameObject)
				{
					UpdateRelations(RelationType.nonexist, tmpHuman, RelationType.nonexist, "SaveResponse_SaveImpact");
				}
			}
		}

		// * Обработка входящей информации
		public void BeAttacked(IInteractable _striker)
		{
			IDamageable tmpDamageable;
			tmpDamageable = _striker as IDamageable;
			if(tmpDamageable != null)
			{
				UpdateRelations(RelationType.nonexist, tmpDamageable, RelationType.attack, "SaveImpact");
				if (Target == null) // TODO Необходимо не просто записать во враги, а произвести аналитическую деятельность после этого события
				{
					Target = tmpDamageable;
				}
				//FightMonitorScr.Monitor.NewFight(owner); // TODO Переделать на связь со сражением, а не в качестве реакции на противника?
			}
			
		}

		public void HitReaction(IInteractable _striker)
		{
			int damage = action.HitReaction(_striker);
			if (damage > 0)
			{
				body.TakeDamage(damage); // TODO Нужна система получения урона и последствий
			}
		}
	}

	// * ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	// * Варианты состояний ума /////////////////////////////////////////////////////////////////////////////////////////
	public class OrdinaryMind : MindState
	{
		public override MindCondition Condition { get => MindCondition.ordinary; }

		public OrdinaryMind(HumanScr _owner, BodyState _body, SpiritState _spirit, ActionState _action) : base(_owner, _body, _spirit, _action) { }

		public override void SetNewOrder(Order _order)
		{
			Order = _order;
			switch (Order.Type)
			{
				case OrderType.AttackEnemy: // ? Связать с образом мысли / характером в Mind: Hunting, AttackingEnemy 
					if (Order.Mark != null)
					{
						Target = Order.Mark.DamageableTarget;
					}
					else
					{
						ChooseEnemy(); // ! Устарело
					}
					if (Target != null)
					{
						owner.ChangeAction(new AttackingAction(owner, Target, body, spirit));
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
			potentialEnemies = Relations.FindAll(x => x.target.CanBeTarget());
			//potentialEnemies = Relations.FindAll(x => x.target.Team != owner.Team && x.target.IsAlive);
			if (potentialEnemies.Count > 0) // TODO Сравнивать с определенной командой противника // TODO Нужна логика выбора оптимального противника
			{
				UpdateRelations(RelationType.attack, potentialEnemies[0].target, potentialEnemies[0].responseType);
				Target = potentialEnemies[0].target;
				//int index = relations.IndexOf(potentialEnemies[1]);
				//relations[index] = new Relation(ImpactType.attacking, relations[index].target, relations[index].responseType);
			}
			else
			{
				Target = null;
				owner.AddToStory("Враги не найдены");
			}
		}
	}

	public class DeadMind : MindState // Тупик, никакие действия уже не сделать
	{
		public override MindCondition Condition { get => MindCondition.dead; }
		public DeadMind(HumanScr _owner) : base(_owner, null, null, null) { }
		public override void SetNewOrder(Order _order) { }
		protected override void ChooseEnemy() { }
	}
}
