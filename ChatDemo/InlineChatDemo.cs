

public class InlineChatDemo
{
    public string HashString(string input)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hashBytes = md5.ComputeHash(inputBytes);
            return System.BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }

    public string HashStringSHA256(string input)
    {
        using (var sha256 = System.Security.Cryptography.SHA256.Create())
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            var hashBytes = sha256.ComputeHash(inputBytes);
            return System.BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
        }
    }
}