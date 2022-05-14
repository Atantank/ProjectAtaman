using System.Collections;
using UnityEngine;

public class CameraControllScr : MonoBehaviour
{
    [SerializeField] private GameObject mainCamera;
	[SerializeField] private bool isOrthographic;

	//public Transform startPoint;
	[SerializeField] private float moveSpeed = 5;
	[SerializeField] private float zoomSpeed = 5;
	[SerializeField] private float maxHeight = 15;
	[SerializeField] private float minHeight = 10;
	[SerializeField] private float stepHeight = 1;
	[SerializeField] private float startHeight = 10;

	private Controller controller;
	private float currentHeight;
	private float aimHeight;
    private Vector2 tmpVector;
    private float wheelScrollDirection;
	private bool L, R, U, D;
	private bool isCameraPositionChanged;

    void Awake() 
    {
		currentHeight = startHeight;
		aimHeight = currentHeight;
		isCameraPositionChanged = false;
		controller = new Controller();
		controller.CameraControl.ChangePosition.started += context => PositionChanged();
		mainCamera.GetComponent<Camera>().orthographic = isOrthographic;
    }

	void OnEnable()
	{
		controller.Enable();
	}

	void OnDisable()
	{
		controller.Disable();
	}

	void Start()
	{
		//mainCamera.transform.position = new Vector3(startPoint.position.x, startPoint.position.y, height);
		mainCamera.transform.position = new Vector3(0, 0, currentHeight);
		mainCamera.transform.rotation = Quaternion.Euler(0, 180, 0);
	}

	void PositionChanged()
	{
		isCameraPositionChanged = true;
		print("Changed");
	}

	public void CursorTriggerEnter(string triggerName)
	{
		PositionChanged();
		switch (triggerName)
		{
			case "L":
				L = true;
				break;
			case "R":
				R = true;
				break;
			case "U":
				U = true;
				break;
			case "D":
				D = true;
				break;
		}
	}

	public void CursorTriggerExit(string triggerName)
	{
		switch (triggerName)
		{
			case "L":
				L = false;
				break;
			case "R":
				R = false;
				break;
			case "U":
				U = false;
				break;
			case "D":
				D = false;
				break;
		}
	}

	void Update()
	{
		wheelScrollDirection = controller.CameraControl.CameraZoom.ReadValue<float>();
		if (wheelScrollDirection != 0)
		{
			isCameraPositionChanged = true;
			if (wheelScrollDirection < 0)
			{
				if (aimHeight + stepHeight <= maxHeight)
					aimHeight += stepHeight;
			}
			if (wheelScrollDirection > 0)
			{
				if (aimHeight - stepHeight >= minHeight)
					aimHeight -= stepHeight;
			}
		}

		if (isCameraPositionChanged)
		{
			tmpVector = controller.CameraControl.CameraMove.ReadValue<Vector2>();
			tmpVector.x = L ? -1 : tmpVector.x;
			tmpVector.x = R ? 1 : tmpVector.x;
			tmpVector.y = U ? 1 : tmpVector.y;
			tmpVector.y = D ? -1 : tmpVector.y;

			Vector3 direction = new Vector3(tmpVector.x, tmpVector.y, 0);
			mainCamera.transform.Translate(direction * moveSpeed * Time.deltaTime);

			if (Mathf.Abs(currentHeight - aimHeight) >= 0.01f)
				//tmpHeight = Mathf.Clamp(tmpHeight, minHeight, maxHeight);
				currentHeight = Mathf.Lerp(currentHeight, aimHeight, zoomSpeed * Time.deltaTime);
			else
				currentHeight = aimHeight;

			mainCamera.transform.position = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y, currentHeight);

			if (!L && !R && !U && !D && tmpVector == Vector2.zero && currentHeight == aimHeight)
			{
				isCameraPositionChanged = false;
			}
		}
	}
}
