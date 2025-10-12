using FluentValidation;
using Microsoft.Extensions.Localization;


namespace SmartAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 版本
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// 文化语言
        /// </summary>
        public string Culture { get; set; }
        /// <summary>
        /// 场影ID
        /// </summary>
        public int SceneId { get; set; }
        public DateTime? CreateTime { get; set; }
        public string CreateUser { get; set; }
        public bool? Validate { get; set; }
        public DateTime? ModifyTime { get; set; }
        public string ModifyUser { get; set; }
    }
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(localizer["product_name_valid_empty"].Value)
                .MaximumLength(40).WithMessage(localizer["product_name_valid_len"].Value);

            RuleFor(x => x.Version)
               .NotEmpty().WithMessage(localizer["product_version_valid_empty"].Value)
               .MaximumLength(40).WithMessage(localizer["product_version_valid_len"].Value);

            RuleFor(x => x.SceneId)
            .GreaterThan(0).WithMessage(localizer["product_sceneid_valid_empty"].Value);

        }
    }
}
