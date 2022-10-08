using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    [SerializeField] private GameObject modificationMenu;

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
    [SerializeField] private TextMeshProUGUI inertia;
    [SerializeField] private Slider inertiaSlider;
    [Space(10)]

    [Header("Dash")]
    [SerializeField] private TextMeshProUGUI dashValue;
    [SerializeField] private TMP_InputField dashValueInput;
    [SerializeField] private TextMeshProUGUI dashTimer;
    [SerializeField] private TMP_InputField dashTimerInput;
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

    // Start is called before the first frame update
    void Start()
    {
        modificationMenu.SetActive(false);
        player = FindObjectOfType<PlayerController>();

        horizontalAirSpeed.text =  player.HorizontalAirSpeed.ToString();
        gravityValue.text = player.GravityValue.ToString();
        verticalMaxSpeed.text = player.VerticalMaxSpeed.ToString();
        fallGravityFactor.text = player.FallGravityFactor.ToString();
        fallGravityFactorSlider.value = player.FallGravityFactor;

        groundSpeed.text = player.HorizontalGroundSpeed.ToString();
        inertia.text = player.Inertia.ToString();
        inertiaSlider.value = player.Inertia;

        dashValue.text = player.DashValue.ToString();
        dashBrake.text = player.DashBrake.ToString();
        dashTimer.text = player.DashTimer.ToString();
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

    }

    public void OnCheckBoxChange()
    {
        modificationMenu.SetActive(!(modificationMenu.activeSelf));
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
        player.Inertia = inertiaSlider.value;
        inertia.text = inertiaSlider.value.ToString();
    }

    public void OnDashValueChange()
    {
        player.DashValue = (float)Convert.ToDouble(dashValueInput.text);
        dashValue.text = dashValueInput.text;
    }

    public void OnDashTimerChange()
    {
        player.DashTimer = (float)Convert.ToDouble(dashTimerInput.text);
        dashTimer.text = dashTimerInput.text;
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




}
