using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TireDebugOutput : MonoBehaviour
{

    public NpcVehicle Target;
    
    public TextMeshProUGUI LeftFrontSlipOutput;
    public TextMeshProUGUI RightFrontSlipOutput;
    public TextMeshProUGUI LeftRearSlipOutput;
    public TextMeshProUGUI RightRearSlipOutput;
    public TextMeshProUGUI BrakeOutput;
    public TextMeshProUGUI MotorOutput;
    public TMP_Dropdown MeshDropdown;

    private Camera camera;
    private MeshCycler meshCycler;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
        MeshDropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(MeshDropdown);
        });
        HandleNewTarget(Target);
    }

    private void DropdownValueChanged(object m_Dropdown)
    {
        for (var i = 0; i < meshCycler.Meshes.Length; i++)
        {
            if(MeshDropdown.options[MeshDropdown.value].text == meshCycler.Meshes[i].BodyMesh.name)
            {
                meshCycler.SetActiveMesh(i);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = camera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    Transform objectHit = hit.transform;

                    var npcVehicle = hit.transform.gameObject.GetComponent<NpcVehicle>();
                    if(npcVehicle != null)
                    {
                        Target = npcVehicle;
                    }
                }
            }

            var wheelHits = Target.GetWheelHitInfo();
            LeftFrontSlipOutput.text = wheelHits[0].forwardSlip.ToString();
            RightFrontSlipOutput.text = wheelHits[1].forwardSlip.ToString();
            LeftRearSlipOutput.text = wheelHits[2].forwardSlip.ToString();
            RightRearSlipOutput.text = wheelHits[3].forwardSlip.ToString();
            BrakeOutput.text = Target.CurrentMotor.ToString();
            MotorOutput.text = Target.CurrentBrake.ToString();
        }
        catch(Exception ex)
        {
            Debug.Log($"Glossing over exceptions in populating debug output: {ex.Message}");
        }
    }

    private void HandleNewTarget(NpcVehicle vehicle)
    {
        Target = vehicle;
        meshCycler = Target.gameObject.GetComponent<MeshCycler>();
        GetBodyMeshOptions();
    }

    private void GetBodyMeshOptions() 
    {
        MeshDropdown.ClearOptions();
        var options = new List<TMP_Dropdown.OptionData>();
        
        foreach(var m in meshCycler.Meshes)
        {
            options.Add(new TMP_Dropdown.OptionData { text = m.BodyMesh.name });
        }
        MeshDropdown.AddOptions(options);
    }
}
