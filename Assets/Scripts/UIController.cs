using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
//using missileController;

public class UIController : MonoBehaviour
{
    enum State
    {
        Setup,
        Ready,
        Launched
    };

    private State guiState;

    // Start is called before the first frame update
    void Start()
    {
        guiState = State.Setup;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if (guiState == State.Setup)
        {
            if (GUI.Button(leftButtonRect(1), "Start"))
            {
                guiState = State.Ready;
            }
        }
        if (guiState == State.Ready)
        {
            if (GUI.Button(leftButtonRect(1), "Launch"))
            {
                GameObject.FindGameObjectWithTag("Missile").GetComponent<missileController>().launch();
                guiState = State.Launched;
            }
        }
        if (guiState == State.Launched)
        {
            if (GUI.Button(leftButtonRect(1), "Restart"))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                GameObject.FindGameObjectWithTag("Missile").GetComponent<missileController>().launch();
                guiState = State.Launched;
            }
        }

        if (guiState == State.Ready || guiState == State.Launched)
        {
            if (GUI.Button(rightButtonRect(1), "Launcher"))
            {
                GameObject.FindGameObjectWithTag("Missile").GetComponent<missileController>().launch();
                guiState = State.Launched;
            }
            if (GUI.Button(rightButtonRect(2), "Missile"))
            {
                GameObject.FindGameObjectWithTag("Missile").GetComponent<missileController>().launch();
                guiState = State.Launched;
            }
            if (GUI.Button(rightButtonRect(3), "Target"))
            {
                GameObject.FindGameObjectWithTag("Missile").GetComponent<missileController>().launch();
                guiState = State.Launched;
            }
            if (GUI.Button(rightButtonRect(4), "Top-down"))
            {
                GameObject.FindGameObjectWithTag("Missile").GetComponent<missileController>().launch();
                guiState = State.Launched;
            }
        }
    }

    Rect leftButtonRect(int index)
    {
        float sizeFactor = Mathf.Max(Screen.width, Screen.height);
        float w = 0.1f * sizeFactor, h = 0.05f * sizeFactor, s = 0.02f * sizeFactor;
        return new Rect(s, s + (index-1)*(h+s), w, h);
    }

    Rect rightButtonRect(int index)
    {
        float sizeFactor = Mathf.Max(Screen.width, Screen.height);
        float w = 0.1f * sizeFactor, h = 0.05f * sizeFactor, s = 0.02f * sizeFactor;
        return new Rect(Screen.width - s - w, s + (index - 1) * (h + s), w, h);
    }
}
