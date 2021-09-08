using System.Text;

namespace Ucommerce.Sitecore.Installer
{
    internal static class MessageTemplateParser
    {
        public static bool TryParse(string messageTemplate, object[] propertyValues, out string parsedTemplate)
        {
            parsedTemplate = messageTemplate;
            if (string.IsNullOrWhiteSpace(messageTemplate))
                return false;

            var tokenize = Tokenize(messageTemplate);
            parsedTemplate = string.Format(tokenize, propertyValues);
            return true;
        }

        private static string Tokenize(string messageTemplate)
        {
            var index = 0;
            var propertyCounter = 0;
            var accum = new StringBuilder();

            do
            {
                var nc = messageTemplate[index];
                if (nc == '{')
                {
                    accum.Append($"{{{propertyCounter}}}");
                    propertyCounter++;
                    index = messageTemplate.IndexOf('}', index);
                }
                else if (nc == '}')
                {
                    if (index + 1 < messageTemplate.Length)
                        accum.Append(messageTemplate[index + 1]);
                    index++;
                }
                else
                {
                    accum.Append(nc);
                    index++;
                }
            } while (index < messageTemplate.Length);

            return accum.ToString();
        }
    }
}
