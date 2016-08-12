using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ServiceModel;
using System.Runtime.Serialization;


namespace Dremancer.MusicService.lib
{
  [DataContract]
  public class Artist
  {
    [DataMember]
    public string Name { get; private set; }
    [DataMember]
    public string[] Genres { get; private set; }
    /// <summary>
    /// key: api name, value: id of artist in the api
    /// </summary>
    [DataMember]
    public Dictionary<string, string> ApiIds { get; private set; }

    //nejaky obrazok?
  }
}