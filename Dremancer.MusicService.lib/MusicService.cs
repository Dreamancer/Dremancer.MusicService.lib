using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Dremancer.MusicService.lib
{
  // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
  public class MusicService : IMusicService
  {
    public Artist GetArtist(string artistId)
    {
      throw new NotImplementedException();
    }

    public List<Record> GetArtistRecords(string artisrtId)
    {
      throw new NotImplementedException();
    }

    public List<Record> GetRecord(string id)
    {
      throw new NotImplementedException();
    }

    public List<Record> GetShows(string artistId)
    {
      throw new NotImplementedException();
    }

    public bool HasNewRecords(string artistId)
    {
      throw new NotImplementedException();
    }

    public List<SimpleArtist> SearchArtists(string name)
    {
      throw new NotImplementedException();
    }
  }
}
