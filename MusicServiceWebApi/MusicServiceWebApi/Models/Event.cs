using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicServiceWebApi.Models
{
  public class Event
  {
    public Artist Artist { get; set; }
    public DateTime Date { get; set; }
    public string PlaceName { get; set; }
    public decimal Longitude { get; set; }
    public decimal Latitude { get; set; }
  }
}
