/// <summary>
/// �ͻ���
/// </summary>
public class Customer
{
    /// <summary>
    /// ���
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// ����
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// �Ա�
    /// </summary>
    public bool Sex { get; set; }

    /// <summary>
    /// ����
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// �绰
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// ����
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// ��ַ
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// ��Ч��
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// ��ҵ���
    /// </summary>
    public long EnterpriseID { get; set; }

    /// <summary>
    /// ��ҵ��ɫ���
    /// </summary>
    public long RoleID { get; set; }

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
