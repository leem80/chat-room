using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ZqlChartObjects
{
    [DataContract]
    public class ChatUser
    {
        public ChatUser
        (
            string strNickName,
            string strUser,
            string strMachine,
            int nProcessId,
            bool bIsServer
        )
        {
            m_strNickName = strNickName;
            m_strUserName = strUser;
            m_strMachine = strMachine;
            m_nProcessId = nProcessId;
            m_bIsServer = bIsServer;
        }

        private int m_nProcessId;
        private string m_strMachine;
        private string m_strUserName;
        private string m_strNickName;
        private bool m_bIsServer;

        [DataMember]
        public string NickName
        {
            get { return m_strNickName; }
            set { m_strNickName = value; }
        }

        [DataMember]
        public string UserName
        {
            get { return m_strUserName; }
            set { m_strUserName = value; }
        }

        [DataMember]
        public string Machine
        {
            get { return m_strMachine; }
            set { m_strMachine = value; }
        }

        [DataMember]
        public int ProcessId
        {
            get { return m_nProcessId; }
            set { m_nProcessId = value; }
        }

        [DataMember]
        public bool IsServer
        {
            get { return m_bIsServer; }
            set { m_bIsServer = value; }
        }
    }
}
