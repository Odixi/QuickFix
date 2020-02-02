using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionState
{
    Flight,
    FromPlatformer,
    Platformer,
    ToMenu,
    FromMenu,
    Menu
}

public class CameraPan : MonoBehaviour
{
    public AnimationCurve curve;
    public float TransitionTime = 3;
    private TransitionState state = TransitionState.Menu;
    private float t = 0;
    private Vector3 FlightPosition = new Vector3(7.05f, 30.68f, -10);
    private Vector3 PlatformerPosition = new Vector3(7.05f, -2.64f, -10);
    private Vector3 MenuPosition = new Vector3(7.05f, 13.64f, -10);
    private bool inputReleased = true;
    private Camera camera;
    private Action callback;

    private void Awake()
    {
        camera = Camera.main;
        // For now start from menu
        camera.transform.position = FlightPosition;
    }

    void Update()
    {
        if (state == TransitionState.Flight || state == TransitionState.Platformer || state == TransitionState.Menu)
        {
            return;
        }

        if (state == TransitionState.ToMenu)
        {
            t += Time.deltaTime;
            camera.transform.position = Vector3.Lerp(FlightPosition, MenuPosition,
                curve.Evaluate(Mathf.Min(1, t / TransitionTime)));
            if (t > TransitionTime)
            {
                state = TransitionState.Platformer;
                callback?.Invoke();
                callback = null;
                t = 0;
            }
        }
        if (state == TransitionState.FromMenu)
        {
            t += Time.deltaTime;
            camera.transform.position = Vector3.Lerp(MenuPosition, PlatformerPosition,
                curve.Evaluate(Mathf.Min(1, t / TransitionTime)));
            if (t > TransitionTime)
            {
                state = TransitionState.Platformer;
                callback?.Invoke();
                callback = null;
                t = 0;
            }
        }
        if (state == TransitionState.FromPlatformer)
        {
            t += Time.deltaTime;
            camera.transform.position = Vector3.Lerp(PlatformerPosition, FlightPosition, 
                curve.Evaluate(Mathf.Min(1, t/TransitionTime)));
            if (t > TransitionTime)
            {
                state = TransitionState.Flight;
                callback?.Invoke();
                callback = null;
                t = 0;
            }
        }

    }

    public void SetState(TransitionState state, Action callback) // cb?
    {
        this.state = state;
        this.callback = callback;
    }


}
