using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicWebApi.Helpers
{
    public class SearchGenrePrefixComparer : IEqualityComparer<string>
    {
        static SearchGenrePrefixComparer() { }

        //public int Compare(string x, string y)
        //{
           
        //}

        public bool Equals(string x, string y)
        {
            if (!String.IsNullOrEmpty(x) && !String.IsNullOrEmpty(y))
            {
                string[] xGenreArray = x.Split(' ', '-', '/');
                string[] yGenreArray = y.Split(' ', '-', '/');

                bool result = xGenreArray[0].ToLower() == yGenreArray[0].ToLower();
                return result;
            }
            else
                return StringComparer.OrdinalIgnoreCase.Compare(x.ToLower(), y.ToLower()) > 0;
        }

        public int GetHashCode(string obj)
        {
            string[] xGenreArray = obj.Split(' ', '-', '/');
            return xGenreArray[0].ToLower().GetHashCode();
        }
    }

    public class SearchGenreComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            if (!String.IsNullOrEmpty(x) && !String.IsNullOrEmpty(y))
            {
                string[] xGenreArray = x.Split(' ', '-', '/');
                string[] yGenreArray = y.Split(' ', '-', '/');

                bool result = xGenreArray.Last().ToLower() == yGenreArray.Last().ToLower();
                return result;
            }
            else
                return StringComparer.OrdinalIgnoreCase.Compare(x.ToLower(), y.ToLower()) > 0;
        }

        public int GetHashCode(string obj)
        {
            string[] xGenreArray = obj.Split(' ', '-', '/');
            return xGenreArray.Last().ToLower().GetHashCode();
        }
    }
}
