using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ServiceModel;
using System.Runtime.Serialization;


namespace Dremancer.MusicService.lib
{
  [DataContract]
  public class Song
  {
    [DataMember]
    public int No { get; private set; }
    [DataMember]
    public string Name { get; private set; }
    [DataMember]
    public string Duration { get; private set; }
  }
}