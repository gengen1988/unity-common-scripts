using UnityEngine;

public class DebugMovement : MonoBehaviour
{
	public float speed = 5f;

	Rigidbody2D rb;

	private void Awake()
	{
		TryGetComponent(out rb);
	}

	private void FixedUpdate()
	{
		float hInput = Input.GetAxisRaw("Horizontal");
		float vInput = Input.GetAxisRaw("Vertical");
		Vector2 intent = new Vector2(hInput, vInput);
		Vector2 velocity = speed * intent.normalized;
		Vector2 displacement = velocity * Time.deltaTime;
		rb.MovePosition(rb.position + displacement);
	}
}