using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugModularBuilding : MonoBehaviour
{
    private ModularBuilding building;
    // Start is called before the first frame update
    void Start()
    {
        building = GetComponent<ModularBuilding>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            building.Render();
        }
    }
}
