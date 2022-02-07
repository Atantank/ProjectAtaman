using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanLib
{
    public abstract class State
    {
		protected WarriorScr owner;

		public void SetOwner(WarriorScr _owner)
		{
			owner = _owner;
		}
    }

	// * Тело //////////////////////////////////////////////////////////////////////////////
	[Serializable]
    public enum BodyCondition
    {
        good,
        bad,
        dying,
        dead
    }

    public abstract class BodyState : State
    {
		public abstract BodyCondition Condition { get; }

		protected abstract float MoveSpeed { get; } // TODO Реализовать разные способы передвижения: бег, ходьба, ползание (в зависимости от возможснотей тела)
		protected List<Ailment> ailments = new List<Ailment>();

        public abstract void TakeDamage(int _damage);
    }

    class GoodBody : BodyState
    {
        public override BodyCondition Condition { get => BodyCondition.good; }
		protected override float MoveSpeed { get; }

        public override void TakeDamage(int _damage)
        {
			owner.AddToStory("Получил удар");
            owner.ChangeBodyCondition(new BadBody());
        }
    }

    class BadBody : BodyState
    {
		public override BodyCondition Condition { get => BodyCondition.bad; }
		protected override float MoveSpeed { get; }

		public override void TakeDamage(int _damage)
        {
			owner.AddToStory("Смертельно ранен");
			owner.ChangeAction(new NothingCan());
			owner.ChangeBodyCondition(new DyingBody());
        }
    }

    class DyingBody : BodyState // TODO Нужно добавить угасание жизни со временем
	{
		public override BodyCondition Condition { get => BodyCondition.dying; }
		protected override float MoveSpeed { get; }

		public override void TakeDamage(int _damage)
        {
			owner.AddToStory("Умер");
			owner.ChangeAction(new NothingCan());
			owner.ChangeBodyCondition(new DeadBody());
            owner.ChangeMindCondition(new DeadMind());
        }
    }

    class DeadBody : BodyState
    {
		public override BodyCondition Condition { get => BodyCondition.dead; }
		protected override float MoveSpeed { get; }
        public override void TakeDamage(int _damage){}
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

	// * Разум //////////////////////////////////////////////////////////////////////////////
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
		public override void ChooseEnemy() {}
	}

	// * Действия //////////////////////////////////////////////////////////////////////////////
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

        public override void ContinueToAct(GameObject _object = null) {}
    }

	public class Walking : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.walking; }

		public override void ContinueToAct(GameObject _object = null)
		{}
	}

	public class Running : ActionState
	{
		public override ActionStatus Status { get => ActionStatus.running; }

		public override void ContinueToAct(GameObject _object = null)
		{}
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

		public override void ContinueToAct(GameObject _object = null){}
	}

	// * Бой //////////////////////////////////////////////////////////////////////////////
	[Serializable]
    public enum RelationType
    {
        attacking,
        nonexistent
    }

    [Serializable]
    public struct Relation
    {
        public RelationType impactType;
        public WarriorScr target;
        public RelationType responseType;

        public Relation(RelationType _impactType, WarriorScr _target, RelationType _responseType)
        {
			impactType = _impactType;
            target = _target;
            responseType = _responseType;
        }
    }
}