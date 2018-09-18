
namespace RedmineMailService 
{


    public class MassMail
    {
        public MailTemplate Template;
        public System.Data.DataTable Data;
        protected MailSettings m_mailSettings;


        public MassMail()
        { } // End Constructor 


        public MassMail(MailSettings mailSettings)
            : this()
        {
            this.m_mailSettings = mailSettings;
        } // End Constructor 


        // When multiple handlers are associated with a single event in C# 
        // and the handler signature has a return type, 
        // then the value returned by the last handler executed 
        // will be the one returned to the event raiser.
        public delegate bool SaveEventHandler_t(object sender, System.EventArgs e);
        public event SaveEventHandler_t OnStart;
        public event SaveEventHandler_t OnSuccess;
        public event SaveEventHandler_t OnFailure;
        public event SaveEventHandler_t OnAlways;
        public event SaveEventHandler_t OnDone;


        public void Send()
        {
            System.MulticastDelegate m = (System.MulticastDelegate)OnStart;
            System.Delegate[] dlist = m.GetInvocationList();
            foreach (System.Delegate thisDelegate in dlist)
            {
                object[] p = { /*put your parameters here*/ };
                object ret = thisDelegate.DynamicInvoke(p);
            } // Next thisDelegate 
            
            OnStart(this, null);

            foreach (System.Data.DataRow dr in Data.Rows)
            {
                if (object.Equals("a", "b"))
                    OnSuccess(this, null);
                else
                    OnFailure(this, null);

                OnAlways(this, null);
            } // Next dr 

            OnDone(this, null);
        } // End Sub Send 


    } // End Class MassMail 


} // End Namespace RedmineMailService 
