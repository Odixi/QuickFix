using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPan : MonoBehaviour
{
    public float moveSpeed = 0.1f;
    public AnimationCurve curve;
    private State state = State.Menu;
    private float t = 0;
    private Vector3 FlightPosition = new Vector3(7.05f, 29.64f, -10);
    private Vector3 PlatformerPosition = new Vector3(7.05f, -2.64f, -10);
    private Vector3 MenuPosition = new Vector3(7.05f, 13.64f, -10);
    private bool inputReleased = true;

    enum State
    {
        Flight,
        Platformer,
        Menu
    }

    void Update()
    {
        t = Mathf.Clamp(t + Time.deltaTime * moveSpeed, 0, 1);
        if (Input.GetAxis("P1Vertical") > 0)
        {
            if (inputReleased)
            {
                if (state == State.Menu) ChangeState(State.Flight);
                else if (state == State.Platformer) ChangeState(State.Menu);
                
            }
            inputReleased = false;
        }
        if (Input.GetAxis("P1Vertical") < 0)
        {
            if (inputReleased)
            {
                if (state == State.Flight) ChangeState(State.Menu);
                else if (state == State.Menu) ChangeState(State.Platformer);
            }
            inputReleased = false;
        }
        if (Input.GetAxis("P1Vertical") == 0) inputReleased = true;

        if (state == State.Flight) MoveTowards(FlightPosition);
        if (state == State.Platformer) MoveTowards(PlatformerPosition);
        if (state == State.Menu) MoveTowards(MenuPosition);
    }

    void ChangeState(State s)
    {
        if (state == s) return;
        state = s;
        t = 0;
    }

    // void MoveTowards(Vector3 position) => Camera.main.transform.Translate(Vector3.ClampMagnitude(position - Camera.main.transform.position, Time.deltaTime * moveSpeed));
    void MoveTowards(Vector3 position) => Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, position, curve.Evaluate(t));
    // void MoveTowards(Vector3 position) => Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, position, Time.deltaTime * moveSpeed);
}
