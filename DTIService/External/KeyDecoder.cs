﻿using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;

namespace WinProdKeyFind
{
    ///
    /// Original code from mrpeardotnet at GitHub 
    /// https://github.com/mrpeardotnet
    ///
    /// Code page: https://github.com/mrpeardotnet/WinProdKeyFinder/blob/master/WinProdKeyFind/KeyDecoder.cs
    ///

    /// <summary>
    /// Enumartion that specifies DigitalProductId version
    /// </summary>
    public enum DigitalProductIdVersion
    {
        /// <summary>
        /// All systems up to Windows 7 (Windows 7 and older versions)
        /// </summary>
        UpToWindows7,
        /// <summary>
        /// Windows 8 and up (Windows 8 and newer versions)
        /// </summary>
        Windows8AndUp
    }

    /// <summary>
    /// Provides methods to decode Windows Product Key from registry or from DigitalProductId.
    /// This class is static.
    /// </summary>
    public static class KeyDecoder
    {
        public static string GetWindowsProductKeyFromRegistry()
        {
            var localKey =
                RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, Environment.Is64BitOperatingSystem
                    ? RegistryView.Registry64
                    : RegistryView.Registry32);

            var registryKeyValue = localKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")?.GetValue("DigitalProductId");
            if (registryKeyValue == null)
                return "Failed to get DigitalProductId from registry";
            var digitalProductId = (byte[])registryKeyValue;
            localKey.Close();
            var isWin8OrUp =
                Environment.OSVersion.Version.Major == 6 && Environment.OSVersion.Version.Minor >= 2
                ||
                Environment.OSVersion.Version.Major > 6;
            return GetWindowsProductKeyFromDigitalProductId(digitalProductId,
                isWin8OrUp ? DigitalProductIdVersion.Windows8AndUp : DigitalProductIdVersion.UpToWindows7);
        }

        /// <summary>
        /// Decodes Windows Product Key from DigitalProductId with specified DigitalProductId version.
        /// </summary>
        /// <param name="digitalProductId"></param>
        /// <param name="digitalProductIdVersion"></param>
        /// <returns></returns>
        public static string GetWindowsProductKeyFromDigitalProductId(byte[] digitalProductId, DigitalProductIdVersion digitalProductIdVersion)
        {

            var productKey = digitalProductIdVersion == DigitalProductIdVersion.Windows8AndUp 
                ? DecodeProductKeyWin8AndUp(digitalProductId) 
                : DecodeProductKey(digitalProductId);
            return productKey;
        }

        /// <summary>
        /// Decodes Windows Product Key from the DigitalProductId. 
        /// This method applies to DigitalProductId from Windows 7 or lower versions of Windows.
        /// </summary>
        /// <param name="digitalProductId">DigitalProductId to decode</param>
        /// <returns>Decoded Windows Product Key as a string</returns>
        private static string DecodeProductKey(byte[] digitalProductId)
        {
            const int keyStartIndex = 52;
            const int keyEndIndex = keyStartIndex + 15;
            var digits = new[]
            {
                'B', 'C', 'D', 'F', 'G', 'H', 'J', 'K', 'M', 'P', 'Q', 'R',
                'T', 'V', 'W', 'X', 'Y', '2', '3', '4', '6', '7', '8', '9',
            };
            const int decodeLength = 29;
            const int decodeStringLength = 15;
            var decodedChars = new char[decodeLength];
            var hexPid = new ArrayList();
            for (var i = keyStartIndex; i <= keyEndIndex; i++)
            {
                hexPid.Add(digitalProductId[i]);
            }
            for (var i = decodeLength - 1; i >= 0; i--)
            {
                // Every sixth char is a separator.
                if ((i + 1) % 6 == 0)
                {
                    decodedChars[i] = '-';
                }
                else
                {
                    // Do the actual decoding.
                    var digitMapIndex = 0;
                    for (var j = decodeStringLength - 1; j >= 0; j--)
                    {
                        var byteValue = (digitMapIndex << 8) | (byte)hexPid[j];
                        hexPid[j] = (byte)(byteValue / 24);
                        digitMapIndex = byteValue % 24;
                        decodedChars[i] = digits[digitMapIndex];
                    }
                }
            }
            return new string(decodedChars);
        }

        /// <summary>
        /// Decodes Windows Product Key from the DigitalProductId. 
        /// This method applies to DigitalProductId from Windows 8 or newer versions of Windows.
        /// </summary>
        /// <param name="digitalProductId">DigitalProductId to decode</param>
        /// <returns>Decoded Windows Product Key as a string</returns>
        public static string DecodeProductKeyWin8AndUp(byte[] digitalProductId)
        {
            var key = String.Empty;
            const int keyOffset = 52;
            var isWin8 = (byte)((digitalProductId[66] / 6) & 1);
            digitalProductId[66] = (byte)((digitalProductId[66] & 0xf7) | (isWin8 & 2) * 4);

            const string digits = "BCDFGHJKMPQRTVWXY2346789";
            var last = 0;
            for (var i = 24; i >= 0; i--)
            {
                var current = 0;
                for (var j = 14; j >= 0; j--)
                {
                    current = current*256;
                    current = digitalProductId[j + keyOffset] + current;
                    digitalProductId[j + keyOffset] = (byte)(current/24);
                    current = current%24;
                    last = current;
                }
                key = digits[current] + key;
            }

            var keypart1 = key.Substring(1, last);
            var keypart2 = key.Substring(last + 1, key.Length - (last + 1));
            key = keypart1 + "N" + keypart2;

            for (var i = 5; i < key.Length; i += 6)
            {
                key = key.Insert(i, "-");
            }

            return key;
        }

        private static string decodeMicrosoftProductID(byte[] digitalProductId)
        {
            if (digitalProductId == null)
            {
                return "Product key not available.";
            }

            String productkey;
            const int KeyOffset = 52;
            string Chars = "BCDFGHJKMPQRTVWXY2346789";
            try
            {
                byte isWin8 = (byte)((digitalProductId[66] / 6) & 1);
                digitalProductId[66] = (byte)((digitalProductId[66] & 0xF7) | ((isWin8 & 2) * 4));
                int iteration = 24;
                string keyoutput = "";
                int last = 0;
                do
                {
                    int current = 0;
                    int X = 14;
                    do
                    {
                        current = current * 256;
                        current = digitalProductId[X + KeyOffset] + current;
                        digitalProductId[X + KeyOffset] = (byte)(current / 24);
                        current = current % 24;
                        --X;
                    }
                    while (X >= 0);
                    iteration--;
                    keyoutput = Chars.Substring(current, 1) + keyoutput;
                    last = current;
                }
                while (iteration >= 0);
                if (isWin8 == 1)
                {
                    string keypart1 = keyoutput.Substring(1, last);
                    keyoutput = keyoutput.Substring(1);
                    int pos = keyoutput.IndexOf(keypart1, StringComparison.OrdinalIgnoreCase);
                    if (pos > -1)
                    {
                        keyoutput = keyoutput.Substring(0, pos) + keypart1 + "N" + keyoutput.Substring(pos + keypart1.Length);
                    }
                    if (last == 0)
                    {
                        keyoutput = "N" + keyoutput;
                    }
                }
                String a = keyoutput.Substring(0, 5);
                String b = keyoutput.Substring(5, 5);
                String c = keyoutput.Substring(10, 5);
                String d = keyoutput.Substring(15, 5);
                String e = keyoutput.Substring(20, 5);

                productkey = a + "-" + b + "-" + c + "-" + d + "-" + e;
            }
            catch (Exception e)
            {
                productkey = "Unspecified error occured.\n" + e.Message;
            }
            return productkey;
        }
    }

}
