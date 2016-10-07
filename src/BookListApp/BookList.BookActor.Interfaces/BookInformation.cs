using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BookList.BookActor.Interfaces {
    [DataContract]
    public class BookInformation {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public int Pages { get; set; }
    }
}
