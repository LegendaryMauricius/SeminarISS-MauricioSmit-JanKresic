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
        Aim,
        Gunner,
        GunnerBinoculars,
        Missile,
        Target,
        TopDown
    };

    GUIStyle buttonStyleNormal, labelStyle;
    //GameObject savedMissile;
    bool dragging;

    State guiState;
    CameraMode cameraMode;
    float fogValue;
    float sunValue;

    public Texture ScopeTexture;
    public Texture BinocularsTexture;

    public Transform SunTransform;
    public Light Sun;
    public GameObject missile;
    public GameObject launcher;
    public GameObject mainCamera;
    //public Camera mainCamera;

    public float fovNormal;
    public float fovBinoculars;
    public float launcherYaw;
    public float launcherPitch;
    public Vector3 GunnerOffset;// = new Vector3(1.5f, 0, 0);
    public Vector3 AimOffset;// = new Vector3(0, 0, 0.5f);
    public float height;

    Vector3 prevGyroEuler;// = new Vector3(0, 0, 0.5f);

    private static Quaternion GyroToUnity(Quaternion q)
    {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    // Start is called before the first frame update
    void Start()
    {
        guiState = State.Setup;
        cameraMode = CameraMode.Aim;
        fogValue = 0.0f;
        sunValue = 1.2f;
        //savedMissile = GameObject.FindGameObjectWithTag("Missile");
        dragging = false;

        prevGyroEuler = GyroToUnity(Input.gyro.attitude).eulerAngles;
        //style = new GUIStyle(GUI.skin.button);
        //GUI.skin.button.fontSize = (int) (0.06f * Mathf.Max(Screen.width, Screen.height));
        //style.
    }

    // Update is called once per frame
    void Update()
    {
        float missileUp = 0.8f, missileBack = 1.5f;

        //GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        //Camera mainCamera = mainCameraObj.GetComponent<Camera>();
        //Debug.Log(mainCameraObj);
        //GameObject launcher = GameObject.FindGameObjectWithTag("Launcher");
        GameObject target = GameObject.FindGameObjectWithTag("MissileTarget");
        //GameObject missile = savedMissile;// GameObject.FindGameObjectWithTag("Missile");
        Quaternion cameraUpRot = Quaternion.Euler(-90, 0, 0);
        Vector3 missileBaseDir = new Vector3(0, 1, 0);
        Vector3 cameraBaseDir = new Vector3(0, 0, 1);
        Vector3 missileFacingDir = missile.transform.rotation * missileBaseDir;

        mainCamera.GetComponent<Camera>().fieldOfView = fovNormal;
        if (guiState == State.SetMissile)
        {
            mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            mainCamera.transform.position = new Vector3(
                launcher.transform.position.x,
                launcher.transform.position.y + height,
                launcher.transform.position.z
            );
        }
        else if (guiState == State.SetTank)
        {
            mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
            mainCamera.transform.position = new Vector3(
                target.transform.position.x,
                target.transform.position.y + height,
                target.transform.position.z
            );
        }
        else
        {
            //mainCamera;
            switch (cameraMode)
            {
                case CameraMode.Aim:
                    mainCamera.GetComponent<Camera>().fieldOfView = fovBinoculars;
                    mainCamera.transform.rotation = launcher.transform.rotation * cameraUpRot;
                    mainCamera.transform.position = launcher.transform.position + 
                        launcher.transform.rotation * AimOffset;

                    RaycastHit hit;
                    Ray ray = new Ray(
                        mainCamera.transform.position,
                        launcher.transform.rotation * missileBaseDir
                    );
                    if (Physics.Raycast(ray, out hit))
                    {
                        missile.GetComponent<missileController>().targetPos = hit.point;
                    }
                    break; 
                case CameraMode.Gunner:
                    mainCamera.transform.rotation = Quaternion.Euler(0, launcherYaw, 0);
                    mainCamera.transform.position = launcher.transform.position + 
                        mainCamera.transform.rotation * GunnerOffset;
                    break;
                case CameraMode.GunnerBinoculars:
                    mainCamera.GetComponent<Camera>().fieldOfView = fovBinoculars;
                    mainCamera.transform.rotation = Quaternion.Euler(0, launcherYaw, 0);
                    mainCamera.transform.position = launcher.transform.position + 
                        mainCamera.transform.rotation * GunnerOffset;
                    break;
                case CameraMode.Target:
                    mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
                    mainCamera.transform.position = new Vector3(
                        target.transform.position.x,
                        target.transform.position.y + height,
                        target.transform.position.z
                    );
                    break;
                case CameraMode.Missile:
                    mainCamera.transform.rotation = Quaternion.FromToRotation(cameraBaseDir, missileFacingDir);
                    mainCamera.transform.position = missile.transform.position - missileFacingDir* missileBack + 
                        mainCamera.transform.rotation * new Vector3(0, 1, 0) * missileUp;
                    break;
                case CameraMode.TopDown:
                    mainCamera.transform.rotation = Quaternion.Euler(90, 0, 0);
                    mainCamera.transform.position = new Vector3(
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
        float aimSpeed = 600.0f / Mathf.Max(Screen.width, Screen.height);

        if (dragging)
        {
            if ((guiState == State.Ready || guiState == State.Launched) && 
                (cameraMode == CameraMode.Aim || cameraMode == CameraMode.Gunner || cameraMode == CameraMode.GunnerBinoculars))
            {
                launcherYaw -= aimSpeed * Input.GetAxis("Mouse X");
                launcherPitch += aimSpeed * Input.GetAxis("Mouse Y");
                launcher.transform.rotation = Quaternion.Euler(launcherPitch, launcherYaw, 0);
            }
            else if (guiState == State.SetMissile)
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
                //Debug.Log("Normal:" + normal);

                //target.transform.position += normal * 1.5f;
                target.transform.rotation = Quaternion.FromToRotation(new Vector3(0, -1, 0), normal);
                target.GetComponent<TankTargetController>().StartingPos = target.transform.position;
                target.GetComponent<TankTargetController>().StartingRot = target.transform.rotation;
            }
        }
        if (Input.GetMouseButtonDown(0) && 
            (guiState == State.SetMissile || guiState == State.SetTank || 
            ((guiState == State.Ready || guiState == State.Launched) && 
            (cameraMode == CameraMode.Aim || cameraMode == CameraMode.Gunner || cameraMode == CameraMode.GunnerBinoculars))))
        {
            dragging = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
        }

        if (guiState == State.Setup || guiState == State.SetMissile || guiState == State.SetTank)
        {
            launcher.transform.position = new Vector3(
                launcher.transform.position.x,
                Terrain.activeTerrain.SampleHeight(launcher.transform.position) + 0.5f,
                launcher.transform.position.z
            );
            target.transform.position = new Vector3(
                target.transform.position.x,
                Terrain.activeTerrain.SampleHeight(target.transform.position) + 1.0f,
                target.transform.position.z
            );
            target.GetComponent<TankTargetController>().reset();
        }

        launcher.transform.rotation = Quaternion.Euler(launcherPitch, launcherYaw, 0);
        target.GetComponent<TankTargetController>().m_Target = launcher.transform.position;

    }

    void OnGUI()
    {
        //GUI.skin.button.fontSize = (int)(0.06f * Mathf.Max(Screen.width, Screen.height));
        GameObject target = GameObject.FindGameObjectWithTag("MissileTarget");
        buttonStyleNormal = new GUIStyle(GUI.skin.button);
        buttonStyleNormal.fontSize = (int)(0.03f * Mathf.Max(Screen.width, Screen.height));
        labelStyle = new GUIStyle(GUI.skin.label);
        labelStyle.fontSize = buttonStyleNormal.fontSize;

        if (guiState == State.Setup)
        {
            if (GUI.Button(leftButtonRect(1), "Set missile", buttonStyleNormal))
            {
                guiState = State.SetMissile;
            }
            if (GUI.Button(leftButtonRect(2), "Set tank", buttonStyleNormal))
            {
                guiState = State.SetTank;
            }
            if (GUI.Button(leftButtonRect(3), "Start sim.", buttonStyleNormal))
            {
                guiState = State.Ready;
                target.GetComponent<TankTargetController>().move();
            }
            GUI.Label(leftButtonRect(4), "Fog strength:", labelStyle);
            fogValue = GUI.HorizontalSlider(leftSliderRect(4), fogValue, 0.0f, 1.0f);
            GUI.Label(leftButtonRect(5), "Time of day:", labelStyle);
            sunValue = GUI.HorizontalSlider(leftSliderRect(5), sunValue, 0.0f, 2 * 3.141592f);
        }
        else if (guiState == State.SetMissile || guiState == State.SetTank)
        {
            if (GUI.Button(leftButtonRect(1), "OK", buttonStyleNormal))
            {
                guiState = State.Setup;
            }
        }
        else 
        {
            if (cameraMode == CameraMode.GunnerBinoculars)
            {
                GUI.DrawTexture(overlayRect(), BinocularsTexture);
            }
            else if (cameraMode == CameraMode.Aim)
            {
                GUI.DrawTexture(overlayRect(), ScopeTexture);
            }
        }

        if (guiState == State.Ready)
        {
            if (cameraMode == CameraMode.Aim)
            {
                if (GUI.Button(leftButtonRect(1), "Launch", buttonStyleNormal))
                {
                    missile.GetComponent<missileController>().launch();
                    guiState = State.Launched;
                }
            }
            if (GUI.Button(leftButtonRect(4), "Sim. setup", buttonStyleNormal))
            {
                guiState = State.Setup;
                //target.GetComponent<TankTargetController>().reset();
            }
        }
        if (guiState == State.Launched || guiState == State.Ready)
        {
            if (GUI.Button(leftButtonRect(3), "Restart", buttonStyleNormal))
            {
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                //GameObject.FindGameObjectWithTag("Missile").GetComponent<missileController>().launch();

                missile.GetComponent<missileController>().reset();
                target.GetComponent<TankTargetController>().reset();
                target.GetComponent<TankTargetController>().move();
                guiState = State.Ready;
            }
        }

        if (guiState == State.Ready || guiState == State.Launched)
        {
            if (GUI.Button(rightButtonRect(1), "Aim", buttonStyleNormal)) {
                cameraMode = CameraMode.Aim;
            }
            if (GUI.Button(rightButtonRect(2), "Gunner", buttonStyleNormal)) {
                cameraMode = CameraMode.Gunner;
            }
            if (GUI.Button(rightButtonRect(3), "Gunner binoculars", buttonStyleNormal))
            {
                cameraMode = CameraMode.GunnerBinoculars;
            }
            if (GUI.Button(rightButtonRect(5), "Sim:Missile", buttonStyleNormal))
            {
                cameraMode = CameraMode.Missile;
            }
            if (GUI.Button(rightButtonRect(6), "Sim:Target", buttonStyleNormal))
            {
                cameraMode = CameraMode.Target;
            }
            if (GUI.Button(rightButtonRect(7), "Sim:Top-down", buttonStyleNormal))
            {
                cameraMode = CameraMode.TopDown;
            }
        }
    }

    Rect leftButtonRect(int index)
    {
        float sizeFactor = Mathf.Max(Screen.width, Screen.height);
        float w = 0.18f * sizeFactor, h = 0.04f * sizeFactor, s = 0.01f * sizeFactor;
        return new Rect(s, s + (index-1)*(h+s), w, h);
    }
    Rect leftSliderRect(int index)
    {
        float sizeFactor = Mathf.Max(Screen.width, Screen.height);
        float w = 0.3f * sizeFactor, h = 0.04f * sizeFactor, s = 0.01f * sizeFactor;
        return new Rect(s, s + (index - 1) * (h + s) + h/3*2, w, h/2);
    }

    Rect rightButtonRect(int index)
    {
        float sizeFactor = Mathf.Max(Screen.width, Screen.height);
        float w = 0.22f * sizeFactor, h = 0.04f * sizeFactor, s = 0.01f * sizeFactor;
        return new Rect(Screen.width - s - w, s + (index - 1) * (h + s), w, h);
    }

    Rect overlayRect()
    {
        float size = Mathf.Max(Screen.width, Screen.height);
        return new Rect(Screen.width/2 - size/2, Screen.height/2 - size/2, size, size);
    }
}
