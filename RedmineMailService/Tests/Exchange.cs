
using Microsoft.Exchange.WebServices.Data;


namespace RedmineMailService
{


    // https://github.com/sherlock1982/ews-managed-api
    // https://msdn.microsoft.com/en-us/library/office/dn567668(v=exchg.150).aspx
    // https://msdn.microsoft.com/en-us/library/office/dn495632(v=exchg.150).aspx
    public class Exchange
    {


        private class NoTrace : Microsoft.Exchange.WebServices.Data.ITraceListener
        {
            void ITraceListener.Trace(string traceType, string traceMessage)
            {
                // throw new NotImplementedException();
            }
        }



        private static void SetBasicHeader(ExchangeService ews, string username, string password)
        {
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes($"{username}:{password}");
            string headerValue = "Basic " + System.Convert.ToBase64String(bytes);
            ews.HttpHeaders.Add("Authorization", headerValue);
        }


        private static void SetNtlmHeader(ExchangeService ews, string username, string password)
        {
            var cache = new System.Net.CredentialCache();
            cache.Add(ews.Url, "NTLM", new System.Net.NetworkCredential(username, password));
            ews.Credentials = cache;
        }
        

        private static bool RedirectionUrlValidationCallback(string redirectionUrl)
        {
            // The default for the validation callback is to reject the URL.
            bool result = false;

            System.Uri redirectionUri = new System.Uri(redirectionUrl);

            // Validate the contents of the redirection URL. In this simple validation
            // callback, the redirection URL is considered valid if it is using HTTPS
            // to encrypt the authentication credentials. 
            if (redirectionUri.Scheme == "https")
            {
                result = true;
            } // End if (redirectionUri.Scheme == "https") 

            return result;
        } // End Function RedirectionUrlValidationCallback 


        public static ExchangeService GetService(ExchangeVersion version)
        {
            ExchangeService service = new ExchangeService(version);
            service.WebProxy = new System.Net.WebProxy("127.0.0.1", 8000);

            // service.WebProxy
            service.PreAuthenticate = true;
            service.UseDefaultCredentials = false;
            service.Credentials = new WebCredentials(Trash.UserData.Email, Trash.UserData.Password);

            // Workaround: NTLM doesn't work...
            // and then just set the correct header yourself.
            // SetBasicHeader(service, Trash.UserData.Email, Trash.UserData.Password);
            // SetNtlmHeader(service, Trash.UserData.Email, Trash.UserData.Password);

            Microsoft.Exchange.WebServices.Data.ITraceListener listener = new NoTrace();
            listener = null;

            if (listener != null)
            {
                service.TraceListener = listener;
                service.TraceFlags = TraceFlags.All;
                service.TraceEnabled = true;
            } // End if (listener != null) 

            service.TraceFlags = TraceFlags.All;
            service.TraceEnabled = true;

            // service.Url = new System.Uri("https://webmail.somedomain.com/ews/exchange.asmx");
            // nslookup -type=srv _autodiscover._tcp.somedomain.com
            // nslookup -type=srv _autodiscover._tcp.somedomain.local
            service.AutodiscoverUrl(RedmineMailService.Trash.UserData.Email, RedirectionUrlValidationCallback);

            return service;
        }


        public static ExchangeService GetService()
        {
            return GetService(ExchangeVersion.Exchange2007_SP1);
        }



        public static void GetInbox(ExchangeService service)
        {
            Folder inbox = Folder.Bind(service, WellKnownFolderName.Inbox);
        } // End Sub ListAllMails 


        /// <summary>
        /// Finds the first email message and initiates an attempt to call a phone number and 
        /// dictate the contents of the email message. This sample requires that Unified Messaging
        /// is enabled for the caller.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        public static void PlayEmailOnPhone(ExchangeService service)
        {

            /// <summary>
            /// Callback method for refreshing call state for the phone call.
            /// </summary>
            /// <param name="pCall">The PhoneCall object that contains the call state.</param>
            void RefreshPhoneCallState(object pCall)
            {
                PhoneCall phoneCall = (PhoneCall)pCall;

                // Update the phone call state. This results in a GetPhoneCallInformation operation call to EWS.
                phoneCall.Refresh();

                System.Console.WriteLine(System.DateTime.Now + " - Call Status: " + phoneCall.State);
            } // End Sub RefreshPhoneCallState 


            // Find the first email message in the Inbox folder.
            ItemView view = new ItemView(1);
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly);

            // Find the first email message in the Inbox. This results in a FindItem operation call to EWS. 
            FindItemsResults<Item> results = service.FindItems(WellKnownFolderName.Inbox, view);

            try
            {
                string itemId = results.Items[0].Id.UniqueId;
                string dialstring = "4255551212";

                // Initiate a call to dictate an email message over a phone call. 
                // This results in a PlayOnPhone operation call to EWS.
                PhoneCall call = service.UnifiedMessaging.PlayOnPhone(itemId, dialstring);

                System.Console.WriteLine("Call Number: " + dialstring);
                System.Console.WriteLine(System.DateTime.Now + " - Call Status: " + call.State + "\n\r");

                // Create a timer that will start immediately. Timer will call callback every 2 seconds.
                using (System.Threading.Timer timer = new System.Threading.Timer(RefreshPhoneCallState, call, 0, 2000))
                {
                    System.Console.WriteLine("PRESS ENTER TO END THE PHONE CALL AND CALL STATUS UPDATES");
                    System.Console.ReadLine();

                    // Disconnect the phone call if it is not already disconnected.
                    if (call.State != PhoneCallState.Disconnected)
                    {
                        call.Disconnect();
                    }
                } // End Using timer 

                System.Console.WriteLine("PRESS ENTER TO END CLOSE THIS WINDOW");
                System.Console.ReadLine();
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex.Message);
            }

        } // End Sub PlayEmailOnPhone 


        public static void PlayEmailOnPhone()
        {
            // PlayOnPhone requires 2010+ !
            PlayEmailOnPhone(GetService(ExchangeVersion.Exchange2010));
        } // End Sub PlayEmailOnPhone 




        private static FolderId FindFolderIdByDisplayName(
            ExchangeService service, string DisplayName, WellKnownFolderName SearchFolder)
        {
            // Specify the root folder to be searched.
            Folder rootFolder = Folder.Bind(service, SearchFolder);

            // Loop through the child folders of the folder being searched.
            foreach (Folder folder in rootFolder.FindFolders(new FolderView(100)))
            {
                // If the display name of the current folder matches the specified display name, return the folder's unique identifier.
                if (folder.DisplayName == DisplayName)
                {
                    return folder.Id;
                }
            } // Next folder 

            // If no folders have a display name that matches the specified display name, return null.
            return null;
        } // End Function FindFolderIdByDisplayName 
        

        public static void DelaySendEmail(ExchangeService service)
        {
            // Create a new email message.
            EmailMessage message = new EmailMessage(service);

            // Specify the email recipient and subject.
            
            message.ToRecipients.Add(RedmineMailService.Trash.UserData.info);
            message.Subject = "Test subject";

            // Identify the extended property that can be used to specify when to send the email.
            ExtendedPropertyDefinition PR_DEFERRED_SEND_TIME = new ExtendedPropertyDefinition(16367, MapiPropertyType.SystemTime);

            // Set the time that will be used to specify when the email is sent.
            // In this example, the email will be sent 1 minute after the next line executes,
            // provided that the message.SendAndSaveCopy request is processed by the server within 1 minute.
            string sendTime = System.DateTime.Now.AddMinutes(20) //.ToUniversalTime()
                .ToString(System.Globalization.CultureInfo.InvariantCulture);

            // Specify when to send the email by setting the value of the extended property.
            message.SetExtendedProperty(PR_DEFERRED_SEND_TIME, sendTime);

            // Specify the email body.
            System.Text.StringBuilder str = new System.Text.StringBuilder();
            str.AppendLine("Client submitted the message.SendAndSaveCopy request at: " 
                + System.DateTime.Now.ToUniversalTime().ToString() + ";");
            str.AppendLine(" email message will be sent at: " + sendTime + ".");
            message.Body = str.ToString();

            System.Console.WriteLine("");
            System.Console.WriteLine("Client submitted the message.SendAndSaveCopy request at: " 
                + System.DateTime.Now.ToUniversalTime().ToString() + ".");
            System.Console.WriteLine("Email message will be sent at: " + sendTime + ".");

            // Submit the request to send the email message.
            message.SendAndSaveCopy();
        } // End Sub DelaySendEmail 


        public static void DelaySendEmail()
        {
            DelaySendEmail(GetService());
        } // End Sub DelaySendEmail 


        public static void FindUnreadEmail(ExchangeService service)
        {
            try
            {
                Folder inbox = Folder.Bind(service, WellKnownFolderName.Inbox);

            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e);
            }
            
            // The search filter to get unread email.
            SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And
                , new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));
            ItemView view = new ItemView(1);

            // https://stackoverflow.com/questions/20662855/fetching-all-mails-in-inbox-from-exchange-web-services-managed-api-and-storing-t
            FindItemsResults<Item> findResults = null;

            do
            {
                // Fire the query for the unread items.
                // This method call results in a FindItem call to EWS.
                findResults = service.FindItems(WellKnownFolderName.Inbox, sf, view);

                System.Console.WriteLine(findResults.TotalCount);

                foreach (Item item in findResults)
                {
                    if (item is EmailMessage)
                    {
                        EmailMessage em = item as EmailMessage;
                        System.Console.WriteLine("Subject: \"{0}\"", em.Subject);

                        // https://msdn.microsoft.com/en-us/library/office/dn726695(v=exchg.150).aspx
                        if (em.HasAttachments)
                        {
                            EmailMessage message = EmailMessage.Bind(service, item.Id
                                , new PropertySet(BasePropertySet.IdOnly, ItemSchema.Attachments));

                            foreach (Attachment attachment in message.Attachments)
                            {
                                System.Console.WriteLine(attachment.Name);

                                if (attachment is FileAttachment)
                                {
                                    FileAttachment fa = attachment as FileAttachment;
                                    fa.Load();
                                    // System.Console.WriteLine(fa.Content);
                                    // System.IO.File.WriteAllBytes(@"d:\" + attachment.Name, fa.Content);
                                    // System.Console.WriteLine(fa.ContentType);
                                    // System.Console.WriteLine(fa.Content.Length);
                                } // End if (attachment is FileAttachment) 
                                else if (attachment is ItemAttachment)
                                {
                                    ItemAttachment itemAttachment = attachment as ItemAttachment;
                                    itemAttachment.Load(ItemSchema.MimeContent);
                                    string fileName = @"C:\Temp\" + itemAttachment.Item.Subject + @".eml";

                                    // Write the bytes of the attachment into a file.
                                    // System.IO.File.WriteAllBytes(fileName, itemAttachment.Item.MimeContent.Content);
                                    System.Console.WriteLine("Email attachment name: " + itemAttachment.Item.Subject + ".eml");
                                } // End else if (attachment is ItemAttachment) 

                            } // Next attachment 

                        } // End if (em.HasAttachments) 

                    } // End if (item is EmailMessage)
                    else if (item is MeetingRequest)
                    {
                        MeetingRequest mr = item as MeetingRequest;
                        System.Console.WriteLine("Subject: \"{0}\"", mr.Subject);
                    }
                    else
                    {
                        // we can handle other item types
                    }

                } // Next item 

                //any more batches?
                if (findResults.NextPageOffset.HasValue)
                {
                    view.Offset = findResults.NextPageOffset.Value;
                } // End if (findResults.NextPageOffset.HasValue) 

            } while (findResults.MoreAvailable) ;

        } // End Sub FindUnreadEmail 


        public static void FindUnreadEmail()
        {
            FindUnreadEmail(GetService());
        } // End Sub FindUnreadEmail 



        public static void MoveMessage(ExchangeService service)
        {
            // Create two items to be moved. You can move any item type in your Exchange mailbox. 
            // You will need to save these items to your Exchange mailbox before they can be moved.
            EmailMessage email1 = new EmailMessage(service);
            email1.Subject = "Draft email one";
            email1.Body = new MessageBody(BodyType.Text, "Draft body of the mail.");

            EmailMessage email2 = new EmailMessage(service);
            email2.Subject = "Draft email two";
            email1.Body = new MessageBody(BodyType.Text, "Draft body of the mail.");


            System.Collections.ObjectModel.Collection<EmailMessage> messages = new System.Collections.ObjectModel.Collection<EmailMessage>();
            messages.Add(email1);
            messages.Add(email2);

            try
            {
                // This results in a CreateItem operation call to EWS. The items are created on the server.
                // The response contains the item identifiers of the newly created items. The items on the client
                // now have item identifiers, which you need in order to move the item.
                ServiceResponseCollection<ServiceResponse> responses = service.CreateItems(messages, WellKnownFolderName.Drafts, MessageDisposition.SaveOnly, null);

                if (responses.OverallResult == ServiceResult.Success)
                {
                    System.Console.WriteLine("Successfully created items to be copied.");
                }
                else
                {
                    throw new System.Exception("The batch creation of the email message draft items was not successful.");
                }
            }
            catch (ServiceResponseException ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }

            try
            {
                // You can move a single item. This will result in a MoveItem operation call to EWS.
                // The EmailMessage that is returned is the item with its updated item identifier. You must save the email
                // message to the server before you can move it. 
                EmailMessage email3 = email1.Move(WellKnownFolderName.DeletedItems) as EmailMessage;
            }

            catch (ServiceResponseException ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }

        } // End Sub MoveMessage 


        public static void MoveMessage()
        {
            MoveMessage(GetService());
        } // End Sub MoveMessage  


        /// <summary>
        /// Creates and tries to send three email messages with one call to EWS. The third email message intentionally fails 
        /// to demonstrate how EWS returns errors for batch requests.
        /// </summary>
        /// <param name="service">A valid ExchangeService object with credentials and the EWS URL.</param>
        static void SendBatchEmails(ExchangeService service)
        {
            // Create three separate email messages.
            EmailMessage message1 = new EmailMessage(service);
            message1.ToRecipients.Add("user1@contoso.com");
            message1.ToRecipients.Add("user2@contoso.com");
            message1.Subject = "Status Update";
            message1.Body = "Project complete!";

            EmailMessage message2 = new EmailMessage(service);
            message2.ToRecipients.Add("user1@contoso.com");
            message2.Subject = "High priority work items";
            message2.Importance = Importance.High;
            message2.Body = "Finish estimate by EOD!";

            EmailMessage message3 = new EmailMessage(service);
            message3.BccRecipients.Add("user1@contoso.com");
            message3.BccRecipients.Add("user2contoso.com"); // Invalid email address format. 
            message3.Subject = "Surprise party!";
            message3.Body = "Don't tell anyone. It will be at 6:00 at Aisha's house. Shhh!";
            message3.Categories.Add("Personal Party");

            System.Collections.ObjectModel.Collection<EmailMessage> msgs = new System.Collections.ObjectModel.Collection<EmailMessage>() { message1, message2, message3 };

            try
            {
                // Send the batch of email messages. This results in a call to EWS. The response contains the results of the batched request to send email messages.
                ServiceResponseCollection<ServiceResponse> response = service.CreateItems(msgs, WellKnownFolderName.Drafts, MessageDisposition.SendOnly, null);

                // Check the response to determine whether the email messages were successfully submitted.
                if (response.OverallResult == ServiceResult.Success)
                {
                    System.Console.WriteLine("All email messages were successfully submitted");
                    return;
                }

                int counter = 1;

                /* If the response was not an overall success, access the errors.
                 * Results are returned in the order that the action was submitted. For example, the attempt for message1
                 * will be represented by the first result since it was the first one added to the collection.
                 * Errors are not returned if an NDR is returned.
                 */
                foreach (ServiceResponse resp in response)
                {
                    System.Console.WriteLine("Result (message {0}): {1}", counter, resp.Result);
                    System.Console.WriteLine("Error Code: {0}", resp.ErrorCode);
                    System.Console.WriteLine("Error Message: {0}\r\n", resp.ErrorMessage);

                    counter++;
                }
            }
            catch (System.Exception e)
            {
                System.Console.WriteLine(e.Message);
            }
        }


        public static void SendBatchEmails()
        {
            SendBatchEmails(GetService());
        } // End Sub SendBatchEmails


        // https://blogs.msdn.microsoft.com/fiddler/2011/12/10/revisiting-fiddler-and-win8-immersive-applications/

        // mono mozroots.exe --import --sync
        // https://support.securly.com/hc/en-us/articles/206081828-How-to-manually-install-the-Securly-SSL-certificate-in-Chrome
        // https://askubuntu.com/questions/73287/how-do-i-install-a-root-certificate

        // sudo cp foo.crt /usr/share/ca-certificates/extra/foo.crt
        // sudo dpkg-reconfigure ca-certificates

        // cer = cert. 
        // In case of a.pem file on Ubuntu, it must first be converted to a.crt file:
        // import firefox > export pem
        // openssl x509 -in foo.pem -inform PEM -out foo.crt
        // cert-sync "/root/Desktop/DO_NOT_TRUST_FiddlerRoot.pem"
        // cert-sync --user "/root/Desktop/DO_NOT_TRUST_FiddlerRoot.pem"

        // WWW-Authenticate: Negotiate <!!!
        // WWW-Authenticate: NTLM

        // <t:TimeZoneDefinition Id="W. Europe Standard Time" />
        // NTLM - Negotiate
        public static void ListFolders(ExchangeService service)
        {
            System.Console.WriteLine("start folder listing");
            
            try
            {
                Folder rootfolder = Folder.Bind(service, WellKnownFolderName.MsgFolderRoot);

                // Set the properties you want to retrieve when you load the folder.
                PropertySet propsToLoad = new PropertySet(FolderSchema.DisplayName,
                    FolderSchema.ChildFolderCount,
                    FolderSchema.FolderClass,
                    // Note that you don't display the folder IDs because they're very large,
                    // but retrieve them because they can be useful in other methods you might call.
                    FolderSchema.Id);

                // Get the root folder with the selected properties.
                rootfolder.Load(propsToLoad);

                // Load the number of subfolders unless there are more than 100.
                int numSubFoldersToView = rootfolder.ChildFolderCount <= 100 ? rootfolder.ChildFolderCount : 100;

                // Display the child folders under the root, the number of subfolders under each child, and the folder class of each child folder.
                System.Console.WriteLine("\r\n" + "Folder Name".PadRight(28) + "\t" + "subfolders".PadRight(12) +
                                         "Folder Class" + "\r\n");

                foreach (Folder childFolder in rootfolder.FindFolders(new FolderView(numSubFoldersToView)))
                {
                    System.Console.WriteLine(childFolder.DisplayName.PadRight(28)
                                             + "\t"
                                             + childFolder.ChildFolderCount.ToString().PadRight(12)
                                             + childFolder.FolderClass);
                } // Next childFolder 
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine(ex);
                System.Environment.Exit(-1);
            }

            System.Console.WriteLine("finished");
        } // End Sub ListFolders 


        public static void ListFolders()
        {
            ListFolders(GetService());
        } // End Sub ListFolders 


        public static void TestSend(ExchangeService service)
        {
            EmailMessage email = new EmailMessage(service);

            email.ToRecipients.Add(RedmineMailService.Trash.UserData.info);

            email.Subject = "EWS Managed API Test";
            string now  = System.DateTime.Now.ToString("dddd, dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            email.Body = new MessageBody($"This email has been sent by using the EWS Managed API at {now}.");

            email.Send();
            System.Console.WriteLine("E-Mail sent...");
        } // End Sub TestSend 


        public static void TestSend()
        {
            TestSend(GetService());
        } // End Sub TestSend 


    } // End Class 


} // End Namespace 
