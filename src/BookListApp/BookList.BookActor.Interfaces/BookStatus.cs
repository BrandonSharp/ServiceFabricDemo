using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BookList.BookActor.Interfaces {
    [DataContract]
    public class BookStatus {
        public BookStatus() { }
        public BookStatus(bool isAvailable, string userWithCurrentLoan, int waitlistLength) {
            IsAvailable = isAvailable;
            UserWithCurrentLoan = userWithCurrentLoan;
            WaitlistLength = waitlistLength;
        }
        [DataMember]
        public bool IsAvailable { get; set; }
        [DataMember]
        public string UserWithCurrentLoan { get; set; }
        [DataMember]
        public int WaitlistLength { get; set; }
    }
}
