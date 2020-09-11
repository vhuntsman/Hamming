// (c) EchidnaLabs

using System;
using System.Collections.Generic;
using System.Linq;

namespace Hamming {
    /// <summary>
    ///     Encoder decoder for Hamming ECC.
    /// </summary>
    public static class HamEcc
    {
        // TODO: Add 2-bit error detection

        // Reserved indices for checksum - up to 13 bits
        private static readonly int[] ReservedIndices = { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };

        /// <summary>
        ///     Generates the Hamming ECC code for an array of data data.
        /// </summary>
        /// <param name="data">Array of data.</param>
        /// <returns>A 4 byte ECC checksum.</returns>
        public static int GenerateChecksum(byte[] data) {
            // The starting position is 3. Index 0, 1, 2 are reserved for parity checks
            var index = 3;
            var checksumEcc = 0;
            if (data == null) {
                return checksumEcc;
            }

            foreach (var singleByte in data) {
                for (var i = 0; i < 8; i++) {
                    var singleBit = (singleByte >> i) & 0x1;

                    // If index is not in the reserved list
                    if (ReservedIndices.Contains(index)) {
                        index++;
                    }

                    // Exclusive OR each non-zero bit with its index;
                    if (singleBit == 1) {
                        checksumEcc ^= index;
                    }

                    index++;
                }
            }

            return checksumEcc;
        }

        /// <summary>
        ///     Verifies a data block against its ECC checksum.
        /// </summary>
        /// <param name="data">The received data block.</param>
        /// <param name="rcvChecksum">The received checksum.</param>
        /// <returns> Data and checksum error bit position tuple. Returns -1 if there are no errors.</returns>
        public static (int dataErrPos, int eccErrPos) Verify(byte[] data, int rcvChecksum) {
            // Generate checksum first on sender's data
            var calcChecksum = GenerateChecksum(data);
            var errorIndex = calcChecksum ^ rcvChecksum;
            var dataErrIdx = -1;
            var eccErrIdx = -1;

            // Check if the error is found in the reserved index list.
            // If it is, then return the bit position of the ecc checksum
            // If it's not one of the reserved indices, will be 0 (no error).
            var reservedIndexErr = Array.IndexOf(ReservedIndices, errorIndex);
            if (reservedIndexErr != -1) {
                eccErrIdx = reservedIndexErr;
            }
            else if (errorIndex > 0) {
                // Check if error is in the data area instead
                // Use a sorted list to determine where the error bit should be
                List<int> sortedList = new List<int>(ReservedIndices);
                sortedList.Add(errorIndex);
                sortedList.Sort();
                var dataReservedIndex = sortedList.IndexOf(errorIndex);
                if (dataReservedIndex != -1) {
                    // Correct the data error index including the zeroth bit
                    dataErrIdx = errorIndex - (dataReservedIndex + 1);
                }
            }

            return (dataErrIdx, eccErrIdx);
        }

        /// <summary>
        ///     Converts a hexadecimal string to an array of data.
        /// </summary>
        /// <param name="hex">Hexadecimal string.</param>
        /// <returns>Array of data, LSB first (little endian).</returns>
        public static byte[] StringToByteArray(string hex) {
            if (hex == null) {
                return null;
            }

            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .Reverse()
                .ToArray();
        }
    }
}
