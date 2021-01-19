using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
//using missileController;

public class UIController : MonoBehaviour
{
    enum State
    {
        Setup,
        SetMissile,
        SetTank,
        Ready,
        Launched
    };
    enum CameraMode
    {
        Launcher,
        Missile,
        Target,
        TopDown
    };

    GUIStyle buttonStyleNormal, labelStyle;
    GameObject savedMissile;
    bool dragging;

    State guiState;
    CameraMode cameraMode;
    float fogValue;
    float sunValue;

    public Transform SunTransform;
    public Light Sun;

    // Start is called before the first frame update
    void Start()
    {
        guiState = State.Setup;
        cameraMode = CameraMode.Launcher;
        fogValue = 0.0f;
        sunValue = 0.0f;
        savedMissile = GameObject.FindGameObjectWithTag("Missile");
        dragging = false;
        //style = new GUIStyle(GUI.skin.button);
        //GUI.skin.button.fontSize = (int) (0.06f * Mathf.Max(Screen.width, Screen.height));
        //style.
    }

    // Update is called once per frame
    void Update()
    {
        float height = 30;
        float missileUp = 0.8f, missileBack = 1.5f;

        GameObject camera = GameObject.FindGameObjectWithTag("MainCamera");
        GameObject launcher = GameObject.FindGameObjectWithTag("Launcher");
        GameObject target = GameObject.FindGameObjectWithTag("MissileTarget");
        GameObject missile = savedMissile;// GameObject.FindGameObjectWithTag("Missile");
        Vector3 missileBaseDir = new Vector3(0, 1, 0);
        Vector3 cameraBaseDir = new Vector3(0, 0, 1);
        Vector3 missileFacingDir = missile.transform.rotation * missileBaseDir;

        if (guiState == State.SetMissile)
        {
            camera.transform.rotation = Quaternion.Euler(90, 0, 0);
            camera.transform.position = new Vector3(
                launcher.transform.position.x,
                launcher.transform.position.y + height,
                launcher.transform.position.z
            );
        }
        else if (guiState == State.SetTank)
        {
            camera.transform.rotation = Quaternion.Euler(90, 0, 0);
            camera.transform.position = new Vector3(
                target.transform.position.x,
                target.transform.position.y + height,
                target.transform.position.z
            );
        }
        else
        {
            switch (cameraMode)
            {
                case CameraMode.Launcher:
                    camera.transform.rotation = Quaternion.Euler(0, 0, 0);
                    camera.transform.position = launcher.transform.position + new Vector3(0.8f, 0.5f, -0.7f);
                    break;
                case CameraMode.Target:
                    camera.transform.rotation = Quaternion.Euler(90, 0, 0);
                    camera.transform.position = new Vector3(
                        target.transform.position.x,
                        target.transform.position.y + height,
                        target.transform.position.z
                    );
                    break;
                case CameraMode.Missile:
                    camera.transform.rotation = Quaternion.FromToRotation(cameraBaseDir, missileFacingDir);
                    camera.transform.position = missile.transform.position - missileFacingDir* missileBack + 
                        camera.transform.rotation * new Vector3(0, 1, 0) * missileUp;
                    break;
                case CameraMode.TopDown:
                    camera.transform.rotation = Quaternion.Euler(90, 0, 0);
                    camera.transform.position = new Vector3(
                        missile.transform.position.x,
                        missile.transform.position.y + height,
                        missile.transform.position.z
                    );
                    break;
            }
        }
         
        if (guiState == State.Setup)
        {
            Color eveningColor = Color.Lerp(Color.red, Color.yellow, 0.5f);
            Color noonColor = (Color.cyan + Color.white) / 2;
            Color sunEveningColor = Color.Lerp(Color.red, Color.yellow, 0.3f);
            Color sunNoonColor = (Color.yellow + Color.white) / 2;
            float evH, evS, evV;
            float noH, noS, noV;
            float sevH, sevS, sevV;
            float snoH, snoS, snoV;
            Color.RGBToHSV(eveningColor, out evH, out evS, out evV);
            Color.RGBToHSV(noonColor, out noH, out noS, out noV);
            Color.RGBToHSV(sunEveningColor, out sevH, out sevS, out sevV);
            Color.RGBToHSV(sunNoonColor, out snoH, out snoS, out snoV);
            float redness = Mathf.Abs(Mathf.Cos(sunValue));
            float lightness = Mathf.Max(Mathf.Sin(sunValue*0.9f + 0.1f), 0.0f);

            SunTransform.rotation = Quaternion.AngleAxis(sunValue * Mathf.Rad2Deg, new Vector3(1, 0, 0.2f));
            Sun.intensity = lightness;
            
            Color fogDay = Color.Lerp(Color.white, Color.gray, 0.5f);
            Color fogNight = Color.black;
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = fogValue * fogValue;
            RenderSettings.fogColor = Color.Lerp(fogNight, fogDay, lightness);

        }

        float adjustSpeed = 800.0f / Mathf.Max(Screen.width, Screen.height);

        if (dragging)
        {
            if (guiState == State.SetMissile)
            {
                launcher.transform.position += new Vector3(
                    -adjustSpeed * Input.GetAxis("Mouse X"),
                    0,
                    -adjustSpeed * Input.GetAxis("Mouse Y")
                );
                launcher.transform.position = new Vector3(
                    launcher.transform.position.x,
                    Terrain.activeTerrain.SampleHeight(launcher.transform.position) + 0.5f,
                    launcher.transform.position.z
                );
            }
            else if (guiState == State.SetTank)
            {
                target.transform.position += new Vector3(
                    -adjustSpeed * Input.GetAxis("Mouse X"),
                    0,
                    -adjustSpeed * Input.GetAxis("Mouse Y")
                );
                target.transform.position = new Vector3(
                    target.transform.position.x,
                    Terrain.activeTerrain.SampleHeight(target.transform.position) + 1.0f,
                    target.transform.position.z
                );

                Vector3 cV = new Vector3(
                    0,
                    Terrain.activeTerrain.SampleHeight(target.transform.position),
                    0
                );
                Vector3 xV = new Vector3(
                    1,
                    Terrain.activeTerrain.SampleHeight(target.transform.position + new Vector3(1, 0, 0)),
                    0
                );
                Vector3 zV = new Vector3(
                    0,
                    Terrain.activeTerrain.SampleHeight(target.transform.position + new Vector3(0, 0, 1)),
                    1
                );
                Vector3 normal = Vector3.Cross(xV - cV, zV - cV).normalized;
                Debug.Log("Normal:" + normal);

                //target.transform.position += normal * 1.5f;
                target.transform.rotation = Quaternion.FromToRotation(new Vector3(0, -1, 0), normal);
            }
        }
        if (Input.GetMouseButtonDown(0) && (guiState == State.SetMissile || guiState == State.SetTank))
        {
            dragging = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

    }

    void OnGUI()
    {
        //GUI.skin.button.fontSize = (int)(0.06f * Mathf.Max(Screen.width, Screen.height));
        buttonStyleNormal = new GUIStyle(GUI.skin.button);
        buttonStyleNormal.fontSize = (int)(0.03f * Mathf.Max(Screen.width, Screen.height));
        labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = buttonStyleNormal.fontSize;

        if (guiState == State.Setup)
        {
            if (GUI.Button(leftButtonRect(1), "Start", buttonStyleNormal))
            {
                guiState = State.Ready;
            }
            if (GUI.Button(leftButtonRect(2), "Set missile", buttonStyleNormal))
            {
                guiState = State.SetMissile;
            }
            if (GUI.Button(leftButtonRect(3), "Set tank", buttonStyleNormal))
            {
                guiState = State.SetTank;
            }
            GUI.Label(leftButtonRect(4), "Fog strength:", labelStyle);
            fogValue = GUI.HorizontalSlider(leftSliderRect(4), fogValue, 0.0f, 1.0f);
            GUI.Label(leftButtonRect(5), "Time of day:", labelStyle);
            sunValue = GUI.HorizontalSlider(leftSliderRect(5), sunValue, 0.0f, 2 * 3.141592f);
        }
        if (guiState == State.SetMissile || guiState == State.SetTank)
        {
            if (GUI.Button(leftButtonRect(1), "OK", buttonStyleNormal))
            {
                guiState = State.Setup;
            }
        }
        if (guiState == State.Ready)
        {
            if (GUI.Button(leftButtonRect(1), "Launch", buttonStyleNormal))
            {
                savedMissile.GetComponent<missileController>().launch();
                guiState = State.Launched;
            }
            if (GUI.Button(leftButtonRect(2), "Setup", buttonStyleNormal))
            {
                guiState = State.Setup;
            }
        }
        if (guiState == State.Launched)
        {
            if (GUI.Button(leftButtonRect(1), "Restart", buttonStyleNormal))
            {
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                //GameObject.FindGameObjectWithTag("Missile").GetComponent<missileController>().launch();

                savedMissile.GetComponent<missileController>().reset();
                guiState = State.Ready;
            }
        }

        if (guiState == State.Ready || guiState == State.Launched)
        {
            if (GUI.Button(rightButtonRect(1), "Launcher", buttonStyleNormal))
            {
                cameraMode = CameraMode.Launcher;
            }
            if (GUI.Button(rightButtonRect(2), "Missile", buttonStyleNormal))
            {
                cameraMode = CameraMode.Missile;
            }
            if (GUI.Button(rightButtonRect(3), "Target", buttonStyleNormal))
            {
                cameraMode = CameraMode.Target;
            }
            if (GUI.Button(rightButtonRect(4), "Top-down", buttonStyleNormal))
            {
                cameraMode = CameraMode.TopDown;
            }
        }
    }

    Rect leftButtonRect(int index)
    {
        float sizeFactor = Mathf.Max(Screen.width, Screen.height);
        float w = 0.18f * sizeFactor, h = 0.05f * sizeFactor, s = 0.02f * sizeFactor;
        return new Rect(s, s + (index-1)*(h+s), w, h);
    }
    Rect leftSliderRect(int index)
    {
        float sizeFactor = Mathf.Max(Screen.width, Screen.height);
        float w = 0.3f * sizeFactor, h = 0.05f * sizeFactor, s = 0.02f * sizeFactor;
        return new Rect(s, s + (index - 1) * (h + s) + h/3*2, w, h/2);
    }

    Rect rightButtonRect(int index)
    {
        float sizeFactor = Mathf.Max(Screen.width, Screen.height);
        float w = 0.18f * sizeFactor, h = 0.05f * sizeFactor, s = 0.02f * sizeFactor;
        return new Rect(Screen.width - s - w, s + (index - 1) * (h + s), w, h);
    }
}
