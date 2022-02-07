using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ! В Одиночке есть проблема с многопоточностью, необходимо переделать, если продолжу использовать
public class FightMonitorScr : MonoBehaviour //? BattleMonitor?
{
    public static FightMonitorScr Monitor { get; private set; }
    [SerializeField] private List<WarriorScr> warringWarriors = new List<WarriorScr>(); // TODO Нужен список не самих воинов, а сражений у каждого воюющего
    
    /*private float timeCounter = 0; // Заготовочка для установки скорости / темпа игры
	timeCounter += Time.deltaTime;
    if (timeCounter > GameManager.GM.GameSpeed)
    {
        timeCounter -= GameManager.GM.GameSpeed;
    }*/

    void Awake()
    {
        Monitor = this;
    }

    void FixedUpdate() // ? Что вообще следует мониторить тут? Не просто же пробегать по действующим лицам и заставлять их продолжать действовать?
    {
        if (!GameManager.GM.IsPaused)
        {
            warringWarriors.RemoveAll(x => !x.IsAlive); // ? Может стоит заменить список воюющих на общий список, тогда поиск по списку будет один
            warringWarriors.RemoveAll(x => x.Status != HumanLib.ActionStatus.fighting);
            foreach (WarriorScr w in warringWarriors)
            {
                w.ContinueToAct();
            }
        }
    }

    public void NewFight(WarriorScr _warrior)
    {
		if (!warringWarriors.Exists(x => x == _warrior))
		{
			warringWarriors.Add(_warrior);
		}
    }
}
