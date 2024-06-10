using UnityEngine;

public class DebugCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) collision 2d enter: {this} - {other.collider}", this);
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) collision 2d exit: {this} - {other.collider}", this);
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) collision 2d stay: {this} - {other.collider}", this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) trigger 2d enter: {this} - {other}", this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) trigger 2d exit: {this} - {other}", this);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) trigger 2d stay: {this} - {other}", this);
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) collision 3d enter: {this} - {other.collider}", this);
    }

    private void OnCollisionExit(Collision other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) collision 3d exit: {this} - {other.collider}", this);
    }

    private void OnCollisionStay(Collision other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) collision 3d stay: {this} - {other.collider}", this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) trigger 3d enter: {this} - {other}", this);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) trigger 3d exit: {this} - {other}", this);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log($"[{Time.frameCount}] ({GetInstanceID()}) trigger 3d stay: {this} - {other}", this);
    }
}