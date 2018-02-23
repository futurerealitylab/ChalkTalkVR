using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holojam.Network;

internal class KeyEvent : EventPusher {
  public enum Type { DOWN = 0, STAY = 1, UP = 2 }

  public override string Label { get { return "keyEvent"; } }
  public int Key { set { data.ints[0] = value; } }
  public Type EventType { set { data.ints[1] = (int)value; } }
  public override void ResetData() { data = new Flake(0, 0, 0, 2); }

  public void FireKeyDown(int key) {
    Key = key;
    EventType = Type.DOWN;
    this.Push();
  }

  public void FireKeyUp(int key) {
    Key = key;
    EventType = Type.UP;
    this.Push();
  }
}

internal class MouseEvent : EventPusher {

  public enum Type { DOWN = 0, MOVE = 1, UP = 2 }
  public override string Label { get { return "mouseEvent"; } }
  public Type EventType { set { data.ints[0] = (int)value; } }

	//refactor the mouse coordinate

	public Vector2 Position { set {
			data.floats[0] = (value.x+1f)/2f * Chalktalk.Manager.WIDTH;
			data.floats[1] = Chalktalk.Manager.HEIGHT - (value.y +1f)/2f * Chalktalk.Manager.HEIGHT;
	} }
  public override void ResetData() { data = new Flake(0, 0, 2, 1); }

  public void FireMouseDown(Vector2 pos) {
    EventType = Type.DOWN;

    Position = pos;
    this.Push();
  }

  public void FireMouseMove(Vector2 pos) {
    EventType = Type.MOVE;
    Position = pos;
    this.Push();
  }

  public void FireMouseUp(Vector2 pos) {
    EventType = Type.UP;
    Position = pos;
    this.Push();
  }
}
