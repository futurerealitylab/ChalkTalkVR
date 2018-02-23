using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivityToggler : MonoBehaviour {

  public enum ActiveOn { All, Build, NotBuild, Clients, Master, None }
  public ActiveOn activeOn;
  public int index;
  public GameObject go;
	// Update is called once per frame
	void Update () {
    if (!go) return;

    switch (activeOn) {
      case ActiveOn.All:
        go.SetActive(true);
        break;
      case ActiveOn.Master:
        go.SetActive(Holojam.Tools.BuildManager.IsMasterClient());
        break;
      case ActiveOn.Clients:
        go.SetActive(!Holojam.Tools.BuildManager.IsMasterClient());
        break;
      case ActiveOn.Build:
        go.SetActive(Holojam.Tools.BuildManager.BUILD_INDEX == index);
        break;
      case ActiveOn.NotBuild:
        go.SetActive(Holojam.Tools.BuildManager.BUILD_INDEX != index);
        break;
      case ActiveOn.None:
        go.SetActive(false);
        break;
    }
	}
}
