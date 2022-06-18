using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ModularBuildingStory : MonoBehaviour
{
    //Max width of the building story
    public float Width = 10f;
    //Max height of the building story
    public float Height = 10f;
    public int StoryIndex = 0;
    [SerializeField]
    private GameObject[] storyPrefabs;
    private BuildingStoryComponent[] storyComponents;
    [SerializeField]
    private GameObject[] groundFloorFillerPrefabs;
    private float[] selectionProbability;
    [SerializeField]
    private StoryComponentSpawnRule[] spawnRules;
    private List<GameObject> prefabInstances;


    void Awake()
    {
        Debug.Log("modular building story awake");
        prefabInstances = new List<GameObject>();
        storyComponents = new BuildingStoryComponent[storyPrefabs.Length];
        selectionProbability = new float[storyPrefabs.Length];
        for (var i = 0; i < storyPrefabs.Length; i++)
        {
            storyComponents[i] = storyPrefabs[i].GetComponent<BuildingStoryComponent>();
            selectionProbability[i] = 1f / (float)storyPrefabs.Length;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //prefabInstances = new List<GameObject>();
        //storyComponents = new BuildingStoryComponent[storyPrefabs.Length];
        //selectionProbability = new float[storyPrefabs.Length];
        //for (var i = 0; i < storyPrefabs.Length; i++)
        //{
        //    storyComponents[i] = storyPrefabs[i].GetComponent<BuildingStoryComponent>();
        //    selectionProbability[i] = 1f / (float)storyPrefabs.Length;
        //}
        //if (StoryIndex == 0)
        //{
        //    GenerateGroundFloor();
        //}
    }

    public void Render()
    {
        for (var i = prefabInstances.Count - 1; i >= 0; i--)
        {
            Destroy(prefabInstances[i]);
        }
        if (StoryIndex == 0)
        {
            GenerateGroundFloor();
        }
        else
        {
            Generate();
        }
    }

    /// <summary>
    /// The ground floor is always going to have to contend with the steep hill angle so it has special considerations.
    /// 
    /// </summary>
    private void GenerateGroundFloor()
    {
        var direction = Vector3.right;
        //what side of the street are we on to know whichway is downhill
        if (gameObject.transform.forward == Vector3.forward)
        {
            direction = Vector3.left;
        }
        float i = 0;
        int placed = 0;
        while (i < Width)
        {
            var index = GetNextComponent(Width - i, Height);
            // wanted to use sin(17) since the angle of the level is fixed at 17 degrees but messed around to get 0.297 instead
            var pos = transform.position + direction * i - (new Vector3(0, 0.297f * i, 0));
            prefabInstances.Add(Instantiate(storyPrefabs[index], transform.position + (direction * i - new Vector3((storyComponents[index].Width / 2), 0, 0)) - (new Vector3(0, 0.297f * i, 0)), Quaternion.identity, transform));
            //the first element establishes the true max height of the first story and shouldnt have filler
            if (placed > 0)
            {
                var filler = Instantiate(groundFloorFillerPrefabs[index], transform.position + direction * i - new Vector3(storyComponents[index].Width / 2, 0, 0) + new Vector3(0, -0.297f * i + Height, 0), Quaternion.identity, transform);
                filler.transform.localScale = new Vector3(1, (0.297f * i), 1);
                prefabInstances.Add(filler);
            }
            placed++;
            i += storyComponents[index].Width;
        }
    }

    private void Generate()
    {
        var direction = Vector3.right;
        //what side of the street are we on to know whichway is downhill
        if (gameObject.transform.forward == Vector3.forward)
        {
            direction = Vector3.left;
        }
        float i = 0;
        while (i < Width)
        {
            var index = GetNextComponent(Width - i, Height);
            prefabInstances.Add(Instantiate(storyPrefabs[index], transform.position + (direction * i - new Vector3((storyComponents[index].Width / 2), 0, 0)), Quaternion.identity, transform));
            i += storyComponents[index].Width;
        }
    }

    private float GetGroundFloorComponentY(int index, float fullWidth, float angle)
    {
        return fullWidth * Mathf.Sin(angle) * index;
    }

    private int GetNextComponent(float widthBudget, float heightBudget)
    {
        var attempts = 0;
        var index = 0;
        while (attempts < 10)
        {
            index = Random.Range(0, storyPrefabs.Length);
            if (storyComponents[index].Width <= widthBudget &&
                storyComponents[index].Height <= heightBudget && selectionProbability[index] > 0)
            {
                UpdateSelectionProbabilities(storyComponents[index]);
                return index;
            }
            attempts++;

        }
        // Just fail and return the first thing and we'll have a bunch of it, should probably default to just generic wall
        return index;
    }

    private void UpdateSelectionProbabilities(BuildingStoryComponent selectedComponent)
    {
        for (var i = 0; i < storyComponents.Length; i++)
        {
            if (storyComponents[i].ComponentType == selectedComponent.ComponentType)
            {
                var rule = GetRule(storyComponents[i].ComponentType);
                selectionProbability[i] -= rule.ProbabilityReductionRate;
            }
        }
    }


    /// <summary>
    /// Get spawn rule for a component type in case it should lower in probability going forward, should probably just be a dictionary
    /// </summary>
    /// <param name="componentType">A <see cref="StoryComponentType"/></param>
    /// <returns>A <see cref="StoryComponentType"/></returns>
    private StoryComponentSpawnRule GetRule(StoryComponentType componentType)
    {
        foreach (var rule in spawnRules)
        {
            if (rule.ComponentType == componentType)
            {
                return rule;
            }
        }
        // give safe default for being a slacker in defining rules
        return new StoryComponentSpawnRule { ComponentType = componentType, ProbabilityReductionRate = 0 };
    }

}

[Serializable]
public class StoryComponentSpawnRule
{
    public StoryComponentType ComponentType;
    public float ProbabilityReductionRate;
}
