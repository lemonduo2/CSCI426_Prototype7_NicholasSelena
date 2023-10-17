using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public float power = 5f;
    public float maxDragDistance = 2f;
    public Vector3 minScale = new Vector3(0f, 0f, 0f);
    public Vector3 maxScale = new Vector3(0.3f, 0.3f, 0.3f); // Assuming initial scale is (1, 1, 1)
    public LineRenderer powerIndicator;

    private Rigidbody rb;
    private Vector3 startPoint;
    //private Vector3 endPoint;
    private Vector3 direction;
    private bool isDragging = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        powerIndicator.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            OnDragStart();
        }
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            OnDragEnd();
        }

        if (isDragging)
        {
            OnDrag();
        }
        /*if (rb.velocity.magnitude > 0.1f)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, minScale, 0.01f);
        }*/
        if (rb.velocity.magnitude > 0.1f)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, minScale, 0.0001f);
        }
    }

    void OnDragStart()
    {
        startPoint = GetMouseWorldPosition();
        powerIndicator.SetPosition(0, transform.position);
        powerIndicator.enabled = true;
    }

    void OnDrag()
    {
        /*Vector3 currentPoint = GetMouseWorldPosition();
        direction = startPoint - currentPoint;
        float distance = Vector3.Distance(startPoint, currentPoint);
        transform.localScale = Vector3.Lerp(maxScale, minScale, Mathf.Clamp01(distance * 0.1f)); // Adjust the 0.1f as needed*/
        Vector3 currentPoint = GetMouseWorldPosition();
        direction = currentPoint - startPoint; // Invert this for opposite drag direction
        float distance = Mathf.Clamp(Vector3.Distance(startPoint, currentPoint), 0, maxDragDistance);
        powerIndicator.SetPosition(1, transform.position - direction.normalized * distance);
    }

    void OnDragEnd()
    {
        /* endPoint = GetMouseWorldPosition();
         direction.Normalize();

         rb.AddForce(direction * power, ForceMode.Impulse); // Adjust the ForceMode as needed
         transform.localScale = maxScale;*/
        direction.Normalize();

        float distance = Vector3.Distance(powerIndicator.GetPosition(0), powerIndicator.GetPosition(1));
        rb.AddForce(-direction * distance * power, ForceMode.Impulse); // Note the negative sign to invert the direction
        powerIndicator.enabled = false;
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }
}
