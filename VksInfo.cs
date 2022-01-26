using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vksanal
{
    public class VksInfo
    {
        private DateTime _startTime;
        private DateTime _endTime;
        public string Id;
        public string Topic;
        public DateTime StartTime {
            get {
                return this._startTime;
            }
            set
            {
                this._startTime = value;
                this._endTime = this.StartTime.AddDays(1);
            }
        }
        public DateTime EndTime {
            get { return this._endTime; }
            set
            {
                this._endTime = value;
            }
        }
        public int Duration
        {
            get { return (int)this.EndTime.Subtract(StartTime).TotalMinutes; }
        }

        public List<string> Participants = new List<string>();
    }
}
