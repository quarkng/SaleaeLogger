using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleaeAutomationApi
{
    public static class StringHelper
    {

        public static byte[] toByteArray(this String str)
        {
            int count = str.Length;
            char[] char_array = str.ToCharArray();
            byte[] array = new byte[count];
            for (int i = 0; i < count; ++i)
            {
                array[i] = (byte)char_array[i];

            }
            return array;
        }

        public static String ReadString(this System.Net.Sockets.NetworkStream stream)
        {

            int max_length = 128;
            byte[] buffer = new byte[max_length];
            String str = "";
            int bytes_read = 0;
            while (true)
            {
                bytes_read = stream.Read(buffer, 0, max_length);

                for (int i = 0; i < bytes_read; ++i)
                {
                    str += (char)buffer[i];
                }

                if (bytes_read < max_length)
                    break;
            }
            return str;

        }
    }
}
