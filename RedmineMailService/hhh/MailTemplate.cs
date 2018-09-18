
namespace RedmineMailService
{


    public class MailTemplate
        : BaseMailTemplate 
    {


        public MailTemplate()
            :base()
        { }


        public MailTemplate(string template)
            : base(template)
        { }


        public MailTemplate(string template
            , System.Collections.Generic.IEnumerable<Resource> images
            , System.Collections.Generic.IEnumerable<Resource> attachments
            )
            : base(template, images, attachments)
        { }


        public MailTemplate(string template, params Resource[] images)
            : base(template, images)
        { }


        public MailTemplate(string template
            , System.Collections.Generic.IEnumerable<Resource> images
            , System.Collections.Generic.IEnumerable<Resource> attachments
            , bool useHtml
            )
            : base(template, images, attachments, useHtml)
        { }


        public virtual MailTemplate Clone()
        {
            return new MailTemplate(
                 this.TemplateString
                , this.EmbeddedImages
                , this.AttachmentFiles
                , this.UseHtml
            );
        } // End Function Clone 


    } // End Class MailTemplate 


} // End Namespace RedmineMailService 
