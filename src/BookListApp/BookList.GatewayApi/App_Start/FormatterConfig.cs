using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;

namespace BookList.GatewayApi.App_Start {
    public static class FormatterConfig {
        public static void ConfigureFormatters(MediaTypeFormatterCollection formatters) {
            formatters.Remove(formatters.XmlFormatter);

            JsonSerializerSettings settings = formatters.JsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.None;
        }
    }
}
