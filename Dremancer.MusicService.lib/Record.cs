﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ServiceModel;
using System.Runtime.Serialization;

namespace Dremancer.MusicService.lib
{
  [DataContract]
  public class Record
  {
    [DataMember]
    public string Name { get; private set; }
    [DataMember]
    public DateTime ReleaseDate { get; set; }
    [DataMember]
    public Dictionary<string, string> ApiIds { get; private set; }
    [DataMember]
    public List<Song> Songs { get; private set; }
    //cover obrazok
  }
}