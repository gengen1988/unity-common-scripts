using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class RaycastAPITest
{
    // Create a test for the Raycast method
    [MenuItem("Tests / Test Raycast List Clear")]
    private static void TestRaycast()
    {
        // Arrange
        Vector2 origin = Vector2.zero;
        Vector2 direction = Vector2.right;
        ContactFilter2D contactFilter = new ContactFilter2D();
        List<RaycastHit2D> results = new List<RaycastHit2D>
        {
            new RaycastHit2D(),
            new RaycastHit2D()
        };
        float distance = Mathf.Infinity;

        // Act
        int hitCount = Physics2D.Raycast(origin, direction, contactFilter, results, distance);

        // Assert
        if (results.Count == 0)
        {
            Debug.Log("Test Passed: Results list was cleared.");
        }
        else
        {
            Debug.LogError("Test Failed: Results list was not cleared.");
        }
    }

    [MenuItem("Tests / Test Raycast List Order")]
    private static void TestResultOrder()
    {
        // Arrange
        Vector2 origin = Vector2.zero;
        Vector2 direction = Vector2.right;
        ContactFilter2D contactFilter = new ContactFilter2D();
        List<RaycastHit2D> results = new List<RaycastHit2D>();
        float distance = Mathf.Infinity;

        // Act
        int hitCount = Physics2D.Raycast(origin, direction, contactFilter, results, distance);
        Debug.Assert(hitCount > 1, "this test is only work when multiple hit");

        // Assert
        bool isOrdered = true;
        for (int i = 1; i < results.Count; i++)
        {
            if (results[i].distance < results[i - 1].distance)
            {
                isOrdered = false;
                break;
            }
        }

        if (isOrdered)
        {
            Debug.Log("Test Passed: Results list is ordered by distance.");
        }
        else
        {
            Debug.LogError("Test Failed: Results list is not ordered by distance.");
        }
    }
}