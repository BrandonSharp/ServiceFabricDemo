using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BookList.Library.Models {
    [DataContract]
    public class BookCreationRequest {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Author { get; set; }
        [DataMember]
        public int PageCount { get; set; }
    }
}
