# Hamming
This library corrects 1 bit error given a codeword.<br>
Theoretically supports up to 2048 bytes.<br>
Felt inspired to write a Hamming code library after watching the esteemed Grant Sanderson's video on Hamming Codes:

[Part 1](https://www.youtube.com/watch?v=X8jsijhllIA)<br>
[Part 2](https://www.youtube.com/watch?v=b3NxrZOu_CE)

It's a bit re-inventing the wheel, but it was fun to code it.

## Build

Target framework: .NET Core 2.2

## Usage

* Generate an ecc checksum by providing a byte array, e.g.,

int checksum = HamEcc.GenerateChecksum(data);

* Verify the data against checksum. A tuple containing the error positions in the data or checksum will be returned.<br>
(-1, -1) is returned for zero errors, e.g.,

(int dataErrorPos, int eccErrorPos) result = HamEcc.Verify(data, checksum);