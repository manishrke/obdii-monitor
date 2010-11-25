// <copyright file="PollResponse.cs" company="University of Louisville">
// Copyright (c) 2010 All Rights Reserved
// </copyright>
// <author>Bradley Schoch</author>
// <author>Nicholas Bell</author>
// <date>2010-10-16</date>
// <summary>Contains logic for handling responses to polls.</summary>
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
    class PollResponse
    {
        /// <summary>
        /// See Controller.cs
        /// TODO: Possibly explain what this actually does.
        /// </summary>
        private Controller controller;

        /// <summary>
        /// Dataword that indicates the beginning of the data to be read.
        /// </summary>
        public static char StartTag = '\x00EE';

        public static string ConfigTag = "CF";

        /// <summary>
        /// Length of the above start tagn, in bytes.
        /// </summary>
        public static int StartTagLength = 2;

        /// <summary>
        /// Index of the first byte to decode
        /// </summary>
        public static int ConstantStart = 8;

        /// <summary>
        /// Type of data being read (for example: "OB")
        /// </summary>
        private string dataType;

        /// <summary>
        /// Gets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public string DataType
        {
            get { return this.dataType; }
        }

        /// <summary>
        /// Length of the poll response, in bytes
        /// </summary>
        private int length;

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get { return this.length; }
        }

        /// <summary>
        /// The data in the poll response
        /// </summary>
        private string data;

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The collected data.</value>
        public string Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// The data in the poll response
        /// </summary>
        private byte[] data2;

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <value>The collected data.</value>
        public byte[] Data2
        {
            get { return this.data2; }
        }

        /// <summary>
        /// Time that the data was collected
        /// TODO: Specify format of time
        /// </summary>
        private uint time;

        /// <summary>
        /// Gets the time.
        /// </summary>
        /// <value>The time data was collected.</value>
        public uint Time
        {
            get { return this.time; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PollResponse"/> class.
        /// </summary>
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
        /// <param name="bytes">The bytes.</param>
        public PollResponse(Controller controller, byte[] bytes)
        {
            this.controller = controller;

            if (bytes.Length < 2)
            {
                throw new Exception();
            }

            if (bytes[0] != StartTag)
            {
                throw new Exception();
            }

            this.length = bytes[1];

            if ((this.length + StartTagLength) != bytes.Length)
            {
                throw new Exception();
            }

            this.time = System.BitConverter.ToUInt32(bytes, 2);

            ASCIIEncoding enc = new ASCIIEncoding();

            this.dataType = enc.GetString(bytes, 6, 2);

            if (this.dataType == "OB" || this.dataType == "GP" || this.dataType == "GT" || this.dataType == "TC")
            {
                this.data = enc.GetString(bytes, ConstantStart, this.length - ConstantStart + StartTagLength);
                if (this.dataType == "TC")
                    controller.TcWindow.Set_Data(this.time, this.data);
            }
            else if (this.dataType != "MK" && this.dataType != "NS")
            {
                data2 = new byte[this.length - ConstantStart + StartTagLength];
                Array.Copy(bytes, ConstantStart, data2, 0, this.length - ConstantStart + StartTagLength);
                if (data2.Length != 3 && this.dataType == "AC")
                    throw new Exception();
            }

        }

        /// <summary>
        /// Converts the data.
        /// </summary>
        /// <returns>Converted data.</returns>
        public string ConvertData()
        {
            if ((this.data != null)&&(this.data.Length > 2) && (this.dataType == "OB"))
            {
                try
                {
                    return ConvertSensorData.convert(this.data.Substring(0, 2), this.data.Substring(2), controller.US);
                }
                catch
                {
                    return null;
                }
            }

            if (this.dataType == "AC")
            {
                return controller.AccelerometerConverver.convert(data2).ToString();
            }

            if (this.dataType == "GP")
            {
                return this.data;
            }

            return "";
        }

        /// <summary>
        /// Converts bitstream to byte array.
        /// </summary>
        /// <returns>Converted byte array.</returns>
        public byte[] ToBytes()
        {
            Stream stream = new MemoryStream();

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            stream.Write(System.BitConverter.GetBytes(StartTag), 0, 1);

            if (this.dataType == "OB" || this.dataType == "GP" || this.dataType == "GT" || this.dataType == "TC")
                stream.Write(System.BitConverter.GetBytes(this.data.Length + ConstantStart - StartTagLength), 0, 1);
            else if (this.dataType == "MK" || this.dataType == "NS")
            {
                stream.Write(System.BitConverter.GetBytes(ConstantStart - StartTagLength), 0, 1);
            }
            else
                stream.Write(System.BitConverter.GetBytes(this.data2.Length + ConstantStart - StartTagLength), 0, 1);


            stream.Write(System.BitConverter.GetBytes(this.time), 0, System.BitConverter.GetBytes(this.time).Length);

            stream.Write(encoding.GetBytes(this.dataType), 0, encoding.GetBytes(this.dataType).Length);

            if (this.dataType == "OB" || this.dataType == "GP" || this.dataType == "GT" || this.dataType == "TC")
                stream.Write(encoding.GetBytes(this.data), 0, encoding.GetBytes(this.data).Length);
            else if(this.dataType != "MK" && this.dataType != "NS")
                stream.Write(this.data2, 0, this.data2.Length);    

            stream.Position = 0;

            byte[] bytes = new byte[StartTagLength + this.length];

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
            if (dataType == "AC")
            {
                ASCIIEncoding enc = new ASCIIEncoding();
                byte[] nums = data2;
                return this.length + "-" + this.time + "-" + this.dataType + "-" + nums[0] + "." + nums[1] + "." + nums[2];
            }
            else if (dataType == "OB" || dataType == "GP" || dataType == "GT" || this.dataType == "TC")
                return this.length + "-" + this.time + "-" + this.dataType + "-" + this.data;
            else
                return this.length + "-" + this.time + "-" + this.dataType;

        }
    }
}
