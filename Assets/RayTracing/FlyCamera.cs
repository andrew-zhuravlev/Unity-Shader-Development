using UnityEngine;

public class FlyCamera : MonoBehaviour {

    [SerializeField] float mainSpeed = 100.0f;
    [SerializeField] float shiftAdd = 250.0f;
    float maxShift = 1000.0f;
    [SerializeField] float camSens = 0.25f;

    Vector3 lastMousePosition = new Vector3(255, 255, 255);
    float totalRun = 1.0f;

    void LateUpdate() {
        lastMousePosition = Input.mousePosition - lastMousePosition;
        lastMousePosition = new Vector3(-lastMousePosition.y * camSens, lastMousePosition.x * camSens, 0);
        lastMousePosition = new Vector3(transform.eulerAngles.x + lastMousePosition.x, transform.eulerAngles.y + lastMousePosition.y, 0);
        transform.eulerAngles = lastMousePosition;
        lastMousePosition = Input.mousePosition;

        //Keyboard commands
        float f = 0.0f;
        Vector3 p = GetBaseInput();
        if (Input.GetKey(KeyCode.LeftShift)) {
            totalRun += Time.deltaTime;
            p = p * totalRun * shiftAdd;
            p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
            p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
            p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
        }
        else {
            totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
            p = p * mainSpeed;
        }

        p = p * Time.deltaTime;
        Vector3 newPosition = transform.position;
        if (Input.GetKey(KeyCode.Space)) { //If player wants to move on X and Z axis only
            transform.Translate(p);
            newPosition.x = transform.position.x;
            newPosition.z = transform.position.z;
            transform.position = newPosition;
        }
        else {
            transform.Translate(p);
        }

    }

    Vector3 GetBaseInput() { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W)) {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S)) {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A)) {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D)) {
            p_Velocity += new Vector3(1, 0, 0);
        }
        if (Input.GetKey(KeyCode.E)) {
            p_Velocity += Vector3.up;
        }
        if (Input.GetKey(KeyCode.Q)) {
            p_Velocity += Vector3.down;
        }
        return p_Velocity;
    }
}