using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GUIManager : MonoBehaviour
{
    [Header("Menus")]
    [SerializeField] private GameObject modificationMenu;
    [SerializeField] private GameObject feedbacksMenu;
    [SerializeField] private Toggle feedbacksEnable; 
    [SerializeField] private GameObject feedbacksCheckboxGroup;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private Button controlsMenuButton;
    [Space(10)]



    [Header("Air Movement")]
    [SerializeField] private TextMeshProUGUI horizontalAirSpeed;
    [SerializeField] private TMP_InputField horizontalAirSpeedInput;
    [SerializeField] private TextMeshProUGUI gravityValue;
    [SerializeField] private TMP_InputField gravityValueInput;
    [SerializeField] private TextMeshProUGUI verticalMaxSpeed;
    [SerializeField] private TMP_InputField verticalMaxSpeedInput;
    [SerializeField] private TextMeshProUGUI fallGravityFactor;
    [SerializeField] private Slider fallGravityFactorSlider;
    [Space(10)]

    [Header("Ground Movement")]
    [SerializeField] private TextMeshProUGUI groundSpeed;
    [SerializeField] private TMP_InputField groundSpeedInput;
    [SerializeField] private TextMeshProUGUI brakeForce;
    [SerializeField] private Slider brakeForceSlider;
    [SerializeField] private TextMeshProUGUI sprint;
    [SerializeField] private Slider sprintSlider;
    [Space(10)]

    [Header("Dash")]
    [SerializeField] private TextMeshProUGUI dashForce;
    [SerializeField] private TMP_InputField dashForceInput;
    [SerializeField] private TextMeshProUGUI dashDelay;
    [SerializeField] private TMP_InputField DashDelayInput;
    [SerializeField] private TextMeshProUGUI dashBrake;
    [SerializeField] private Slider dashBrakeSlider;
    [Space(10)]

    [Header("Wall Jump")]
    [SerializeField] private TextMeshProUGUI wallGrabDuration;
    [SerializeField] private TMP_InputField wallGrabDurationInput;
    [SerializeField] private TextMeshProUGUI wallFriction;
    [SerializeField] private Slider wallFrictionSlider;
    [Space(10)]

    [Header("Special Platforms")]
    [SerializeField] private TextMeshProUGUI platformClipSpeed;
    [SerializeField] private TMP_InputField platformClipSpeedInput;
    [SerializeField] private TextMeshProUGUI platformBounciness;
    [SerializeField] private Slider platformBouncinessSlider;
    [Space(10)]

    [Header("Jump")]
    [SerializeField] private TextMeshProUGUI verticalImpulse;
    [SerializeField] private TMP_InputField verticalImpulseInput;
    [SerializeField] private TextMeshProUGUI jumpNumber;
    [SerializeField] private TMP_InputField jumpNumberInput;
    [SerializeField] private TextMeshProUGUI jumpTimeTolerance;
    [SerializeField] private TMP_InputField jumpTimeToleranceInput;
    [SerializeField] private TextMeshProUGUI longJumpThreshold;
    [SerializeField] private Slider longJumpThresholdSlider;
    [Space(10)]

    private PlayerController player;
    private Toggle[] feedbackCheckboxToggles;
    private Button[] pauseMenuButtons;
    private int currentSelectedPauseButton;

    private float pauseMenuTimer = .5f;
    private float changeButtonTimer = .1f;
    private float time;
    private float buttonTime;

    // Start is called before the first frame update
    void Start()
    {
        currentSelectedPauseButton = 1;
        buttonTime = 0f;
        pauseMenuButtons = pauseMenu.GetComponentsInChildren<Button>();
        modificationMenu.SetActive(false);
        feedbacksMenu.SetActive(false);
        player = FindObjectOfType<PlayerController>();
        feedbackCheckboxToggles = feedbacksCheckboxGroup.GetComponentsInChildren<Toggle>();

        horizontalAirSpeed.text =  player.HorizontalAirSpeed.ToString();
        gravityValue.text = player.GravityValue.ToString();
        verticalMaxSpeed.text = player.VerticalMaxSpeed.ToString();
        fallGravityFactor.text = player.FallGravityFactor.ToString();
        fallGravityFactorSlider.value = player.FallGravityFactor;

        groundSpeed.text = player.HorizontalGroundSpeed.ToString();
        brakeForce.text = player.BrakeForce.ToString();
        brakeForceSlider.value = player.BrakeForce;
        sprint.text = player.SprintSpeedFactor.ToString();
        sprintSlider.value = player.SprintSpeedFactor;

        dashForce.text = player.DashForce.ToString();
        dashBrake.text = player.DashBrake.ToString();
        dashDelay.text = player.DashDelay.ToString();
        dashBrakeSlider.value = player.DashBrake;

        wallFriction.text = player.WallFriction.ToString();
        wallGrabDuration.text = player.WallGrabDuration.ToString();
        wallFrictionSlider.value = player.WallFriction;

        platformClipSpeed.text = player.WallGrabDuration.ToString();
        platformBounciness.text = player.WallGrabDuration.ToString();
        platformBouncinessSlider.value = player.BouncyPlatformBounciness;

        verticalImpulse.text = player.VerticalImpulse.ToString();
        jumpNumber.text = player.JumpNumber.ToString();
        jumpTimeTolerance.text = player.JumpTimeTolerance.ToString();
        longJumpThreshold.text = player.LongJumpThreshold.ToString();
        longJumpThresholdSlider.value = player.LongJumpThreshold;

    }

    // Update is called once per frame
    void Update()
    {
        if ((modificationMenu.activeSelf || feedbacksMenu.activeSelf) && !pauseMenu.activeSelf && !controlsMenu.activeSelf)
            EventSystem.current.SetSelectedGameObject(null);
        if ( buttonTime < 5f) buttonTime += Time.unscaledDeltaTime;
        if (Input.GetAxisRaw("Pause") > 0 && Time.unscaledTime > pauseMenuTimer + time)
        {
            time = Time.unscaledTime;
            if (pauseMenu.activeSelf) OnResume();
            else OnPause();
        }
        if (pauseMenu.activeSelf)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 && buttonTime > changeButtonTimer)
            {
                int dir = (int)Input.GetAxisRaw("Horizontal");
                buttonTime = 0f;
                EventSystem.current.SetSelectedGameObject(null);
                if (dir < 0 && currentSelectedPauseButton == 0) currentSelectedPauseButton = pauseMenuButtons.Length - 1;
                else currentSelectedPauseButton = (currentSelectedPauseButton + dir) % pauseMenuButtons.Length;
                EventSystem.current.SetSelectedGameObject(pauseMenuButtons[currentSelectedPauseButton].gameObject);
            }
        }
    }

    public void OnModificationMenuCheckboxChange()
    {
        modificationMenu.SetActive(!(modificationMenu.activeSelf));
    }

    public void OnFeedbacksMenuCheckboxChange()
    {
        feedbacksMenu.SetActive(!(feedbacksMenu.activeSelf));
    }

    public void OnHorizontalAirSpeedChange()
    {
        player.HorizontalAirSpeed = (float)Convert.ToDouble(horizontalAirSpeedInput.text);
        horizontalAirSpeed.text = horizontalAirSpeedInput.text;
    }

    public void OnGravityValueChange()
    {
        player.GravityValue = (float)Convert.ToDouble(gravityValueInput.text);
        gravityValue.text = gravityValueInput.text;
    }

    public void OnVerticalMaxSpeedChange()
    {
        player.VerticalMaxSpeed = (float)Convert.ToDouble(verticalMaxSpeedInput.text);
        verticalMaxSpeed.text = verticalMaxSpeedInput.text;
    }

    public void OnFallGavityFactorChange()
    {
        player.FallGravityFactor = fallGravityFactorSlider.value;
        fallGravityFactor.text = fallGravityFactorSlider.value.ToString();
    }

    public void OnGroundSpeedChange()
    {
        player.HorizontalGroundSpeed = (float)Convert.ToDouble(groundSpeedInput.text);
        groundSpeed.text = groundSpeedInput.text;
    }

    public void OnInertiaChange()
    {
        player.BrakeForce = brakeForceSlider.value;
        brakeForce.text = brakeForceSlider.value.ToString();
    }

    public void OnDashValueChange()
    {
        player.DashForce = (float)Convert.ToDouble(dashForceInput.text);
        dashForce.text = dashForceInput.text;
    }

    public void OnDashTimerChange()
    {
        player.DashDelay = (float)Convert.ToDouble(DashDelayInput.text);
        dashDelay.text = DashDelayInput.text;
    }

    public void OnDashBrakeChange()
    {
        player.DashBrake = dashBrakeSlider.value;
        dashBrake.text = dashBrakeSlider.value.ToString();
    }

    public void OnWallGrabDurationChange()
    {
        player.WallGrabDuration = (float)Convert.ToDouble(wallGrabDurationInput.text);
        wallGrabDuration.text = wallGrabDurationInput.text;
    }

    public void OnWallFrictionChange()
    {
        player.WallFriction = wallFrictionSlider.value;
        wallFriction.text = wallFrictionSlider.value.ToString();
    }

    public void OnPlatformClipSpeedChange()
    {
        player.PlatformClipSpeed = (float)Convert.ToDouble(platformClipSpeedInput.text);
        platformClipSpeed.text = platformClipSpeedInput.text;
    }

    public void OnPlatformBouncinessChange()
    {
        player.BouncyPlatformBounciness = platformBouncinessSlider.value;
        platformBounciness.text = platformBouncinessSlider.value.ToString();
    }

    public void OnVerticalImpulseChange()
    {
        player.VerticalImpulse = (float)Convert.ToDouble(verticalImpulseInput.text);
        verticalImpulse.text = verticalImpulseInput.text;
    }

    public void OnJumpNumberChange()
    {
        player.JumpNumber = (int)Convert.ToDouble(jumpNumberInput.text);
        jumpNumber.text = jumpNumberInput.text;
    }

    public void OnJumpTimeToleranceChange()
    {
        player.JumpTimeTolerance = (float)Convert.ToDouble(jumpTimeToleranceInput.text);
        jumpTimeTolerance.text = jumpTimeToleranceInput.text;
    }

    public void OnLongJumpThresholdChange()
    {
        player.LongJumpThreshold = longJumpThresholdSlider.value;
        longJumpThreshold.text = longJumpThresholdSlider.value.ToString();  
    }

    public void OnSprintValueChange()
    {
        player.SprintSpeedFactor = sprintSlider.value;
        sprint.text = sprintSlider.value.ToString();
    }

    public void OnFeedbackEnableChange()
    {
        foreach (var toggle in feedbackCheckboxToggles)
        {
            toggle.isOn = feedbacksEnable.isOn;
        }
    }

    public void OnControlsMenu()
    {
        pauseMenu.SetActive(false);
        controlsMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(controlsMenuButton.gameObject);
    }

    public void OnPause()
    {
        currentSelectedPauseButton = 1;
        Time.timeScale = 0f;
        controlsMenu.SetActive(false);
        pauseMenu.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(pauseMenuButtons[currentSelectedPauseButton].gameObject);

    }

    public void OnResume()
    {
        EventSystem.current.SetSelectedGameObject(null);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void LoadMainMenu()
    {
        OnResume();
        StartCoroutine(loadMainMenu());
    }

    private IEnumerator loadMainMenu()
    {
        yield return SceneManager.LoadSceneAsync(0);
    }




}
