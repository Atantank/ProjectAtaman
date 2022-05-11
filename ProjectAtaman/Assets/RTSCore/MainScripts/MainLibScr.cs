using System;
using System.Collections;
using System.Collections.Generic;

using GUILib;
using OrderLib;

namespace MainLib 
{
	public interface ISelectable
	{
		public void Select();
		public GUIData GetDataForGUI();
	}

	public interface IInteractable
	{
		public void ContinueToAct();
		public List<OrderType> GetPossibleOrderTypes();
		public void SetNewOrder(Order _order);
	}

	public interface IDamageable
	{
		public bool CanBeTarget();
		public void BeAttacked(IInteractable _striker);
		public void TakeHit(IInteractable _striker);
	}
}