using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularBuilding : MonoBehaviour
{
    public GameObject GroundFloor;
    public GameObject StoryRoot;
    public List<GameObject> StoryPrefabs;
    public GameObject Roof;
    public List<GameObject> Stories;
    public int StoryCount = 2;
    public float StoryHeight = 4;

    public void SetStories(int storyCount) 
    {
        if (StoryCount > Stories.Count)
        {
            AddStory();
        }
        else
        {
            PopStory();
        }
    }
    private void PopStory()
    {

    }
    private void AddStory() 
    {
        var currentHeight = float.NegativeInfinity;
        foreach(var s in Stories)
        {
            if (s.transform.position.y > currentHeight)
            {
                currentHeight = s.transform.position.y;
            }
        }
        var nextStoryPosition = StoryRoot.transform.position + new Vector3(0, StoryHeight * Stories.Count + 1, 0);
        //TODO random select
        var prefab = Instantiate(StoryPrefabs[0], nextStoryPosition,Quaternion.identity,StoryRoot.transform);
        Stories.Add(prefab);
        Roof.transform.position = new Vector3(Roof.transform.position.x, prefab.transform.position.y + (StoryHeight * 0.5f),Roof.transform.position.z);
           
    }

    // Start is called before the first frame update
    void Start()
    {
        Stories = new List<GameObject>();
        for (var i = 0; i < StoryRoot.transform.childCount; i++)
        {
            Stories.Add(StoryRoot.transform.GetChild(i).gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(StoryCount!= Stories.Count)
        {
            SetStories(StoryCount);
        }
    }
}
