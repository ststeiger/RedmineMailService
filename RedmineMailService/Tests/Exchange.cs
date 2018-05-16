﻿
using System.Threading.Tasks;
using Microsoft.Exchange.WebServices.Data;


namespace RedmineMailService
{


    // https://github.com/sherlock1982/ews-managed-api
    // https://msdn.microsoft.com/en-us/library/office/dn567668(v=exchg.150).aspx
    // https://msdn.microsoft.com/en-us/library/office/dn495632(v=exchg.150).aspx
    class Exchange
    {

        class NoTrace : Microsoft.Exchange.WebServices.Data.ITraceListener
        {
            void ITraceListener.Trace(string traceType, string traceMessage)
            {
                // throw new NotImplementedException();
            }
        }

        public static async void ListAllMails()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            service.Credentials = new WebCredentials(RedmineMailService.Trash.UserData.Email, RedmineMailService.Trash.UserData.Password);

            Microsoft.Exchange.WebServices.Data.ITraceListener listener = new NoTrace();

            if (listener != null)
            {
                service.TraceListener = listener;
                service.TraceFlags = TraceFlags.All;
                service.TraceEnabled = true;
            } // End if (listener != null) 

            service.AutodiscoverUrl(RedmineMailService.Trash.UserData.Email, RedirectionUrlValidationCallback);

            Folder inbox = await Folder.Bind(service, WellKnownFolderName.Inbox);

        } // End Sub ListAllMails 


        /// <summary>
        /// Finds the first email message and initiates an attempt to call a phone number and 
        /// dictate the contents of the email message. This sample requires that Unified Messaging
        /// is enabled for the caller.
        /// </summary>
        /// <param name="service">An ExchangeService object with credentials and the EWS URL.</param>
        public static async void PlayEmailOnPhone()
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

            // PlayOnPhone requires 2010+ !
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010);
            service.Credentials = new WebCredentials(RedmineMailService.Trash.UserData.Email, RedmineMailService.Trash.UserData.Password);

            Microsoft.Exchange.WebServices.Data.ITraceListener listener = new NoTrace();

            if (listener != null)
            {
                service.TraceListener = listener;
                service.TraceFlags = TraceFlags.All;
                service.TraceEnabled = true;
            } // End if (listener != null) 

            service.AutodiscoverUrl(RedmineMailService.Trash.UserData.Email, RedirectionUrlValidationCallback);


            // Find the first email message in the Inbox folder.
            ItemView view = new ItemView(1);
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly);

            // Find the first email message in the Inbox. This results in a FindItem operation call to EWS. 
            FindItemsResults<Item> results = await service.FindItems(WellKnownFolderName.Inbox, view);

            try
            {
                string itemId = results.Items[0].Id.UniqueId;
                string dialstring = "4255551212";

                // Initiate a call to dictate an email message over a phone call. 
                // This results in a PlayOnPhone operation call to EWS.
                PhoneCall call = await service.UnifiedMessaging.PlayOnPhone(itemId, dialstring);

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


        public static async Task<FolderId> FindFolderIdByDisplayName(
            ExchangeService service, string DisplayName, WellKnownFolderName SearchFolder)
        {
            // Specify the root folder to be searched.
            Folder rootFolder = await Folder.Bind(service, SearchFolder);

            // Loop through the child folders of the folder being searched.
            foreach (Folder folder in await rootFolder.FindFolders(new FolderView(100)))
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


        public static async void DelaySendEmail()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            service.Credentials = new WebCredentials(RedmineMailService.Trash.UserData.Email, RedmineMailService.Trash.UserData.Password);

            Microsoft.Exchange.WebServices.Data.ITraceListener listener = new NoTrace();

            if (listener != null)
            {
                service.TraceListener = listener;
                service.TraceFlags = TraceFlags.All;
                service.TraceEnabled = true;
            } // End if (listener != null) 

            service.AutodiscoverUrl(RedmineMailService.Trash.UserData.Email, RedirectionUrlValidationCallback);

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
            await message.SendAndSaveCopy();
        } // End Sub DelaySendEmail 


        public static async void FindUnreadEmail()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            service.Credentials = new WebCredentials(RedmineMailService.Trash.UserData.Email, RedmineMailService.Trash.UserData.Password);
            
            Microsoft.Exchange.WebServices.Data.ITraceListener listener = new NoTrace();

            if (listener != null)
            {
                service.TraceListener = listener;
                service.TraceFlags = TraceFlags.All;
                service.TraceEnabled = true;
            } // End if (listener != null) 

            service.AutodiscoverUrl(RedmineMailService.Trash.UserData.Email, RedirectionUrlValidationCallback);


            Folder inbox = await Folder.Bind(service, WellKnownFolderName.Inbox);

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
                findResults = await service.FindItems(WellKnownFolderName.Inbox, sf, view);

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
                            EmailMessage message = await
                                EmailMessage.Bind(service, item.Id
                                , new PropertySet(BasePropertySet.IdOnly, ItemSchema.Attachments));

                            foreach (Attachment attachment in message.Attachments)
                            {
                                System.Console.WriteLine(attachment.Name);

                                if (attachment is FileAttachment)
                                {
                                    FileAttachment fa = attachment as FileAttachment;
                                    await fa.Load();
                                    // System.Console.WriteLine(fa.Content);
                                    // System.IO.File.WriteAllBytes(@"d:\" + attachment.Name, fa.Content);
                                    // System.Console.WriteLine(fa.ContentType);
                                    // System.Console.WriteLine(fa.Content.Length);
                                } // End if (attachment is FileAttachment) 
                                else if (attachment is ItemAttachment)
                                {
                                    ItemAttachment itemAttachment = attachment as ItemAttachment;
                                    await itemAttachment.Load(ItemSchema.MimeContent);
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


        public static async void ListFolders()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);
            service.Credentials = new WebCredentials(RedmineMailService.Trash.UserData.Email, RedmineMailService.Trash.UserData.Password);

            Microsoft.Exchange.WebServices.Data.ITraceListener listener = new NoTrace();

            if (listener != null)
            {
                service.TraceListener = listener;
                service.TraceFlags = TraceFlags.All;
                service.TraceEnabled = true;
            } // End if (listener != null) 

            service.AutodiscoverUrl(RedmineMailService.Trash.UserData.Email, RedirectionUrlValidationCallback);

            Folder rootfolder = await Folder.Bind(service, WellKnownFolderName.MsgFolderRoot);

            // Set the properties you want to retrieve when you load the folder.
            PropertySet propsToLoad = new PropertySet(FolderSchema.DisplayName,
                                                      FolderSchema.ChildFolderCount,
                                                      FolderSchema.FolderClass,
                                                      // Note that you don't display the folder IDs because they're very large,
                                                      // but retrieve them because they can be useful in other methods you might call.
                                                      FolderSchema.Id);

            // Get the root folder with the selected properties.
            await rootfolder.Load(propsToLoad);

            // Load the number of subfolders unless there are more than 100.
            int numSubFoldersToView = rootfolder.ChildFolderCount <= 100 ? rootfolder.ChildFolderCount : 100;

            // Display the child folders under the root, the number of subfolders under each child, and the folder class of each child folder.
            System.Console.WriteLine("\r\n" + "Folder Name".PadRight(28) + "\t" + "subfolders".PadRight(12) + "Folder Class" + "\r\n");

            foreach (Folder childFolder in await rootfolder.FindFolders(new FolderView(numSubFoldersToView)))
            {
                System.Console.WriteLine(childFolder.DisplayName.PadRight(28) 
                    + "\t" 
                    + childFolder.ChildFolderCount.ToString().PadRight(12) 
                    + childFolder.FolderClass);
            } // Next childFolder 

        } // End Sub ListFolders 


        public static async void TestSend()
        {
            ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2007_SP1);

            Microsoft.Exchange.WebServices.Data.ITraceListener listener = new NoTrace();

            if (listener != null)
            {
                service.TraceListener = listener;
                service.TraceFlags = TraceFlags.All;
                service.TraceEnabled = true;
            } // End if (listener != null) 


            service.Credentials = new WebCredentials(RedmineMailService.Trash.UserData.Email, RedmineMailService.Trash.UserData.Password);
            service.AutodiscoverUrl(RedmineMailService.Trash.UserData.Email, RedirectionUrlValidationCallback);


            EmailMessage email = new EmailMessage(service);

            email.ToRecipients.Add(RedmineMailService.Trash.UserData.info);

            email.Subject = "EWS Managed API Test";
            string now  = System.DateTime.Now.ToString("dddd, dd.MM.yyyy HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
            email.Body = new MessageBody($"This email has been sent by using the EWS Managed API at {now}.");

            await email.Send();
            System.Console.WriteLine("E-Mail sent...");
        } // End Sub TestSend 


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


    } // End Class 


} // End Namespace 