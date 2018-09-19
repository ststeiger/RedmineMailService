
namespace RedmineMailService 
{


    public abstract class BaseMailTemplate 
    {


        public enum MailPriority_t
        {
            Normal = 0,
            Low = 1,
            High = 2
        }


        public string MailId;
        public string TemplateId;


        public string Subject;
        public string TemplateString;
        public bool UseHtml;
        public MailPriority_t Priority = MailPriority_t.Normal;
        public System.Collections.Generic.List<Resource> EmbeddedImages;
        public System.Collections.Generic.List<Resource> AttachmentFiles;
        
        
        public string From;

        protected string m_fromName;

        public string FromName
        {
            get
            {
                if (this.m_fromName != null)
                    return this.m_fromName;

                return this.From;
            }
            set
            {
                this.m_fromName = value;
            }
        }


        public string To;

        protected string m_toName;

        public string ToName
        {
            get
            {
                if (this.m_toName != null)
                    return this.m_toName;

                return this.To;
            }
            set
            {
                this.m_toName = value;
            }
        }

        public string CC;

        protected string m_ccName;
        
        public string CCName
        {
            get
            {
                if (this.m_ccName != null)
                    return this.m_ccName;

                return this.CC;
            }
            set
            {
                this.m_ccName = value;
            }
        }
        
        
        
        public string Bcc;

        protected string m_bccName;

        public string BccName
        {
            get
            {
                if (this.m_bccName != null)
                    return this.m_bccName;

                return this.Bcc;
            }
            set
            {
                this.m_bccName = value;
            }
        }


        public string ReplyTo;


        protected string m_replyName;

        public string ReplyToName
        {
            get
            {
                if (this.m_replyName != null)
                    return this.m_replyName;

                return this.ReplyTo;
            }
            set
            {
                this.m_replyName = value;
            }
        }





        public virtual System.Text.StringBuilder TemplateStringBuilder
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(this.TemplateString);
                return sb;
            }
        }


        public virtual System.Text.StringBuilder TemplateStringForFormat
        {
            get
            {
                System.Text.StringBuilder sb = this.TemplateStringBuilder;
                return sb.Replace("{", "{{").Replace("}", "}}");
            }
        }


        public virtual System.Collections.Generic.List<Resource> AllFiles
        {
            get
            {
                System.Collections.Generic.List<Resource> ls =
                    new System.Collections.Generic.List<Resource>();

                ls.AddRange(this.EmbeddedImages);
                ls.AddRange(this.AttachmentFiles);

                return ls;
            }
        }


        public BaseMailTemplate()
        {
            this.MailId = System.Guid.NewGuid().ToString();
            this.UseHtml = true;
            this.TemplateString = "";
            this.EmbeddedImages = new System.Collections.Generic.List<Resource>();
            this.AttachmentFiles = new System.Collections.Generic.List<Resource>();
        }


        public BaseMailTemplate(string template)
            : this()
        {
            if (template != null)
                this.TemplateString = template;
        }


        public BaseMailTemplate(string template
            , System.Collections.Generic.IEnumerable<Resource> images
            , System.Collections.Generic.IEnumerable<Resource> attachments
            )
            : this(template)
        {
            if (images != null)
                this.EmbeddedImages.AddRange(images);

            if (attachments != null)
                this.AttachmentFiles.AddRange(attachments);
        }


        public BaseMailTemplate(string template, params Resource[] images)
            : this(template, images, null)
        { }


        public BaseMailTemplate(string template
            , System.Collections.Generic.IEnumerable<Resource> images
            , System.Collections.Generic.IEnumerable<Resource> attachments
            , bool useHtml
            )
            : this(template, images, attachments)
        {
            this.UseHtml = useHtml;
        }


    } // End Class BaseMailTemplate 


} // End Namespace RedmineMailService 
