using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chalktalk {
  public class Utility {
    public static int ParsetoInt16(byte[] value, int index) {
      return BitConverter.ToInt16(value, index) < 0 ? BitConverter.ToInt16(value, index) + 0x10000 : BitConverter.ToInt16(value, index);
    }

    public static float ParsetoFloat(int number) {
      return (float)number / 65535f * 2.0f - 1.0f;
    }

    public static Color ParsetoColor(byte[] value, int index) {
      int r, g, b, a;
      r = ParsetoInt16(value, index) >> 8;
      g = ParsetoInt16(value, index) & 0x00ff;
      b = ParsetoInt16(value, index + 2) >> 8;
      a = ParsetoInt16(value, index + 2) & 0x00ff;
      return new Color((float)r / 256f, (float)g / 256f, (float)b / 256f, (float)a / 256f);
    }

    public static List<Vector3> ParsetoVector3s(byte[] value, int index, int size) {
      List<Vector3> rst = new List<Vector3>();
      for (int i = 0; i < size; i++) {
        float x = ParsetoFloat(ParsetoInt16(value, index + i * 6)) * 5;
        float y = ParsetoFloat(ParsetoInt16(value, index + i * 6 + 2)) * 5;
        float z = ParsetoFloat(ParsetoInt16(value, index + i * 6 + 4)) * 5;
        rst.Add(new Vector3(x, y, z));
      }
      return rst;
    }

    public static Vector3 ParsetoVector3(byte[] value, int index, float scale) {
      int ix = ParsetoInt16(value, index);
      float x = ParsetoFloat(ParsetoInt16(value, index)) * scale;
      float y = ParsetoFloat(ParsetoInt16(value, index + 2)) * scale;
      float z = ParsetoFloat(ParsetoInt16(value, index + 4)) * scale;
      return new Vector3(x, y, z);
    }
  }
}

