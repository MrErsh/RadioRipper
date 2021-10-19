#pragma warning disable CA1416 // Validate platform compatibility
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.DataTransfer;

namespace MrErsh.RadioRipper.Client
{
    public static class ClipboardHelper
    {
        public static void CopyList(IEnumerable<string> items)
        {
            string result = string.Empty;
            if (items != null)
            {
                var sb = new StringBuilder();
                foreach (var item in items)
                {
                    sb.AppendLine(item);
                }
                result = sb.ToString();
            }

            var data = new DataPackage();
            data.SetText(result);
            Clipboard.SetContent(data);
        }
    }
}
#pragma warning restore CA1416 // Validate platform compatibility
