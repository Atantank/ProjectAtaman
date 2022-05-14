using System;
using System.Collections;
using System.Collections.Generic;

using MainLib;

namespace HumanLib
{
    public abstract class State // Родительский класс для всех классов состояний человека
    {
		protected HumanScr owner;

        public State(HumanScr _owner)
        {
            owner = _owner;
        }
    }

	[Serializable]
    public enum RelationType // Типы отношений человека
	{
        attack,
		unknown,
        nonexist
    }

    [Serializable]
    public struct Relation
    {
        public RelationType impactType; // Влияние
        public IDamageable target; // Цель влияния
		public RelationType responseType; // Ответ цели // ? Информация может устаревать, раньше цель била в ответ, а потом перестала, как мониторить подобные изменения?

        public Relation(RelationType _impactType, IDamageable _target, RelationType _responseType)
        {
			impactType = _impactType;
            target = _target;
            responseType = _responseType;
        }

        public void Update()
        {

        }
    }
}