using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugModularBuildingStory : MonoBehaviour
{
    private ModularBuildingStory story;
    // Start is called before the first frame update
    void Start()
    {
        story = GetComponent<ModularBuildingStory>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            story.Render();
        }
    }
}
