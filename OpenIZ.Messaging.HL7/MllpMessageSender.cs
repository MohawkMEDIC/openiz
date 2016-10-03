/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 *
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: khannan
 * Date: 2016-10-2
 */
using NHapi.Base.Model;
using NHapi.Base.Parser;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace OpenIZ.Messaging.HL7
{
	/// <summary>
	/// MLLP Message Sender
	/// </summary>
	internal class MllpMessageSender
	{
		/// <summary>
		/// The internal reference to the sending endpoint.
		/// </summary>
		private Uri endpoint = null;

		/// <summary>
		/// Gets the last response time of the server.
		/// </summary>
		public TimeSpan LastResponseTime { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MllpMessageSender"/> class 
		/// with a specific endpoint.
		/// </summary>
		/// <param name="endpoint">The endpoint in the form : mllp://ipaddress:port</param>
		public MllpMessageSender(Uri endpoint)
		{
			this.endpoint = endpoint;
		}

		/// <summary>
		/// Send a message and receive the message
		/// </summary>
		public IMessage SendAndReceive(IMessage message)
		{
			// Encode the message
			var parser = new PipeParser();
			string strMessage = string.Empty;
			var id = Guid.NewGuid().ToString();

			strMessage = parser.Encode(message);

			// Open a TCP port
			using (TcpClient client = new TcpClient(AddressFamily.InterNetwork))
			{
				try
				{
					// Connect on the socket
					client.Connect(this.endpoint.Host, this.endpoint.Port);
					DateTime start = DateTime.Now;

					// Get the stream
					using (var stream = client.GetStream())
					{
						// Write message in ASCII encoding
						byte[] buffer = System.Text.Encoding.UTF8.GetBytes(strMessage);
						byte[] sendBuffer = new byte[buffer.Length + 3];

						sendBuffer[0] = 0x0b;

						Array.Copy(buffer, 0, sendBuffer, 1, buffer.Length);
						Array.Copy(new byte[] { 0x1c, 0x0d }, 0, sendBuffer, sendBuffer.Length - 2, 2);

						stream.Write(sendBuffer, 0, sendBuffer.Length);

						// Write end message
						stream.Flush(); // Ensure all bytes get sent down the wire

						// Now read the response
						StringBuilder response = new StringBuilder();

						buffer = new byte[1024];

						while (!buffer.Contains((byte)0x1c)) // HACK: Keep reading until the buffer has the FS character
						{
							int br = stream.Read(buffer, 0, 1024);

							int ofs = 0;

							if (buffer[ofs] == '\v')
							{
								ofs = 1;
								br = br - 1;
							}

							response.Append(Encoding.UTF8.GetString(buffer, ofs, br));
						}

						//Debug.WriteLine(response.ToString());
						// Parse the response
						//this.LastResponseTime = DateTime.Now.Subtract(start);

						return parser.Parse(response.ToString());
					}
				}
				catch (Exception e)
				{
					Debug.WriteLine(e.ToString());
					throw;
				}
			}
		}
	}
}
