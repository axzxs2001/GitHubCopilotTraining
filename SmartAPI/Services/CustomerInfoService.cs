using CNSSDK;
using SmartAPI.Models;
using SmartAPI.Respositories;

namespace SmartAPI.Services
{
    public class CustomerInfoService : ICustomerInfoService
    {
        readonly ICustomerInfoRepository _customerInfoRepository;
        readonly ILogger<CustomerInfoService> _logger;
        readonly ICnsSDK _cns;

        public CustomerInfoService(ICnsSDK cns, ILogger<CustomerInfoService> logger, ICustomerInfoRepository customerInfoRepository)
        {
            _cns = cns;
            _customerInfoRepository = customerInfoRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<CustomerInfo>> GetCustomerInfosAsync(string companyName, string responsiblePerson, int pageSize, int pageNumber, CancellationToken cancellationToken)
        {
            return await _customerInfoRepository.GetCustomerInfosAsync(companyName, responsiblePerson, pageSize, pageNumber, cancellationToken);
        }

        public async Task<int> GetCustomerInfoCountAsync(string companyName, string responsiblePerson, CancellationToken cancellationToken)
        {
            return await _customerInfoRepository.GetCustomerInfoCountAsync(companyName, responsiblePerson, cancellationToken);
        }


        public async Task<CustomerInfo?> GetCustomerInfoAsync(int id, CancellationToken cancellationToken)
        {
            return await _customerInfoRepository.GetCustomerInfoAsync(id, cancellationToken);
        }

        public async Task<bool> ActiveUserAsync(int id, CancellationToken cancellationToken)
        {
            return await _customerInfoRepository.ActiveUserAsync(id, cancellationToken);
        }
        public async Task<bool> AddCustomerInfoAsync(CustomerInfo customerInfo, CancellationToken cancellationToken)
        {
            var result = await _customerInfoRepository.CreateCustomerInfoAsync(customerInfo, cancellationToken);
            if (result)
            {
                return await SendEmailAsync(customerInfo.NssSalespersonEmail, customerInfo.CompanyName);
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> ModifyCustomerInfoAsync(CustomerInfo customerInfo, CancellationToken cancellationToken)
        {
            return await _customerInfoRepository.UpdateCustomerInfoAsync(customerInfo, cancellationToken);
        }

        public async Task<bool> RemoveCustomerInfoAsync(int id, CancellationToken cancellationToken)
        {
            return await _customerInfoRepository.DeleteCustomerInfoAsync(id, cancellationToken);
        }

        async Task<bool> SendEmailAsync(string email, string companyName)
        {
            var msgEntery = new BulkMessageEntry()
            {
                UUID = Uti.UnID(),
                ChannelID = "email-smtp-smartapi",
                Destination = new Destination()
                {
                    ToAddresses = new List<string>() { email },
                },
                ReplacementSubject = $"※ご確認※　【{companyName}】がCheckListを提出しました",
                ReplacementContent = $"""
                NSS開発窓口】様

                お疲れ様です。


                【{companyName}】がCheckListを提出しました。ご記入いただいた内容に誤りがないかご確認ください。

                内容に問題がない場合、以下のプラットフォームににアクセスし、アカウントの有効化プロセスに進んでください。

                https://sandbox.nsstest.com/index


                ※ご注意ください：

                アカウントを有効化すると、オンラインでAPI仕様書を閲覧し、スマートAPIアシスタントを利用できます。
                上記会社とのNDAが締結済みかご確認の上、アカウントの有効化を行ってください。

                 
                """
            };
            var req = new SendBulkMessageRequest()
            {
                UUID = Uti.UnID(),
                APIKey = "kkk",
                ChannelID = "email-smtp-smartapi",
                ChannelType = ChannelType.EMAIL.ToString(),
                FromAddress = "csSdk<csSdk@netstars.co.jp>",
                BulkMessageEntries = new List<MessageEntry>() { msgEntery },
                ConfigurationSetName = "",

            };
            var res = await _cns.SendBulkMessage(req);
            if (string.IsNullOrWhiteSpace(res))
            {
                return true;
            }
            else
            {
                _logger.LogError("Send Email Error：{0}", res);
                return false;
            }
        }
        public async Task<int> GetCountAsync(string companyName, CancellationToken cancellationToken)
        {
            return await _customerInfoRepository.GetCountAsync(companyName, cancellationToken);
        }
    }
}

