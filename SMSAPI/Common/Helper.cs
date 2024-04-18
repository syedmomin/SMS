using System.Text;

namespace SMS_API.Common
{
  public class Helper
  {
    public static string secretKey = "gDP1H21qMa";
    public static string EnccyptText(string text)
    {
      if (!string.IsNullOrEmpty(text))
      {
        text = text + secretKey;
        byte[] hashText = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(hashText);
      }
      else
      {
        return "";
      }
    }
  }
}
