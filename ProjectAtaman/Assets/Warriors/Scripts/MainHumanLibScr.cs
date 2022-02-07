using System;
using System.Collections;
using System.Collections.Generic;

namespace HumanLib
{
    public abstract class State // Родительский класс для всех классов состояния
    {
		protected WarriorScr owner;

		public void SetOwner(WarriorScr _owner)
		{
			owner = _owner;
		}
    }

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