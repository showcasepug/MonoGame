#region License
// /*
// Microsoft Public License (Ms-PL)
// MonoGame - Copyright � 2009-2010 The MonoGame Team
// 
// All rights reserved.
// 
// This license governs use of the accompanying software. If you use the software, you accept this license. If you do not
// accept the license, do not use the software.
// 
// 1. Definitions
// The terms "reproduce," "reproduction," "derivative works," and "distribution" have the same meaning here as under 
// U.S. copyright law.
// 
// A "contribution" is the original software, or any additions or changes to the software.
// A "contributor" is any person that distributes its contribution under this license.
// "Licensed patents" are a contributor's patent claims that read directly on its contribution.
// 
// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, 
// each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.
// 
// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any contributors' name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, 
// your patent license from such contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution 
// notices that are present in the software.
// (D) If you distribute any portion of the software in source code form, you may do so only under this license by including 
// a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object 
// code form, you may only do so under a license that complies with this license.
// (E) The software is licensed "as-is." You bear the risk of using it. The contributors give no express warranties, guarantees
// or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent
// permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular
// purpose and non-infringement.
// */
#endregion License

#region Using clause
using System;
using Android.Content.PM;

#endregion Using clause

namespace Microsoft.Xna.Framework.Input.Touch
{
    /// <summary>
    /// Allows retrieval of capabilities information from touch panel device.
    /// </summary>
    public struct TouchPanelCapabilities
    {
		private bool hasPressure;
		private bool isConnected;
		private int maximumTouchCount;
        private bool initialized;

        internal void Initialize()
        {
            if (!initialized)
            {
                initialized = true;

                // There does not appear to be a way of finding out if a touch device supports pressure.
                // XNA does not expose a pressure value, so let's assume it doesn't support it.
                hasPressure = false;

#if WINDOWS_STOREAPP
                // Is a touch device present?
                var caps = new Windows.Devices.Input.TouchCapabilities();
                isConnected = caps.TouchPresent != 0;

                // Iterate through all pointer devices and find the maximum number of concurrent touches possible
                maximumTouchCount = 0;
                var pointerDevices = Windows.Devices.Input.PointerDevice.GetPointerDevices();
                foreach (var pointerDevice in pointerDevices)
                    maximumTouchCount = Math.Max(maximumTouchCount, (int)pointerDevice.MaxContacts);
#elif WINDOWS
                maximumTouchCount = GetSystemMetrics(SM_MAXIMUMTOUCHES);
                isConnected = (maximumTouchCount > 0);
#elif ANDROID
                // http://developer.android.com/reference/android/content/pm/PackageManager.html#FEATURE_TOUCHSCREEN
                PackageManager pm = Game.Activity.PackageManager;
                isConnected = pm.HasSystemFeature(PackageManager.FeatureTouchscreen);
                if (pm.HasSystemFeature(PackageManager.FeatureTouchscreenMultitouchJazzhand))
                    maximumTouchCount = 5;
                else if (pm.HasSystemFeature(PackageManager.FeatureTouchscreenMultitouchDistinct))
                    maximumTouchCount = 2;
                else
                    maximumTouchCount = 1;
#else
                isConnected = true;
                maximumTouchCount = 8;
#endif
            }
		}

        public bool HasPressure
        {
            get
            {
                return hasPressure;
            }
        }

        /// <summary>
        /// Returns true if a device is available for use.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return isConnected;
            }
        }

        /// <summary>
        /// Returns the maximum number of touch locations tracked by the touch panel device.
        /// </summary>
        public int MaximumTouchCount
        {
            get
            {
                return maximumTouchCount;
            }
        }

#if WINDOWS
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true)]
        static extern int GetSystemMetrics(int nIndex);

        const int SM_MAXIMUMTOUCHES = 95;
#endif
    }
}