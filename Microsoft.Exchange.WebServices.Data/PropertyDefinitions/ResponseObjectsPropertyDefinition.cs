/*
 * Exchange Web Services Managed API
 *
 * Copyright (c) Microsoft Corporation
 * All rights reserved.
 *
 * MIT License
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this
 * software and associated documentation files (the "Software"), to deal in the Software
 * without restriction, including without limitation the rights to use, copy, modify, merge,
 * publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
 * to whom the Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all copies or
 * substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
 * INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
 * PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
 * FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 */

namespace Microsoft.Exchange.WebServices.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Represents response object property defintion.
    /// </summary>
    internal sealed class ResponseObjectsPropertyDefinition : PropertyDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseObjectsPropertyDefinition"/> class.
        /// </summary>
        /// <param name="xmlElementName">Name of the XML element.</param>
        /// <param name="uri">The URI.</param>
        /// <param name="version">The version.</param>
        internal ResponseObjectsPropertyDefinition(
            string xmlElementName,
            string uri,
            ExchangeVersion version)
            : base(
                xmlElementName,
                uri,
                version)
        {
        }

        /// <summary>
        /// Loads from XML.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="propertyBag">The property bag.</param>
        internal override sealed void LoadPropertyValueFromXml(EwsServiceXmlReader reader, PropertyBag propertyBag)
        {
            ResponseActions value = ResponseActions.None;

            reader.EnsureCurrentNodeIsStartElement(XmlNamespace.Types, this.XmlElementName);

            if (!reader.IsEmptyElement)
            {
                do
                {
                    reader.Read();

                    if (reader.IsStartElement())
                    {
                        value |= GetResponseAction(reader.LocalName);
                    }
                }
                while (!reader.IsEndElement(XmlNamespace.Types, this.XmlElementName));
            }

            propertyBag[this] = value;
        }

        /// <summary>
        /// Gets the response action.
        /// </summary>
        /// <param name="responseActionString">The response action string.</param>
        /// <returns></returns>
        private static ResponseActions GetResponseAction(string responseActionString)
        {
            ResponseActions value = ResponseActions.None;

            switch (responseActionString)
            {
                case XmlElementNames.AcceptItem:
                    value = ResponseActions.Accept;
                    break;
                case XmlElementNames.TentativelyAcceptItem:
                    value = ResponseActions.TentativelyAccept;
                    break;
                case XmlElementNames.DeclineItem:
                    value = ResponseActions.Decline;
                    break;
                case XmlElementNames.ReplyToItem:
                    value = ResponseActions.Reply;
                    break;
                case XmlElementNames.ForwardItem:
                    value = ResponseActions.Forward;
                    break;
                case XmlElementNames.ReplyAllToItem:
                    value = ResponseActions.ReplyAll;
                    break;
                case XmlElementNames.CancelCalendarItem:
                    value = ResponseActions.Cancel;
                    break;
                case XmlElementNames.RemoveItem:
                    value = ResponseActions.RemoveFromCalendar;
                    break;
                case XmlElementNames.SuppressReadReceipt:
                    value = ResponseActions.SuppressReadReceipt;
                    break;
                case XmlElementNames.PostReplyItem:
                    value = ResponseActions.PostReply;
                    break;
            }
            return value;
        }

        /// <summary>
        /// Writes to XML.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="propertyBag">The property bag.</param>
        /// <param name="isUpdateOperation">Indicates whether the context is an update operation.</param>
        internal override void WritePropertyValueToXml(
            EwsServiceXmlWriter writer,
            PropertyBag propertyBag,
            bool isUpdateOperation)
        {
            // ResponseObjects is a read-only property, no need to implement this.
        }

        /// <summary>
        /// Gets a value indicating whether this property definition is for a nullable type (ref, int?, bool?...).
        /// </summary>
        internal override bool IsNullable
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the property type.
        /// </summary>
        public override Type Type
        {
            get { return typeof(ResponseActions); }
        }
    }
}