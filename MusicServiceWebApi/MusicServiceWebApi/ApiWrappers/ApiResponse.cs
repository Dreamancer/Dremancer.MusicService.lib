using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicServiceWebApi.ApiWrappers
{
  public class ApiResponse
  {
    public string ApiName { get; set; }
    public ResponseType ResponseType { get; set; }
    public JObject Response { get; set; }
  }

  public enum ResponseType
  {
    Artist, Album, Song, Event, Error
  }
}
