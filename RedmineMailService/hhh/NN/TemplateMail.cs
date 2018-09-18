
namespace RedmineMailService 
{


    public class TemplateMail
    {
        public string Template;

        public System.Collections.Generic.List<Resource> Images;
        public System.Collections.Generic.List<Resource> Attachments;



        public TemplateMail()
        { }

        public void foo()
        {
            var x = new MassMail();

            x.OnStart += delegate (object sender, System.EventArgs e)
            {
                return false;
            };

            x.OnSuccess += delegate (object sender, System.EventArgs e)
            {
                return false;
            };


            x.OnFailure += delegate (object sender, System.EventArgs e)
            {
                return false;
            };

            x.OnDone += delegate (object sender, System.EventArgs e)
            {
                return false;
            };
        }


    }


}
