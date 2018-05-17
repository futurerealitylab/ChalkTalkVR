// Smoothing.cs

using UnityEngine;

namespace FRL.Utility {
    [System.Serializable]
  public enum SmoothingType {
    None, Linear, Lerp, Accurate, Adaptive, DEFAULTVALUE
  }

  public abstract class Smoother {
    public abstract Vector3 Smooth(Vector3 target, ref Vector3 last, float delta);
    public abstract Quaternion Smooth(Quaternion target, ref Quaternion last, float delta);

    public static Smoother GetSmoother(SmoothingType type) {
      switch (type) {
        case SmoothingType.Linear:
          return new LinearSmoother();
        case SmoothingType.Lerp:
          return new LerpSmoother();
        case SmoothingType.Accurate:
          return new AccurateSmoother();
        case SmoothingType.Adaptive:
          return new AdaptiveSmoother();
        default:
          return null;
      }
    }
  }

  public class LinearSmoother : Smoother {

    public float posStep = 5f;
    public float rotStep = 180f;
    public float jump = 3f;

    public LinearSmoother(float posStep = 5f, float rotStep = 180f, float jump = 3f) {
      this.posStep = posStep;
      this.rotStep = rotStep;
      this.jump = jump;
    }

    public override Vector3 Smooth(Vector3 target, ref Vector3 last, float delta) {
      float dist = Vector3.Distance(target, last);
      //Jump if distance is greater than saved jump.
      if (dist > this.jump) {
        return target;
      }

      return Vector3.MoveTowards(last, target, posStep * delta);
    }

    public override Quaternion Smooth(Quaternion target, ref Quaternion last, float delta) {
      return Quaternion.RotateTowards(last, target, rotStep * Time.deltaTime);
    }
  }


  public class LerpSmoother : Smoother {

    public float posStep = 5f;
    public float rotStep = 180f;
    public float jump = 3f;

    public override Vector3 Smooth(Vector3 target, ref Vector3 last, float delta) {
      float dist = Vector3.Distance(target, last);
      //Jump if distance is greater than saved jump.
      if (dist > this.jump) {
        return target;
      }

      return Vector3.Lerp(last, target, posStep * delta);
    }

    public override Quaternion Smooth(Quaternion target, ref Quaternion last, float delta) {
      return Quaternion.Slerp(last, target, rotStep * Time.deltaTime);
    }
  }


  /// <summary>
  /// Position/rotation smoothing class, optimized for low-latency and jitter (noise) reduction.
  /// Useful for environments where accuracy is more important than interpolation.
  /// </summary>
  [System.Serializable]
  public class AccurateSmoother : Smoother {

    /// <summary>
    /// @internal
    /// Scaling factor for the input signal. Lower values emphasize "snapping".
    /// In the case of position, this is meters. In the case of rotation, the signal
    /// ranges from 0 to 1 (a full 180 degree rotation).
    /// </summary>
    public float cap;

    /// <summary>
    /// @internal
    /// Exponent to apply after scaling. A value of 1 produces linear smoothing.
    /// Higher values flatten the curve, smoothing more when the difference is small.
    /// </summary>
    public float pow;

    public AccurateSmoother(float cap = 0.5f, float pow = 0.5f) {
      this.cap = cap;
      this.pow = pow;
    }

    /// <summary>
    /// Smooth a Vector3 given a reference to its last state. Generally this is called
    /// within an update function.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="last"></param>
    /// <returns>A smoothed version of the input.</returns>
    public override Vector3 Smooth(Vector3 target, ref Vector3 last, float delta) {
      target = Vector3.Lerp(last, target, Mathf.Pow(
        Mathf.Min(1, (last - target).magnitude / cap), pow
      ));
      last = target;
      return target;
    }

    /// <summary>
    /// Smooth a Quaternion given a reference to its last state. Generally this is called
    /// within an update function.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="last"></param>
    /// <returns>A smoothed version of the input.</returns>
    public override Quaternion Smooth(Quaternion target, ref Quaternion last, float delta) {
      float difference = .5f * Quaternion.Dot(last, target) + .5f;
      target = Quaternion.Slerp(last, target, Mathf.Pow(
        Mathf.Min(1, difference / cap), pow
      ));
      last = target;
      return target;
    }
  }

  /// <summary>
  /// Position/rotation smoothing class, optimized for steady animation in
  /// variable-framerate environments.
  /// Adaptively applies more or less smoothing based on update rate.
  /// This has been pre-tuned for general use.
  /// </summary>
  public class AdaptiveSmoother : Smoother {

    /// <summary>
    /// @internal
    /// Range of FPS values covered by the smoothing function.
    /// </summary>
    readonly Vector2 FPS_RANGE = new Vector2(10, 60);

    /// <summary>
    /// @internal
    /// Range of scales to interpolate between. Larger values are more true to the input signal.
    /// </summary>
    readonly Vector2 POSITION_SCALE = new Vector2(10, 30);

    /// <summary>
    /// @internal
    /// Range of scales to interpolate between. Larger values are more true to the input signal.
    /// </summary>
    readonly Vector2 ROTATION_SCALE = new Vector2(10, 30);

    /// <summary>
    /// @internal
    /// Scaling factor for the interpolator. Larger values are jerkier but catch
    /// frame drops or hangs more readily.
    /// </summary>
    const int FACTOR_SCALE = 8;

    /// <summary>
    /// @internal
    /// Don't smooth if the update took longer than this many ms.
    /// </summary>
    const int TIMEOUT = 180; //ms

    /// <summary>
    /// @internal
    /// Don't smooth if the position difference ran above this many meters.
    /// </summary>
    const float DISTANCE_CAP = 1; //m

    /// <summary>
    /// @internal
    /// Don't smooth if the rotation dot ran below this value.
    /// </summary>
    const float DOT_MIN = -.5f;

    float positionFactor, rotationFactor;

    /// <summary>
    /// Smooth a Vector3 given a reference to its last state and a delta time.
    /// Generally this is called within an update function.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="last"></param>
    /// <param name="delta"></param>
    /// <returns>A smoothed version of the input.</returns>
    public override Vector3 Smooth(Vector3 target, ref Vector3 last, float delta) {
      if (delta > TIMEOUT / 1000f || Vector3.Distance(target, last) > DISTANCE_CAP) {
        last = target;
        return target;
      }

      float fps = 1 / delta;
      float newFactor = Mathf.Lerp(
        POSITION_SCALE.x, POSITION_SCALE.y, (fps - FPS_RANGE.x) / (FPS_RANGE.y - FPS_RANGE.x)
      );
      positionFactor = Mathf.Lerp(positionFactor, newFactor, Time.smoothDeltaTime * FACTOR_SCALE);

      last = Vector3.Lerp(last, target, Time.smoothDeltaTime * positionFactor);
      return last;
    }

    /// <summary>
    /// Smooth a Quaternion given a reference to its last state and a delta time.
    /// Generally this is called within an update function.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="last"></param>
    /// <param name="delta"></param>
    /// <returns>A smoothed version of the input.</returns>
    public override Quaternion Smooth(Quaternion target, ref Quaternion last, float delta) {
      if (delta > TIMEOUT / 1000f || Quaternion.Dot(target, last) < DOT_MIN) {
        last = target;
        return target;
      }

      float fps = 1 / delta;
      float newFactor = Mathf.Lerp(
        ROTATION_SCALE.x, ROTATION_SCALE.y, (fps - FPS_RANGE.x) / (FPS_RANGE.y - FPS_RANGE.x)
      );
      rotationFactor = Mathf.Lerp(rotationFactor, newFactor, Time.smoothDeltaTime * FACTOR_SCALE);

      last = Quaternion.Slerp(last, target, Time.smoothDeltaTime * rotationFactor);
      return last;
    }
  }
}
