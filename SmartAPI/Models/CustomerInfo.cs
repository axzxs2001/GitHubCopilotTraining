using FluentValidation;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;


namespace SmartAPI.Models
{
    public class CustomerInfo
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ResponsiblePerson { get; set; }
        public string ResponsibleEmail { get; set; }
        public List<string> SalespersonEmail { get; set; }
        public List<string> DeveloperEmail { get; set; }
        public string NssSalespersonEmail { get; set; }
        public List<int> Scene { get; set; }
        public bool? IpWhitelist { get; set; }
        public List<string> Wallet { get; set; }
        public bool? IsSubPay { get; set; }
        public bool? IsUseDataTransfer { get; set; }
        public bool? IsUseSms { get; set; }
        public bool? IsUseEmail { get; set; }
        public bool? IsWalletLogo { get; set; }
        public DateTime CreateTime { get; set; }
        public bool? IsUserActive { get; set; }
    }
    public class CustomerInfoValidator : AbstractValidator<CustomerInfo>
    {
        public CustomerInfoValidator(IStringLocalizer<SharedResource> localizer)
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage(localizer["customint_companname_valid_empty"].Value)
                .MaximumLength(250).WithMessage(localizer["customint_companname_valid_len"].Value);

            RuleFor(x => x.ResponsiblePerson)
               .NotEmpty().WithMessage(localizer["customint_responsibleperson_valid_empty"].Value)
               .MaximumLength(250).WithMessage(localizer["customint_responsibleperson_valid_len"].Value);

            RuleFor(x => x.ResponsibleEmail)
           .NotEmpty().WithMessage(localizer["customint_responsibleemail_valid_empty"].Value)
           .MaximumLength(120).WithMessage(localizer["customint_responsibleperson_valid_len"].Value)
           .EmailAddress().WithMessage(localizer["customint_responsibleemail_valid_format"].Value);

            //RuleFor(x => x.SalespersonEmail)
            //    // .NotEmpty().WithMessage("営業用メールアドレスは空にできません。")
            //    .ForEach(s => s.EmailAddress().WithMessage(localizer["cusomerinfo_salespersonemail_valid_empty"].Value)
            //   .MaximumLength(128).WithMessage(localizer["cusomerinfo_salespersonemail_valid_len"].Value));

            RuleFor(x => x.DeveloperEmail)
               // .NotEmpty().WithMessage("開発用メールアドレスは空にできません。")
               .ForEach(s => s.EmailAddress().WithMessage(localizer["cusomerinfo_devemail_valid_empty"].Value)
               .MaximumLength(128).WithMessage(localizer["cusomerinfo_devemail_valid_len"].Value));


            RuleFor(x => x.NssSalespersonEmail)
           .NotEmpty().WithMessage(localizer["customerinfo_nssemail_valid_empty"].Value)
           .MaximumLength(120).WithMessage(localizer["customerinfo_nssemail_valid_len"].Value)
           .EmailAddress().WithMessage(localizer["customerinfo_nssemail_valid_format"].Value);

            RuleFor(x => x.Scene)
                .NotEmpty().WithMessage(localizer["customerinfo_scene_valid_empty"].Value);

        }
    }
}
