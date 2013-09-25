using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mail;
using System.Xml;

namespace WebSearch.Common.Net
{
    /// <summary>
    /// Email Helper
    /// </summary>
    public class EmailHelper
    {
        #region Properties

        private string _pop3Server = "";

        /// <summary>
        /// For Receive Emails
        /// </summary>
        public string Pop3Server
        {
            get { return _pop3Server; }
            set { _pop3Server = value; }
        }

        private string _smtpServer = "";

        /// <summary>
        /// For Send Emails
        /// </summary>
        public string SmtpServer
        {
            get { return _smtpServer; }
            set { _smtpServer = value; }
        }

        private int _smtpPort = 25;

        /// <summary>
        /// 
        /// </summary>
        public int SmtpPort
        {
            get { return _smtpPort; }
            set { _smtpPort = value; }
        }

        private bool _useSSL = false;

        /// <summary>
        /// 
        /// </summary>
        public bool UseSSL
        {
            get { return _useSSL; }
            set { _useSSL = value; }
        }

        private string _emailAddress = null;

        /// <summary>
        /// Your Email Address
        /// </summary>
        public string EmailAddress
        {
            get { return _emailAddress; }
            set { _emailAddress = value; }
        }

        private string _loginPassword = null;

        /// <summary>
        /// Your Email Login Password
        /// </summary>
        public string LoginPassword
        {
            set { _loginPassword = value; }
        }

        #endregion

        #region Constructors

        public EmailHelper(string serverName)
        {
            XmlElement elem = XmlHelper.ReadNode(Config.SettingPath + "EmailServer.xml", serverName);
            if (elem == null) throw new Exception("Invalid Server Name");

            this._smtpServer = elem.Attributes["_Smtp"].Value;
            this._smtpPort = int.Parse(elem.Attributes["_SmtpPort"].Value);
            this._pop3Server = elem.Attributes["_Pop3"].Value;
            this._useSSL = bool.Parse(elem.Attributes["_UseSSL"].Value);
        }

        public EmailHelper(string serverName, string emailAddress, 
            string password) : this(serverName)
        {
            this._emailAddress = emailAddress;
            this._loginPassword = password;
        }

        public EmailHelper(string smtpServer, string pop3Server)
        {
            this._smtpServer = smtpServer;
            this._pop3Server = pop3Server;
        }

        public EmailHelper(string smtpServer, int smtpPort, string pop3Server, bool useSSL)
        {
            this._smtpServer = smtpServer;
            this._smtpPort = (smtpPort <= 0) ? 25 : smtpPort;
            this._pop3Server = pop3Server;
            this._useSSL = useSSL;
        }

        public EmailHelper(string smtpServer, string pop3Server, 
            string emailAddress, string password) 
            : this(smtpServer, pop3Server)
        {
        }

        #endregion

        #region Public Methods

        public bool SendEmail(string[] tos, string[] ccs, string subject,
            string body, bool ishtml, Encoding encoding, string[] attachs)
        {
            // prepare the to, cc.
            string tolist = "";
            if (tos != null && tos.Length > 0)
            {
                tolist = tos[0];
                for (int i = 1; i < tos.Length; i++)
                    tolist += ";" + tos[i];
            }
            string cclist = "";
            if (ccs != null && ccs.Length > 0)
            {
                cclist = ccs[0];
                for (int i = 1; i < ccs.Length; i++)
                    cclist += ";" + ccs[i];
            }

            return SendEmail(tolist, cclist, subject, 
                body, ishtml, encoding, attachs);
        }

        public bool SendEmail(string to, string cc, string subject,
            string body, bool ishtml, Encoding encoding, string[] attachs)
        {
            // build a MailMessage object
            MailMessage mail = new MailMessage();

            mail.To = (to == null) ? "" : to;
            mail.Cc = (cc == null) ? "" : cc;
            mail.Subject = (subject == null) ? "" : subject;

            // set the mail body
            mail.Body = (body == null) ? "" : body;
            mail.BodyFormat = (ishtml) ? MailFormat.Html : MailFormat.Text;
            mail.BodyEncoding = (encoding == null) ? Encoding.UTF8 : encoding;

            // add attachments
            if (attachs != null && attachs.Length > 0)
            {
                foreach (string attfile in attachs)
                {
                    MailAttachment att = new MailAttachment(attfile);
                    mail.Attachments.Add(att);
                }
            }
            return SendEmail(mail);
        }

        public bool SendEmail(MailMessage mail)
        {
            mail.From = this.EmailAddress;
            // add the smtp authentication into the mail object
            // 1. basic authentication 
            mail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");
            // 2. set your username here 
            mail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", _emailAddress);
            // 3. set your password here
            mail.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", _loginPassword);

            try
            {
                SmtpMail.SmtpServer = this.SmtpServer;
                SmtpMail.Send(mail);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public List<MailMessage> ReceiveEmail()
        {
            return null;
        }

        #endregion

        #region Backup Region

        // POP3 manage class
// Karavaev Denis karavaev_denis@hotmail.com
// http://wasp.elcat.kg
///////////////////////////////////////////////////

//namespace wasp
//{
//    /// <summary>
//    /// Summary description for waspPOP3.
//    /// http://www.codeproject.com/csharp/Karavaev_Denis/
//    /// </summary>
//    class waspPOP3
//    {
//        // variables for pop3 class
//        public String pop3host;
//        public int port;
//        public String user;
//        public String pwd;
//        public String command;
//        public TcpClient w_TcpClient;
//        public NetworkStream w_NetStream;
//        public StreamReader  w_ReadStream;
//        public byte[] bData;	//for the data, tat we'll recive

//        public string DoConnect(String pop3host,int port, String user, String pwd)
//        {	
//            // create POP3 connection
//            w_TcpClient = new TcpClient(pop3host,port);

//            try
//            {	// initialization
//                w_NetStream = w_TcpClient.GetStream();
//                w_ReadStream = new StreamReader(w_TcpClient.GetStream());
//                w_ReadStream.ReadLine();

//                // send login
//                command = "USER "+ user+"\r\n";
//                bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//                w_NetStream.Write(bData,0,bData.Length);
//                w_ReadStream.ReadLine();
//                // send pwd
//                command = "PASS "+ pwd+"\r\n";
//                bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//                w_NetStream.Write(bData,0,bData.Length);
//                w_ReadStream.ReadLine();
//            }
//            catch(InvalidOperationException err)
//            {
//                return ("Error: "+err.ToString());
//            }
//            return "+OK";
//        }
//        public string GetStat()
//        {
//            // Send STAT command to get number of mail and total size
//            command = "STAT\r\n";				
//            bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//            w_NetStream.Write(bData,0,bData.Length);
//            return w_ReadStream.ReadLine();
//        }

//        public string GetList()// Send LIST command with no parametrs to get all information
//        {
//            string sTemp;	// For saving 'list' results
//            string sList = "";
//            command = "LIST\r\n";				
//            bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//            w_NetStream.Write(bData,0,bData.Length);
//            sTemp = w_ReadStream.ReadLine();

//            if(sTemp[0] != '-')	// errors begins with '-'
//            {
//                while(sTemp != ".")	//saving data to string while not found '.'
//                {
//                    sList += sTemp+"\r\n";
//                    sTemp = w_ReadStream.ReadLine();
//                }
//            }
//            else
//            {
//                return sTemp;
//            }
//            return sList;
//        }
//        public string GetList(int num)// Send LIST command with number of a letter
//        {
//            command = "LIST " + num + "\r\n";
//            bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//            w_NetStream.Write(bData,0,bData.Length);
//            return w_ReadStream.ReadLine();
//        }
//        public string Retr(int num)
//        {
//            string sTemp;						
//            string sBody = "";
//            try
//            {
//                command = "RETR "+ num+"\r\n";				
//                bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//                w_NetStream.Write(bData,0,bData.Length);				

//                sTemp = w_ReadStream.ReadLine();
//                if(sTemp[0] != '-')	//errors begins with -
//                {
//                    while(sTemp!=".")	// . - is the end of the server response
//                    {
//                        sBody += sTemp+"\r\n";
//                        sTemp = w_ReadStream.ReadLine();
//                    }
//                }
//                else
//                {
//                    return sTemp;
//                }
//            }
//            catch(InvalidOperationException err)
//            {
//                return ("Error: "+err.ToString());
//            }
//            return sBody;
//        }
//        public string Dele(int num)
//        {
//            // Send DELE command to delete message with specified number
//            command = "DELE " + num + "\r\n";				
//            bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//            w_NetStream.Write(bData,0,bData.Length);
//            return w_ReadStream.ReadLine();
//        }
//        public string Rset()
//        {
//            // Send RSET command to unmark all deleteting messages
//            command = "RSET\r\n";				
//            bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//            w_NetStream.Write(bData,0,bData.Length);
//            return w_ReadStream.ReadLine();
//        }

//        public string Quit()
//        {
//            // Send QUIT
//            command = "QUIT\r\n";				
//            bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//            w_NetStream.Write(bData,0,bData.Length);
//            String tmp = w_ReadStream.ReadLine();
//            w_NetStream.Close();
//            w_ReadStream.Close();
//            return tmp;
//        }
//        public string GetTop(int num)
//        {
//            string sTemp;
//            string sTop = "";
//            try
//            {
//                // retrieve mail with number mail parameter
//                command = "TOP "+ num+" n\r\n";				
//                bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//                w_NetStream.Write(bData,0,bData.Length);				

//                sTemp = w_ReadStream.ReadLine();
//                if(sTemp[0] != '-') 
//                {
//                    while(sTemp != ".")
//                    {
//                        sTop += sTemp+"\r\n";
//                        sTemp = w_ReadStream.ReadLine();
//                    }
//                }
//                else
//                {
//                    return  sTemp;
//                }
//            }
//            catch(InvalidOperationException err)
//            {
//                return ("Error: "+err.ToString());
//            }
//            return sTop;
//        }
//        public string GetTop(int num_mess, int num_strok)
//        {
//            string sTemp;
//            string sTop = "";
//            try
//            {
//                // retrieve mail with number mail parameter
//                command = "TOP " + num_mess + " " + num_strok + "\r\n";				
//                bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//                w_NetStream.Write(bData,0,bData.Length);				

//                sTemp = w_ReadStream.ReadLine();
//                if(sTemp[0] != '-') 
//                {
//                    while(sTemp != ".")
//                    {
//                        sTop += sTemp+"\r\n";
//                        sTemp = w_ReadStream.ReadLine();
//                    }
//                }
//                else
//                {
//                    return sTemp;
//                }
//            }
//            catch(InvalidOperationException err)
//            {
//                return ("Error: "+err.ToString());
//            }
//            return sTop;
//        }
//        public string GetUidl()
//        {
//            string sTemp;
//            string sUidl = "";
//            command = "UIDL\r\n";				
//            bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//            w_NetStream.Write(bData,0,bData.Length);
//            sTemp = w_ReadStream.ReadLine();

//            if(sTemp[0] != '-')	// errors begins with '-'
//            {
//                while(sTemp != ".")	//saving data to string while not found '.'
//                {
//                    sUidl += sTemp+"\r\n";
//                    sTemp = w_ReadStream.ReadLine();
//                }
//            }
//            else
//            {
//                return sTemp;
//            }
//            return sUidl;
//        }
//        public string GetUidl(int num)
//        {
//            command = "UIDL " + num + "\r\n";				
//            bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//            w_NetStream.Write(bData,0,bData.Length);
//            return w_ReadStream.ReadLine();
//        }
//        public string GetNoop()
//        {
//            // Send NOOP command to check if we are connected
//            command = "NOOP\r\n";				
//            bData = System.Text.Encoding.ASCII.GetBytes(command.ToCharArray());
//            w_NetStream.Write(bData,0,bData.Length);
//            return w_ReadStream.ReadLine();
//        }
//    }
//}

        #endregion
    }

    /// <summary>
    /// Email Alerter
    /// </summary>
    public static class EmailAlerter
    {
        private const string _alerterName = "alerter";

        private static EmailHelper _emailHelper = null;

        public static EmailHelper EmailHelper
        {
            get
            {
                if (_emailHelper == null)
                {
                    // 1. get server name by alerter name
                    XmlElement elem = XmlHelper.ReadNode(
                        Config.SettingPath + "EmailAccount.xml", _alerterName);
                    if (elem == null) 
                        throw new Exception("EmailAccount.xml lacks alerter account");

                    _emailHelper = new EmailHelper(elem.Attributes["_Server"].Value,
                        elem.Attributes["_Email"].Value, elem.Attributes["_Pwd"].Value);
                }
                return _emailHelper;
            }
        }

        public static bool Alert(string to, string subject, string message)
        {
            return EmailAlerter.EmailHelper.SendEmail(
                to, "", subject, message, false, null, null);
        }

        public static bool Alert(string to, string cc, 
            string subject, string message, bool ishtml)
        {
            return EmailAlerter.EmailHelper.SendEmail(
                to, cc, subject, message, ishtml, null, null);
        }
    }
}
