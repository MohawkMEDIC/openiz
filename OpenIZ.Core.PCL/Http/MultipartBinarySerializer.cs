/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * User: justi
 * Date: 2016-11-30
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace OpenIZ.Core.Http
{
    /// <summary>
    /// Represents a multipart attachment
    /// </summary>
    public class MultipartAttachment
    {

        /// <summary>
        /// Creates a new multipart attachment
        /// </summary>
        public MultipartAttachment(byte[] data, string mimeType, String name, bool useFormEncoding = false)
        {
            this.Data = data;
            this.MimeType = mimeType;
            this.Name = name;
            this.UseFormEncoding = useFormEncoding;
        }

        /// <summary>
        /// Gets or sets the mime type of the attachment
        /// </summary>
        public String MimeType { get; set; }

        /// <summary>
        /// Gets or sets the name of the attachment
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Represents the data in the attachment
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// When true instructs the serializer to use form data
        /// </summary>
        public bool UseFormEncoding { get; set; }
    }

    /// <summary>
    /// Mutlipart binary serializer
    /// </summary>
    public class MultipartBinarySerializer : IBodySerializer
    {
        // The content type (used for getting boundary
        private string m_contentType;

        /// <summary>
        /// Gets the content type
        /// </summary>
        public MultipartBinarySerializer(string contentType)
        {
            this.m_contentType = contentType;
        }

        /// <summary>
        /// De-serialize
        /// </summary>
        public object DeSerialize(Stream s)
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serialize
        /// </summary>
        public void Serialize(Stream s, object o)
        {
            // Get the boundary
            var mimeParts = this.m_contentType.Split(';');
            if (mimeParts.Length < 2) throw new InvalidOperationException("Missing mime-boundary marker");
            var boundaryParts = mimeParts[1].Split('=');
            if (boundaryParts.Length < 2 && !boundaryParts[0].Trim().Equals("boundary", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Could not find boundary on content type");

            // Boundary writer
            var attachmentList = o as IList;
            if (attachmentList == null)
                attachmentList = new List<Object>() { o };

            using (StreamWriter sw = new StreamWriter(s))
            {
                foreach (var att in attachmentList)
                {
                    var mimeInfo = att as MultipartAttachment;
                    if (mimeInfo == null)
                        mimeInfo = new MultipartAttachment(att as byte[], "application/octet-stream", "attachment", false);

                    sw.WriteLine("--{0}", boundaryParts[1]);
                    if (mimeInfo.UseFormEncoding)
                        sw.WriteLine("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"", mimeInfo.Name);
                    else
                        sw.WriteLine("Content-Disposition: attachment; filename=\"{0}\"", mimeInfo.Name);
                    sw.WriteLine("Content-Type: {0}", mimeInfo.MimeType);
                    sw.WriteLine();
                    sw.Flush();
                    using (MemoryStream ms = new MemoryStream(mimeInfo.Data))
                        ms.CopyTo(s);
                    sw.WriteLine();
                }
                sw.WriteLine("--{0}--", boundaryParts[1]);
                sw.Flush();
            }

        }
    }
}