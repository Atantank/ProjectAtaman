using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FightLib;

// ! В паттерне "Одиночка" есть проблема с многопоточностью, необходимо переделать, если продолжу использовать
public class ObserverScr : MonoBehaviour //? BattleMonitor?
{
    public static ObserverScr Observer { get; private set; }
    [SerializeField] private List<Assault> assaults = new List<Assault>();
	private float timeCounter = 0f;
    private float timeTemp { get => GameManager.GM.GameSpeed; }

    void Awake()
    {
        Observer = this;
    }

    void FixedUpdate() // ? Что вообще следует мониторить тут? Не просто же пробегать по действующим лицам и заставлять их продолжать действовать?
    {
		timeCounter = timeCounter + Time.deltaTime*timeTemp;
        if (!GameManager.GM.IsPaused)
        {
			assaults.RemoveAll(x => !x.IsAssaultContinues);
            foreach (Assault f in assaults.FindAll(x => x.NextActionTime < timeCounter))
            {
                f.Continue(); // TODO Передавать дельту времени
            }
        }
    }

    public void NewAssault(Assault _assault) // TODO Переделать, чтобы отпала необходимость регистрироваться, чтобы не было "забывашек"
    {
		if (!assaults.Exists(x => x == _assault)) // TODO Переработать проверку, данная неполноценна
		{
			assaults.Add(_assault);
			_assault.Targets[0].BeAttacked(_assault.Striker); // TODO В дальнейшем необходимо переделать, чтобы противник не сразу мог понять кто его атакует, эту информацию надо получить самому
		}
    }

    public void SetNextActionTime(float _deltaTime)
    {
        //assaults.Find(x => x.Striker)
    }
}
