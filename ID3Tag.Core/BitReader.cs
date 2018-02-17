using System;

namespace ID3Tag.Core
{
    public static class BitReader
    {
        private const int BIT_COUNT_IN_BYTE = 8;
        public static bool GetBitAtPosition(byte byteValue, int position)
        {
            if (position < 0 || position > BIT_COUNT_IN_BYTE -1)
                return false;

            return GetBitArrayByByte(byteValue)[position];
        }

        public static bool[] GetBitArrayByByte(byte byteValue)
        {
            bool[] ret = new bool[BIT_COUNT_IN_BYTE];
            int byteIntValue = Convert.ToInt32(byteValue);
            
            for (int i = 0;i < BIT_COUNT_IN_BYTE; i++)
            {
                if ((byteIntValue & (1 << (BIT_COUNT_IN_BYTE -1 - i))) != 0)
                    ret[i] = true;
            }

            return ret;
        }

        public static bool[] GetBitArrayByBytes(byte[] byteValues)
        {
            bool[] ret = new bool[byteValues.Length * BIT_COUNT_IN_BYTE];
            int i = 0;

            foreach (byte byt in byteValues)
            {
                bool[] bits = GetBitArrayByByte(byt);
                for (int j = 0; j < BIT_COUNT_IN_BYTE; j++)
                {
                    if (bits[j])
                        ret[i++] = true;
                    else
                        ret[i++] = false;
                }
            }
            
            return ret;
        }

        public static bool GetBitValueAtPosition(byte byteValue, int index)
        {
            bool ret = false;

           if (index > BIT_COUNT_IN_BYTE - 1 || index < 0)
                return false;

            ret = (byteValue & (1 << index)) != 0;

            return ret;
        }
    }
}
