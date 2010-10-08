using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ObdiiMonitor
{
    class PollResponse
    {
        public static string startTag = "ZZ";

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
            if (bytes.Length < 3)
                throw new Exception();

            string str = "";

            str += (char)bytes[0];
            str += (char)bytes[1];

            if (str != startTag)
                throw new Exception();

            length = bytes[2];

            if ((length + 9) != bytes.Length)
                throw new Exception();

            time = System.BitConverter.ToInt32(bytes, 3);

            ASCIIEncoding enc = new ASCIIEncoding();

            dataType = enc.GetString(bytes, 7, 2);

            if (dataType == "OB")
            {
                data = enc.GetString(bytes, 9, length);
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

        public Stream ToStream()
        {
            Stream stream = new MemoryStream();

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            stream.Write(encoding.GetBytes(startTag), 0, encoding.GetBytes(startTag).Length);

            stream.Write(System.BitConverter.GetBytes(data.Length), 0, 1);

            stream.Write(System.BitConverter.GetBytes(time), 0, System.BitConverter.GetBytes(time).Length);

            stream.Write(encoding.GetBytes(dataType), 0, encoding.GetBytes(dataType).Length);

            stream.Write(encoding.GetBytes(data), 0, encoding.GetBytes(data).Length);

            stream.Position = 0;

            return stream;
        }

        public override string ToString()
        {
            return startTag + '-' + length + '-' + time + '-' + dataType + '-' + data;
        }


    }
}
