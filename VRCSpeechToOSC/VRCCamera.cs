using System;
using System.Diagnostics;
using Buildetech.OscCore;

namespace VRCSpeechToOSC
{
    /// <summary>
    /// VRChat camera
    /// </summary>
    internal class VRCCamera
    {
        private static OscClient? _oscClient;
        private static OscClient OscClient
        {
            get
            {
                if (_oscClient == null)
                {
                    var host = Config.Instance.TargetHost;
                    var port = Config.Instance.TargetPort;
                    _oscClient = new OscClient(host, port);
                }
                return _oscClient;
            }
        }

        /// <summary>
        /// OSC path: /usercamera/Capture
        /// Take a photo in VRChat.
        /// </summary>
        public static void Shoot()
        {
            Debug.WriteLine("[VRCCamera] Shoot()");
            OscClient.Send("/usercamera/Capture");
        }

        /// <summary>
        /// OSC path: /usercamera/CaptureDelayed
        /// Take a photo with timer in VRChat.
        /// </summary>
        public static void Timer()
        {
            Debug.WriteLine("[VRCCamera] Timer()");
            OscClient.Send("/usercamera/CaptureDelayed");
        }

        /// <summary>
        /// OSC path: /usercamera/Mode
        /// set to 1(photo mode)
        /// </summary>
        public static void Spawn()
        {
            Debug.WriteLine("[VRCCamera] Spawn()");
            OscClient.Send("/usercamera/Mode", 1);
        }

        /// <summary>
        /// OSC path: /usercamera/Mode
        /// set to 2(stream mode)
        /// </summary>
        public static void StreamMode()
        {
            Debug.WriteLine("[VRCCamera] StreamMode()");
            OscClient.Send("/usercamera/Mode", 2);
        }

        /// <summary>
        /// OSC path: /usercamera/Flying
        /// set to true(bool)
        /// </summary>
        public static void Frying()
        {
            Debug.WriteLine("[VRCCamera] Frying()");
            OscClient.Send("/usercamera/Flying", true);
        }
    }
}
