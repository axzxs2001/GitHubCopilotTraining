using FluentValidation;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.VectorData;


namespace SmartAPI.Models
{
    public class ApiData
    {
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 类别，api，非api
        /// </summary>
        public string CategoryName { get; set; }
        /// <summary>
        /// 产品ID
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// API类型
        /// </summary>
        public string ApiType { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int? SerialNumber { get; set; }
        /// <summary>
        /// 父ID
        /// </summary>
        public int ParentID { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool? Validate { get; set; }
        /// <summary>
        /// 给有回调需求的API配置回调API的ID
        /// </summary>
        public List<int> LinkAPIID { get; set; }
        /// <summary>
        /// 需要关联的返回结果APIID
        /// </summary>
        public List<int> ResultAPIID { get; set; }

        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string ModifyUser { get; set; }
    }
    public class ApiDataValidator : AbstractValidator<ApiData>
    {
        public ApiDataValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer["apidata_name_valid_empty"].Value)
                .MaximumLength(40).WithMessage(localizer["apidata_name_valid_len"].Value);
            RuleFor(x => x.CategoryName)
               .NotEmpty().WithMessage(localizer["apidata_category_valid_empty"].Value)
               .MaximumLength(40).WithMessage(localizer["apidata_category_valid_len"].Value);

            RuleFor(x => x.ProductId)
                .NotNull().WithMessage(localizer["apidata_productid_valid_empty"].Value);

            RuleFor(x => x.SerialNumber)
           .NotNull().WithMessage(localizer["apidata_serialnumber_valid_empty"].Value);

            RuleFor(x => x.ParentID)
                .NotNull().WithMessage(localizer["apidata_parentid_valid_empty"].Value);

        }
    }
}
