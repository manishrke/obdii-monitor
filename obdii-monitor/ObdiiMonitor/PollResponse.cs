using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ObdiiMonitor
{
    class PollResponse
    {
        public static char START_TAG = '\x00EE';

        public static int CONSTANT_LENGTH = 8;

        private string dataType;

        public string DataType
        {
            get { return dataType; }
        }

        private int length;

        public int Length
        {
            get { return length; }
        }

        private string data;

        public string Data
        {
            get { return data; }
        }

        private int time;

        public int Time
        {
            get { return time; }
        }

        public PollResponse(int time, string data, string dataType, int length)
        {
            this.time = time;
            this.data = data;
            this.dataType = dataType;
            this.length = length;
        }

        public PollResponse(byte[] bytes)
        {
            if (bytes.Length < 2)
                throw new Exception();

            if (bytes[0] != START_TAG)
                throw new Exception();

            length = bytes[1];

            if ((length + CONSTANT_LENGTH) != bytes.Length)
                throw new Exception();

            time = System.BitConverter.ToInt32(bytes, 2);

            ASCIIEncoding enc = new ASCIIEncoding();

            dataType = enc.GetString(bytes, 6, 2);

            if (dataType == "OB")
            {
                data = enc.GetString(bytes, CONSTANT_LENGTH, length);
            }
            else
                throw new Exception();

        }

        public double convertData()
        {
            if ((data.Length > 2)&&(dataType == "OB"))
                return Double.Parse(ConvertSensorData.convert(data.Substring(0,2), data.Substring(2)));

            return Double.Parse(data);
        }

        public byte[] ToBytes()
        {
            Stream stream = new MemoryStream();

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            stream.Write(System.BitConverter.GetBytes(START_TAG), 0, 1);

            stream.Write(System.BitConverter.GetBytes(data.Length), 0, 1);

            stream.Write(System.BitConverter.GetBytes(time), 0, System.BitConverter.GetBytes(time).Length);

            stream.Write(encoding.GetBytes(dataType), 0, encoding.GetBytes(dataType).Length);

            stream.Write(encoding.GetBytes(data), 0, encoding.GetBytes(data).Length);

            stream.Position = 0;

            byte[] bytes = new byte[CONSTANT_LENGTH + length];

            stream.Read(bytes, 0, bytes.Length);

            return bytes;
        }

        public override string ToString()
        {
            return length + "-" + time + "-" + dataType + "-" + data;
        }
    }
}
