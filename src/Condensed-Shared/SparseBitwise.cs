using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Condensed
{
    internal static class SparseBitwise
    {
        // lookup table for the number of bits in the values 0..255.
        // See https://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetTable
        private static readonly byte[] _bitsetTable = {
         0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4,
         1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
         1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
         2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
         1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
         2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
         2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
         3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
         1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5,
         2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
         2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
         3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
         2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6,
         3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
         3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7,
         4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8,
        };

        private static readonly byte[] _posOfFirstCleared = {
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 6,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 7,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 6,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 5,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 4,
         0, 1, 0, 2, 0, 1, 0, 3, 0, 1, 0, 2, 0, 1, 0, 8
        };


        const UInt64 m1 = (UInt64)(0x5555555555555555); // binary: 0101...
        const UInt64 m2 = (UInt64)(0x3333333333333333); // binary: 00110011..
        const UInt64 m4 = (UInt64)(0x0f0f0f0f0f0f0f0f); // binary:  4 zeros,  4 ones ...
        const UInt64 h01 = (UInt64)(0x0101010101010101); // the sum of 256 to the power of 0,1,2,3...

        public static int PositionToOffset(UInt64 bitmap, int position)
        {
            Debug.Assert(position < 64);
            ulong masked = bitmap & ((1UL << position) - 1);

            masked -= (masked >> 1) & m1;             // put count of each 2 bits into those 2 bits
            masked = (masked & m2) + ((masked >> 2) & m2); // put count of each 4 bits into those 4 bits 
            masked = (masked + (masked >> 4)) & m4;        // put count of each 8 bits into those 8 bits 
            return (int)((masked * h01) >> 56);           // returns left 8 bits of x + (x<<8) + (x<<16) + (x<<24)+...

        }

        public static int PositionToOffset(UInt32 bitmap, int position)
        {
            Debug.Assert(position < 32);
            uint masked = bitmap & ((1U << position) - 1);
            masked = masked - ((masked >> 1) & 0x55555555);
            masked = (masked & 0x33333333) + ((masked >> 2) & 0x33333333);
            return (int)((((masked + (masked >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24);
        }


        public static int PositionToOffsetOld(UInt64 bitmap, int position)
        {
            Debug.Assert(position < 64);

            // how many bits are set in positions 0..i-1 of the bitmap?
            int ret = 0;
            // see https://referencesource.microsoft.com/#mscorlib/system/string.cs,8281103e6f23cb5c for example of unsafe byte twiddling.
            // https://docs.microsoft.com/en-us/dotnet/articles/csharp/programming-guide/unsafe-code-pointers/

            unsafe
            {
                byte* bm = (byte*)&bitmap;
                for (; position > 8; position -= 8)                   // bm[0..pos/8-1]
                    ret += _bitsetTable[*bm++];              // chars we want *all* bits in
                return ret + _bitsetTable[*bm & ((1 << position) - 1)];    // char including pos
            }
        }

        public static int PositionToOffsetOld(UInt32 bitmap, int position)
        {
            Debug.Assert(position < 32);

            // how many bits are set in positions 0..i-1 of the bitmap?
            int ret = 0;
            // see https://referencesource.microsoft.com/#mscorlib/system/string.cs,8281103e6f23cb5c for example of unsafe byte twiddling.
            // https://docs.microsoft.com/en-us/dotnet/articles/csharp/programming-guide/unsafe-code-pointers/

            unsafe
            {
                byte* bm = (byte*)&bitmap;
                for (; position > 8; position -= 8)                   // bm[0..pos/8-1]
                    ret += _bitsetTable[*bm++];              // chars we want *all* bits in
                return ret + _bitsetTable[*bm & ((1 << position) - 1)];    // char including pos
            }
        }

        public static int OffsetToPosition(UInt64 bitmap, int offset)
        {
            Debug.Assert(offset < 64);
            int ret = 0;

            unsafe
            {
                byte* bm = (byte*)&bitmap;
                for (int i = 0; i < 8; i++)
                {   // forward scan
                    int pop_count = _bitsetTable[*bm];
                    if (pop_count > offset)
                    {
                        byte last_bm = *bm;
                        for (; offset > 0; offset--)
                        {
                            last_bm &= (byte)(last_bm - 1);  // remove right-most set bit
                        }
                        // Clear all bits to the left of the rightmost bit (the &),
                        // and then clear the rightmost bit but set all bits to the
                        // right of it (the -1).
                        last_bm = (byte)((last_bm & -last_bm) - 1);
                        ret += _bitsetTable[last_bm];
                        return ret;
                    }
                    offset -= pop_count;
                    ret += 8;
                    bm++;
                }
            }
            return ret;

        }

        public static int OffsetToPosition(UInt32 bitmap, int offset)
        {
            Debug.Assert(offset < 32);
            int ret = 0;

            unsafe
            {
                byte* bm = (byte*)&bitmap;
                for (int i = 0; i < 4; i++)
                {   // forward scan
                    int pop_count = _bitsetTable[*bm];
                    if (pop_count > offset)
                    {
                        byte last_bm = *bm;
                        for (; offset > 0; offset--)
                        {
                            last_bm &= (byte)(last_bm - 1);  // remove right-most set bit
                        }
                        // Clear all bits to the left of the rightmost bit (the &),
                        // and then clear the rightmost bit but set all bits to the
                        // right of it (the -1).
                        last_bm = (byte)((last_bm & -last_bm) - 1);
                        ret += _bitsetTable[last_bm];
                        return ret;
                    }
                    offset -= pop_count;
                    ret += 8;
                    bm++;
                }
            }
            return ret;

        }

        public static int PositionOfFirstCleared(UInt64 bitmap)
        {
            int ret = 0;
            unsafe
            {
                byte* bm = (byte*)&bitmap;
                for (int i = 0; i < 8; i++)
                {
                    if (*bm == 0xFF)
                    {
                        ret += 8;
                        bm++;
                    }
                    else
                    {
                        ret += _posOfFirstCleared[*bm];
                        break;
                    }
                }
            }
            return ret;
        }

        public static int PositionOfFirstCleared(UInt32 bitmap)
        {
            int ret = 0;
            unsafe
            {
                byte* bm = (byte*)&bitmap;
                for (int i = 0; i < 4; i++)
                {
                    if (*bm == 0xFF)
                    {
                        ret += 8;
                        bm++;
                    }
                    else
                    {
                        ret += _posOfFirstCleared[*bm];
                        break;
                    }
                }
            }
            return ret;
        }
    }
}