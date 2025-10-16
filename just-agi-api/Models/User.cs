/*
  CopilotPrompt
  #file:'2-architecture.md' ����2-architecture.md�е�User�����ݣ�����C#��ʵ���࣬���Ҽ�������ע��
 
 */


namespace just_agi_api.Models
{
    /// <summary>
    /// �û�ʵ����
    /// </summary>
    public class User
    {
        /// <summary>
        /// ���
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// �û��������䣩
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        public string Salt { get; set; }

        /// <summary>
        /// ��Ч��
        /// </summary>
        public bool Valid { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// �����û�
        /// </summary>
        public long CreateUser { get; set; }

        /// <summary>
        /// �޸�ʱ��
        /// </summary>
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// �޸��û�
        /// </summary>
        public long ModifyUser { get; set; }
    }
}
