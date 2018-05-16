﻿/*
   Copyright 2011 - 2016 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/


using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Logging;
using Redmine.Net.Api.Types;


namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// </summary>
    public static class WebExtensions
    {


        private static string GetExceptionDetails(System.Net.WebException exception)
        {
            string details = null;
            System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)exception.Response;

            using (System.IO.Stream strm = response.GetResponseStream())
            {
                if (strm != null)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(strm))
                    {
                        details = sr.ReadToEnd();
                    } // End Using sr 

                } // End if (strm != null) 

            } // End Using strm 

            return details;
        } // End Function GetExceptionDetails 


        /// <summary>
        /// Handles the web exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="method">The method.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineTimeoutException">Timeout!</exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.NameResolutionFailureException">Bad domain name!</exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.NotFoundException"></exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.InternalServerErrorException"></exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.UnauthorizedException"></exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.ForbiddenException"></exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.ConflictException">The page that you are trying to update is staled!</exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.RedmineException">
        /// </exception>
        /// <exception cref="Redmine.Net.Api.Exceptions.NotAcceptableException"></exception>
        public static void HandleWebException(this System.Net.WebException exception, string method, MimeFormat mimeFormat)
        {
            if (exception == null) return;

            switch (exception.Status)
            {
                case System.Net.WebExceptionStatus.Timeout:
                    throw new RedmineTimeoutException("Timeout!", exception);
                case System.Net.WebExceptionStatus.NameResolutionFailure:
                    throw new NameResolutionFailureException("Bad domain name!", exception);
                case System.Net.WebExceptionStatus.ProtocolError:
                    {
                        System.Net.HttpWebResponse response = (System.Net.HttpWebResponse)exception.Response;
                        switch ((int)response.StatusCode)
                        {
                            case (int)System.Net.HttpStatusCode.NotFound:
                                throw new NotFoundException(response.StatusDescription, exception);

                            case (int)System.Net.HttpStatusCode.InternalServerError:
                                string details = GetExceptionDetails(exception);
                                System.Diagnostics.Debug.WriteLine(details);

                                throw new InternalServerErrorException(response.StatusDescription, exception);

                            case (int)System.Net.HttpStatusCode.Unauthorized:
                                throw new UnauthorizedException(response.StatusDescription, exception);

                            case (int)System.Net.HttpStatusCode.Forbidden:
                                throw new ForbiddenException(response.StatusDescription, exception);

                            case (int)System.Net.HttpStatusCode.Conflict:
                                throw new ConflictException("The page that you are trying to update is staled!", exception);

                            case 422:
                                System.Collections.Generic.List<Error> errors = GetRedmineExceptions(exception.Response, mimeFormat);
                                string message = string.Empty;
                                if (errors != null)
                                {
                                    foreach (Error error in errors)
                                        message = message + error.Info + "\n";
                                }
                                throw new RedmineException(
                                    method + " has invalid or missing attribute parameters: " + message, exception);

                            case (int)System.Net.HttpStatusCode.NotAcceptable:
                                throw new NotAcceptableException(response.StatusDescription, exception);
                        }
                    }
                    break;

                default:
                    throw new RedmineException(exception.Message, exception);
            }
        }

        /// <summary>
        /// Gets the redmine exceptions.
        /// </summary>
        /// <param name="webResponse">The web response.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        private static System.Collections.Generic.List<Error> 
            GetRedmineExceptions(this System.Net.WebResponse webResponse, MimeFormat mimeFormat)
        {
            using (System.IO.Stream dataStream = webResponse.GetResponseStream())
            {
                if (dataStream == null)
                    return null;

                using (System.IO.StreamReader reader = new System.IO.StreamReader(dataStream))
                {
                    string responseFromServer = reader.ReadToEnd();

                    if (string.IsNullOrEmpty(responseFromServer.Trim()))
                        return null;

                    try
                    {
                        Redmine.Net.Api.Types.PaginatedObjects<Error> result = 
                            RedmineSerializer.DeserializeList<Error>(responseFromServer, mimeFormat);

                        return result.Objects;
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Current.Error(ex.Message);
                    }
                }
                return null;
            }
        }
    }
}