/*
 * trcanal -- TruConf log analyzer, 2022
 *
 * Data storage class
 *
 * written by Steinkrauz <steinkrauz@yahoo.com>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, version 3 of the License ONLY.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program. If not, see <http://www.gnu.org/licenses/>.
 */

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
