using Citybuilder.Core;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDestinationSetterGrid : MonoBehaviour {

    public float interval;

    private Seeker seeker;

    public void Awake () {
        seeker = GetComponent<Seeker> ();
    }
    private void OnEnable () {
        StartCoroutine (PickRandomGridPoint ());
    }
    private void OnDisable () {
        StopAllCoroutines ();
    }

    private IEnumerator PickRandomGridPoint () {
        while (true) {

            if (World.Current.TileGrid != null) {
                Tile randomTile = World.Current.TileGrid.GetRandom ();
                seeker.StartPath (transform.position, randomTile.WorldPosition, OnPathComplete);
            }
            yield return new WaitForSecondsRealtime (interval);
        }
    }

    private void OnPathComplete (Path p) {
        // We got our path back
        if (p.error) {
            // Nooo, a valid path couldn't be found
        }
        else {
            // Yay, now we can get a Vector3 representation of the path
            // from p.vectorPath
        }
    }
}
