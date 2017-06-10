using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holojam.Network;

internal class InitUnityCTClient : EventPusher{
	public override string Label { get { return "UnityCTClient"; } }
	public override void ResetData ()
	{
		data = new Flake (0, 0, 2, 1,0,true);
		data.text = "Hello Aaron";
	}
}

internal class onkeydown : EventPusher {
  public override string Label { get { return "onkeydown"; } }

  public int KeyCode {
    set {
      data.ints[0] = value;
    }
  }
    public override void ResetData() {
    data = new Flake(0, 0, 0, 1);
  }
}

internal class onkeyup : EventPusher {
  public override string Label { get { return "onkeyup"; } }

  public int KeyCode {
    set {
      data.ints[0] = value;
    }
  }
  public override void ResetData() {
    data = new Flake(0, 0, 0, 1);
  }
}

internal class onmousedown : EventPusher
{
	
	public override string Label { get { return "onmousedown"; } }

	public Vector2 Position {
		get { return new Vector2 (data.floats [0], data.floats [1]); }
		set {
			data.floats [0] = value.x;
			data.floats [1] = value.y;
      data.ints[0] = 3;
    }
	}

	public override void ResetData ()
	{
		data = new Flake (0, 0, 2, 1);
	}
}

internal class onmouseup : EventPusher
{

	public override string Label { get { return "onmouseup"; } }

	public Vector2 Position {
		get { return new Vector2 (data.floats [0], data.floats [1]); }
		set {
			data.floats [0] = value.x;
			data.floats [1] = value.y;
      data.ints[0] = 3;
    }
	}

	public override void ResetData ()
	{
		data = new Flake (0, 0, 2, 1);
	}
}

internal class onmousemove : EventPusher
{

	public override string Label { get { return "onmousemove"; } }

	public override void ResetData ()
	{
		data = new Flake (0, 0, 2, 1);
	}

	public Vector2 Position {
		get { return new Vector2 (data.floats [0], data.floats [1]); }
		set {
			data.floats [0] = value.x;
			data.floats [1] = value.y;
      data.ints[0] = 3;
		}
	}

}
	

public class ChalkTalkSender : MonoBehaviour
{
	private onmouseup omp;
	private onmousedown omd;
	private onmousemove omm;
  private onkeydown okd;
  private onkeyup oku;
	private InitUnityCTClient client;
	void Awake ()
	{
		omp = gameObject.AddComponent<onmouseup> ();
		omd = gameObject.AddComponent<onmousedown> ();
		omm = gameObject.AddComponent<onmousemove> ();
    okd = gameObject.AddComponent<onkeydown>();
    oku = gameObject.AddComponent<onkeyup>();
		client = gameObject.AddComponent<InitUnityCTClient> ();

	}

	void Start() {
		client.Push ();
	}

	public void sendMouseDown (int b, Vector2 p)
	{
		omd.Position = p;
    //print(p);
		omd.Push ();
	}

	public void sendMouseMove (int b, Vector2 p)
	{
		omm.Position = p;
    //print(p);
    omm.Push ();
	}

	public void sendMouseUp (int b, Vector2 p)
	{
		omp.Position = p;
    //print(p);
    omp.Push ();
	}

  public void sendKeyUp(int b) {
    oku.KeyCode = b;
    //print("send" + b + "up");
    oku.Push();
  }
  public void sendKeyDown(int b) {
    okd.KeyCode = b;
    //print("send" + b + "down");
    okd.Push();
  }
}
