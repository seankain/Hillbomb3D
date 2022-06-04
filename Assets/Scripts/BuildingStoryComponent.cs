using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StoryComponentType
{
    Wall,Window,Door,Stair,Balcony,Roof
}
public class BuildingStoryComponent : MonoBehaviour
{
    public float Width = 2f;
    public float Height = 3.5f;
    public StoryComponentType ComponentType = StoryComponentType.Wall;
}
