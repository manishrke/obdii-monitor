//-----------------------------------------------------------------------
// <copyright file="PollResponse.cs" company="University of Louisville">
//     Copyright (c) 2010 All Rights Reserved
// </copyright>
//-----------------------------------------------------------------------
namespace ObdiiMonitor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Contains logic for handling responses to polls.
    /// </summary>
    public class PollResponse
    {
        /// <summary>
        /// Data that indicates the beginning of the data to be read.
        /// </summary>
        private static char startTag = '\x00EE';

        /// <summary>
        /// Tag that symbolizes a Config File
        /// Not currently implemented.
        /// </summary>
        private static string configTag = "CF";

        /// <summary>
        /// Length of the above start tag, in bytes.
        /// </summary>
        private static int startTagLength = 2;

        /// <summary>
        /// Index of the first byte to decode
        /// </summary>
        private static int constantStart = 8;

        /// <summary>
        /// Reference to controller in MVC pattern
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Type of data being read (for example: "OB")
        /// </summary>
        private string dataType;

        /// <summary>
        /// Length of the poll response, in bytes
        /// </summary>
        private int length;

        /// <summary>
        /// The data in the poll response
        /// </summary>
        private string data;

        /// <summary>
        /// The data in the poll response
        /// </summary>
        private byte[] data2;

        /// <summary>
        /// Time that the data was collected in ms
        /// </summary>
        private uint time;

        /// <summary>
        /// Initializes a new instance of the <see cref="PollResponse"/> class.
        /// </summary>
        /// <param name="controller">The controller reference.</param>
        /// <param name="time">The time data was collected.</param>
        /// <param name="data">The data that was collected.</param>
        /// <param name="dataType">Type of the data.</param>
        /// <param name="length">The length of the data in bytes.</param>
        public PollResponse(Controller controller, uint time, string data, string dataType, int length)
        {
            this.controller = controller;
            this.time = time;
            this.data = data;
            this.dataType = dataType;
            this.length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PollResponse"/> class.
        /// </summary>
        /// <param name="controller">The controller reference.</param>
        /// <param name="bytes">The bytes.</param>
        public PollResponse(Controller controller, byte[] bytes)
        {
            this.controller = controller;

            if (bytes.Length < 2)
            {
                throw new Exception();
            }

            if (bytes[0] != startTag)
            {
                throw new Exception();
            }

            this.length = bytes[1];

            if ((this.length + startTagLength) != bytes.Length)
            {
                throw new Exception();
            }

            this.time = System.BitConverter.ToUInt32(bytes, 2);

            ASCIIEncoding enc = new ASCIIEncoding();

            this.dataType = enc.GetString(bytes, 6, 2);

            if (this.dataType == "OB" || this.dataType == "GP" || this.dataType == "GT" || this.dataType == "TC")
            {
                this.data = enc.GetString(bytes, constantStart, this.length - constantStart + startTagLength);
            }
            else if (this.dataType != "MK" && this.dataType != "NS")
            {
                this.data2 = new byte[this.length - constantStart + startTagLength];
                Array.Copy(bytes, constantStart, this.data2, 0, this.length - constantStart + startTagLength);
                if (this.data2.Length != 3 && this.dataType == "AC")
                {
                    throw new Exception();
                }
            }
        }

        /// <summary>
        /// Gets the start tag.
        /// </summary>
        /// <value>Data that indicates the beginning of the data to be read.</value>
        public static char StartTag
        {
            get { return startTag; }
        }

        /// <summary>
        /// Gets the length of the start tag in bytes.
        /// </summary>
        /// <value>Byte length of start tag.</value>
        public static int StartTagLength
        {
            get { return startTagLength; }
        }

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public string DataType
        {
            get { return this.dataType; }
        }

        /// <summary>
        /// Gets the length of the poll response in bytes.
        /// </summary>
        /// <value>Byte length of the poll response.</value>
        public int Length
        {
            get { return this.length; }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The collected data.</value>
        public string Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The collected data.</value>
        public byte[] Data2
        {
            get { return this.data2; }
        }

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <value>The time data was collected.</value>
        public uint Time
        {
            get { return this.time; }
        }

        /// <summary>
        /// Converts the data.
        /// </summary>
        /// <returns>Converted data.</returns>
        public string ConvertData()
        {
            if ((this.data != null) && (this.data.Length > 2) && (this.dataType == "OB"))
            {
                try
                {
                    return ConvertSensorData.ConvertData(this.data.Substring(0, 2), this.data.Substring(2), this.controller.US);
                }
                catch
                {
                    return null;
                }
            }

            if (this.dataType == "AC")
            {
                return this.controller.AccelerometerConverter.Convert(this.data2).ToString();
            }

            if (this.dataType == "GP" || this.dataType == "TC")
            {
                return this.data;
            }

            return string.Empty;
        }

        /// <summary>
        /// Converts bitstream to byte array.
        /// </summary>
        /// <returns>Converted byte array.</returns>
        public byte[] ToBytes()
        {
            Stream stream = new MemoryStream();

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            stream.Write(System.BitConverter.GetBytes(startTag), 0, 1);

            if (this.dataType == "OB" || this.dataType == "GP" || this.dataType == "GT" || this.dataType == "TC")
            {
                stream.Write(System.BitConverter.GetBytes(this.data.Length + constantStart - startTagLength), 0, 1);
            }
            else if (this.dataType == "MK" || this.dataType == "NS")
            {
                stream.Write(System.BitConverter.GetBytes(constantStart - startTagLength), 0, 1);
            }
            else
            {
                stream.Write(System.BitConverter.GetBytes(this.data2.Length + constantStart - startTagLength), 0, 1);
            }

            stream.Write(System.BitConverter.GetBytes(this.time), 0, System.BitConverter.GetBytes(this.time).Length);

            stream.Write(encoding.GetBytes(this.dataType), 0, encoding.GetBytes(this.dataType).Length);

            if (this.dataType == "OB" || this.dataType == "GP" || this.dataType == "GT" || this.dataType == "TC")
            {
                stream.Write(encoding.GetBytes(this.data), 0, encoding.GetBytes(this.data).Length);
            }
            else if (this.dataType != "MK" && this.dataType != "NS")
            {
                stream.Write(this.data2, 0, this.data2.Length);
            }

            stream.Position = 0;

            byte[] bytes = new byte[startTagLength + this.length];

            stream.Read(bytes, 0, bytes.Length);

            return bytes;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.dataType == "AC")
            {
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] nums = this.data2;
                return this.length + "-" + this.time + "-" + this.dataType + "-" + nums[0] + "." + nums[1] + "." + nums[2];
            }
            else if (this.dataType == "OB" || this.dataType == "GP" || this.dataType == "GT" || this.dataType == "TC")
            {
                return this.length + "-" + this.time + "-" + this.dataType + "-" + this.data;
            }
            else
            {
                return this.length + "-" + this.time + "-" + this.dataType;
            }
        }
    }
}
