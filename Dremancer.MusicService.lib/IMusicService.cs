using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace Dremancer.MusicService.lib
{
  // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
  [ServiceContract]
  public interface IMusicService
  {
    [OperationContract]
    [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Artists/{name}")]
    List<SimpleArtist> SearchArtists(string name);

    [OperationContract]
    [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Artist/{artistId}")]
    Artist GetArtist(string artistId);

    [OperationContract]
    [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/ArtistRecords/{artistId}")]
    List<Record> GetArtistRecords(string artisrtId);

    [OperationContract]
    [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Record/{id}")]
    List<Record> GetRecord(string id);

    [OperationContract]
    [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "HasNewRecords/{artistId}")]
    bool HasNewRecords(string artistId); //domysliet ako toto riesit ci na strane service, alebo na strane klienta

    [OperationContract]
    [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json, UriTemplate = "/Shows/{artistId}")]
    List<Record> GetShows(string artistId);

    //TODO: metoda na shows near me, vymysliet ako (uzivatelova poloha)
  } 
}
