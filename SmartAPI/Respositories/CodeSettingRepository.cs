using Dapper;
using MySql.Data.MySqlClient;
using SmartAPI.Models;

namespace SmartAPI.Respositories
{
    public class CodeSettingRepository : ICodeSettingRepository
    {
        readonly string _connectionString;
        public CodeSettingRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<IEnumerable<CodeSetting>> GetCodeSettingsAsync(CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    SELECT 
                        id AS Id,
                        language_name AS LanguageName,
                        language_runtime AS LanguageRuntime,
                        language as Language,
                        serial_number as SerialNumber,
                        runtime_prompt AS RuntimePrompt,
                        code_filename AS CodeFilename,
                        code_template AS CodeTemplate,
                        additional_filename AS AdditionalFilename,
                        additional_templates AS AdditionalTemplates,
                        entry_point AS EntryPoint,
                        validate AS Validate,
                        create_time AS CreateTime,
                        create_user AS CreateUser,
                        modify_time AS ModifyTime,
                        modify_user AS ModifyUser
                        FROM code_setting where validate=true order by serial_number
                    """;
                var command = new CommandDefinition(
                  sql, cancellationToken: cancellationToken);
                return await connection.QueryAsync<CodeSetting>(command);
            }
        }
        public async Task<CodeSetting?> GetCodeSettingAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var sql = """
                    SELECT 
                        id AS Id,
                        language_name AS LanguageName,
                        language_runtime AS LanguageRuntime,
                        runtime_prompt AS RuntimePrompt,
                        language as Language,
                        serial_number as SerialNumber,
                        code_filename AS CodeFilename,
                        code_template AS CodeTemplate,
                        additional_filename AS AdditionalFilename,
                        additional_templates AS AdditionalTemplates,
                        entry_point AS EntryPoint,
                        validate AS Validate,
                        create_time AS CreateTime,
                        create_user AS CreateUser,
                        modify_time AS ModifyTime,
                        modify_user AS ModifyUser
                        FROM code_setting  
                        WHERE ID=@id
                    """;
                var command = new CommandDefinition(sql, new { id }, cancellationToken: cancellationToken);
                return await connection.QuerySingleOrDefaultAsync<CodeSetting>(command);
            }
        }
        public async Task<int> CreateCodeSettingAsync(CodeSetting codeSetting, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("INSERT INTO code_setting (language_name, language_runtime, language, serial_number,runtime_prompt, code_filename, code_template, additional_filename, additional_templates, entry_point, validate, create_time, create_user, modify_time, modify_user) VALUES (@LanguageName, @LanguageRuntime,@Language, @SerialNumber, @RuntimePrompt, @CodeFilename, @CodeTemplate, @AdditionalFilename, @AdditionalTemplates, @EntryPoint, @Validate, @CreateTime, @CreateUser, @ModifyTime, @ModifyUser)", codeSetting, cancellationToken: cancellationToken);
                return await connection.ExecuteAsync(command);
            }
        }
        public async Task<int> UpdateCodeSettingAsync(CodeSetting codeSetting, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("UPDATE code_setting SET language_name = @LanguageName, language_runtime = @LanguageRuntime, language=@Language,serial_number=@SerialNumber,runtime_prompt=@RuntimePrompt,code_filename = @CodeFilename, code_template = @CodeTemplate, additional_filename = @AdditionalFilename, additional_templates = @AdditionalTemplates, entry_point = @EntryPoint, validate = @Validate, modify_time = @ModifyTime, modify_user = @ModifyUser WHERE id = @Id", codeSetting, cancellationToken: cancellationToken);
                return await connection.ExecuteAsync(command);
            }
        }
        public async Task<int> DeleteCodeSettingAsync(int id, CancellationToken cancellationToken)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                var command = new CommandDefinition("DELETE FROM code_setting WHERE id = @Id", new { id }, cancellationToken: cancellationToken);
                return await connection.ExecuteAsync(command);
            }
        }
    }
}
