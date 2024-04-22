using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StartPlayer : MonoBehaviour {

    [SerializeField]
    private Image playerImage;
    public Image PlayerImage => playerImage;

    [SerializeField]
    private Button colorButton;
    public Button ColorButton => colorButton;

    [SerializeField]
    private TMP_Text playerText;
    public TMP_Text PlayerText => playerText;

    [SerializeField]
    private TMP_Text controllerText;
    public TMP_Text ControllerText => controllerText;

    [SerializeField]
    private Button addRemoveButton;
    public Button AddRemoveButton => addRemoveButton;

    [SerializeField]
    private PlayerButton addRemovePlayerButton;
    public PlayerButton AddRemovePlayerButton => addRemovePlayerButton;

    [SerializeField]
    private TMP_Text addRemoveButtonText;
    public TMP_Text AddRemoveButtonText => addRemoveButtonText;

    [SerializeField]
    private Toggle cpuToggle;
    public Toggle CPUToggle => cpuToggle;

    [SerializeField]
    private PlayerToggle cpuPlayerToggle;
    public PlayerToggle CpuPlayerToggle => cpuPlayerToggle;

    [SerializeField]
    private TMP_Dropdown vehicleDropdown;
    public TMP_Dropdown VehicleDropdown => vehicleDropdown;

    [SerializeField]
    private TMP_Text vehicleDropdownLabel;
    public TMP_Text VehicleDropdownLabel => vehicleDropdownLabel;

}