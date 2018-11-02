using UnityEngine;

namespace RockVR.Rift.Demo
{
    public class RecordTime : MonoBehaviour
    {
        public static float recordTime = 0;
        private static int hour;
        private static int minute;
        private static int second;
        private static int millisecond;

        public static string SetRecordTime()
        {
            recordTime += Time.deltaTime;
            hour = (int)recordTime / 3600;
            minute = ((int)recordTime - hour * 3600) / 60;
            second = (int)recordTime - hour * 3600 - minute * 60;
            millisecond = (int)((recordTime - (int)recordTime) * 1000);
            return string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}", hour, minute, second, millisecond);
        }
        private void OnApplicationQuit()
        {
            recordTime = 0;
        }
    }
}
