using System;
using System.Collections;
using System.Collections.Generic;

using GUILib;
using OrderLib;

namespace MainLib 
{
	// Интерфейс, описывающий возможность игрока выделять игровой объект, и получать информацию от него
	public interface ISelectable
	{
		// Действия объекта при его выделении
		public void Select();
		// Действия объекта при сбросе выделения
		public void UnSelect();
		// Получить данные об объекте для заполнения информации в интерфейсе
		public GUIData GetDataForGUI();
	}

	// Интерфейс описывает объекты, которыми игрок может управлять
	public interface IInteractable
	{
		// Функция, которая будет вызываться из FixedUpdate для обработки текущих действия объекта
		public void ContinueToAct();
		// Получить список команд, которые может исполнить объект
		public List<OrderType> GetPossibleOrderTypes();
		// Установить новую команду для объекта
		public void SetNewOrder(Order _order);
	}

	// Интерфейс описывает поведение объекта при получении урона
	public interface IDamageable
	{
		public bool CanBeTarget();
		public void BeAttacked(IInteractable _striker);
		public void TakeHit(IInteractable _striker);
	}
}