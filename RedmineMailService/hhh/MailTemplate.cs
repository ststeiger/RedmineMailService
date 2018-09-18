
namespace RedmineMailService
{

    public abstract class MailTemplate
        :BaseMailTemplate 
    {}
    
    

    public abstract class BaseMailTemplate 
    {

        public bool UseHtml;
        public string TemplateString;
        public System.Collections.Generic.List<Resource> EmbeddedImages; 
        public System.Collections.Generic.List<Resource> AttachmentFiles;

        // ------
        public System.Net.Mail.MailAddress From;
        public System.Net.Mail.MailAddress To;
        public System.Net.Mail.MailAddress Bcc;
        public string Subject;
        public MailPriority mp = MailPriority.High;

        
        public enum MailPriority
        {
            Normal = 0,
            Low = 1,
            High = 2
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
            get {
                System.Collections.Generic.List<Resource> ls = 
                    new System.Collections.Generic.List<Resource>();

                ls.AddRange(this.EmbeddedImages);
                ls.AddRange(this.AttachmentFiles);

                return ls;
            }
        }


        public MailTemplate()
        {
            this.UseHtml = true;
            this.TemplateString = "";
            this.EmbeddedImages = new System.Collections.Generic.List<Resource>();
            this.AttachmentFiles = new System.Collections.Generic.List<Resource>();
        }


        public MailTemplate(string template)
            : this()
        {
            if(template != null)
                this.TemplateString = template;
        }


        public MailTemplate(string template
            , System.Collections.Generic.IEnumerable<Resource> images
            , System.Collections.Generic.IEnumerable<Resource> attachments
            )
            : this(template)
        {
            if(images != null)
                this.EmbeddedImages.AddRange(images);

            if (attachments != null)
                this.AttachmentFiles.AddRange(attachments);
        }


        public MailTemplate(string template, params Resource[] images)
            : this(template, images, null)
        { }


        public MailTemplate(string template
            , System.Collections.Generic.IEnumerable<Resource> images
            , System.Collections.Generic.IEnumerable<Resource> attachments
            , bool useHtml
            )
            : this(template, images, attachments)
        {
            this.UseHtml = useHtml;
        }
        
        
        public virtual MailTemplate Clone()
        {
            return new MailTemplate(
                 this.TemplateString 
                ,this.EmbeddedImages 
                ,this.AttachmentFiles 
                ,this.UseHtml 
            );
        } // End Function Clone 


    } // End Class MailTemplate 


} // End Namespace RedmineMailService 
