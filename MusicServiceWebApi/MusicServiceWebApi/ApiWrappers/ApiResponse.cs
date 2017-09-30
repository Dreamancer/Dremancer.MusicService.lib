using MusicWebApi.ApiWrappers.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWebApi.ApiWrappers
{
    public class ApiResponse
    {
        public ApiName ApiName { get; set; }
        public ResponseType ResponseType { get; set; }
        public JObject Response { get; set; }
    }
}
