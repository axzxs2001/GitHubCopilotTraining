/// <summary>
/// 客户类
/// </summary>
public class Customer
{
    /// <summary>
    /// 编号
    /// </summary>
    public long ID { get; set; }

    /// <summary>
    /// 姓名
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 性别
    /// </summary>
    public bool Sex { get; set; }

    /// <summary>
    /// 年龄
    /// </summary>
    public int Age { get; set; }

    /// <summary>
    /// 电话
    /// </summary>
    public string Phone { get; set; }

    /// <summary>
    /// 邮箱
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// 地址
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// 有效性
    /// </summary>
    public bool Valid { get; set; }

    /// <summary>
    /// 企业编号
    /// </summary>
    public long EnterpriseID { get; set; }

    /// <summary>
    /// 企业角色编号
    /// </summary>
    public long RoleID { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 创建用户
    /// </summary>
    public long CreateUser { get; set; }

    /// <summary>
    /// 修改时间
    /// </summary>
    public DateTime ModifyTime { get; set; }

    /// <summary>
    /// 修改用户
    /// </summary>
    public long ModifyUser { get; set; }
}
