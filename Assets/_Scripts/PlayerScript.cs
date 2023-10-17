using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public float power = 5f;
    public float maxDragDistance = 2f;
    public Vector3 minScale = new Vector3(0.05f, 0.05f, 0.05f);
    public Vector3 maxScale = new Vector3(0.3f, 0.3f, 0.3f); // Assuming initial scale is (1, 1, 1)
    public LineRenderer powerIndicator;

    private Rigidbody rb;
    private Vector3 startPoint;
    //private Vector3 endPoint;
    private Vector3 direction;
    private bool isDragging = false;
    public float lineRendererZOffset = 0.01f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        powerIndicator.enabled = false;
    }

    private void Update()
    {
        if (transform.localScale == minScale)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;  // Exit out of Update for this frame
        }

        // Only allow shooting if the ball is almost stationary
        if (rb.velocity.magnitude < 0.1f)
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
        }

        if (isDragging)
        {
            OnDrag();
        }

        if (rb.velocity.magnitude > 0.2f)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, minScale, 0.00007f);
        }
    }

    void OnDragStart()
    {
        startPoint = GetMouseWorldPosition();
        Vector3 adjustedStartPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + lineRendererZOffset);
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
        Vector3 adjustedEndPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + lineRendererZOffset) - direction.normalized * distance;
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

        int layerMask = ~(LayerMask.GetMask("Trees", "Chalet", "Fence", "Border"));

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            return hit.point;
        }

        Plane playerDepthPlane = new Plane(Vector3.up, transform.position); // This assumes Z is your depth axis. Adjust if necessary.
        float distanceToPlane;
        if (playerDepthPlane.Raycast(ray, out distanceToPlane))
        {
            return ray.GetPoint(distanceToPlane);
        }

        return Vector3.zero;
    }
}
