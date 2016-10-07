using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BookList.BookActor.Interfaces {
    [DataContract]
    public class BookCheckoutResponse {
        [DataMember]
        public bool IsSuccessful { get; set; }
        [DataMember]
        public BookStatus Status { get; set; }
    }
}
