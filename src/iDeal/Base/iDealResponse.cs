using System;

namespace iDeal.Base
{
    public abstract class iDealResponse
    {
        public int AcquirerId { get; protected set; }

        public string CreateDateTimeStamp { get; protected set; }

        public DateTime CreateDateTimeStampLocalTime { get { return DateTime.Parse(CreateDateTimeStamp); } }
    }
}
