
namespace RedmineMailService
{


    public class MailTemplate
        : BaseMailTemplate, System.IDisposable 
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


        void System.IDisposable.Dispose()
        {
            if (this.EmbeddedImages != null)
            {
                foreach (Resource thisResource in this.EmbeddedImages)
                {
                    if (thisResource != null)
                        ((System.IDisposable)thisResource).Dispose();
                }

                this.EmbeddedImages.Clear();
                this.EmbeddedImages = null;
            }

            if (this.AttachmentFiles != null)
            {
                foreach (Resource thisResource in this.AttachmentFiles)
                {
                    if (thisResource != null)
                        ((System.IDisposable)thisResource).Dispose();
                }

                this.AttachmentFiles.Clear();
                this.AttachmentFiles = null;
            }
        }

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
