using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScanTool
{
    class PollResponse
    {
        private static string startTag = "ZZ";

        private string dataTag;

        public string DataTag
        {
            get { return dataTag; }
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

        private DateTime time;

        public DateTime Time
        {
            get { return time; }
        }

        public PollResponse(DateTime time, string data, string dataTag, int length)
        {
            this.time = time;
            this.data = data;
            this.dataTag = dataTag;
            this.length = length;
        }

        public double convertData()
        {
            return Double.Parse(ConvertSensorData.convert(dataTag, data));
        }

        public override string ToString()
        {
            return startTag + time.TimeOfDay + length + dataTag;
        }
    }
}
