using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update

    public int ghostType;
    [HideInInspector]
    public int ghostPointValue;
    [HideInInspector]
    public AudioClip ghostSFX;
    [HideInInspector]
    public Animation ghostAnimation;
    [HideInInspector]
    public float ghostVisibilityTime;

    [HideInInspector]
    public SpawnLocation currentLocation;

    private void Start()
    {
        currentLocation.occupied = true;
    }

    public void PlayAnimation()
    {

    }

    public void PlaySFX()
    {

    }



}
