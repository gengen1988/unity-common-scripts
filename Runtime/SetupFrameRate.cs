using UnityEngine;

public class SetupFrameRate : MonoBehaviour
{
	public int fps = 120;

	void Start()
	{
		Execute();
	}

	public void Execute()
	{
		Application.targetFrameRate = fps;
	}
}