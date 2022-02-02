/*
 * trcanal -- TruConf log analyzer, 2022
 *
 * Collection of log parsers
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace vksanal
{
    public interface IParser
    {
        bool IsApplicable(string str);
        void Parse(string str);
    }


    public abstract class BaseParser : IParser
    {
        protected Dictionary<string, VksInfo> infos;
        protected Regex key;

        public BaseParser(Dictionary<string, VksInfo> storage, String matchExp)
        {
            infos = storage;
            key = new Regex(matchExp);
        }

        protected DateTime Str2Time(string str)
        {
            DateTime result;
            try
            {
                result = DateTime.ParseExact(str.Trim(), "dd/MM/yyyy HH:mm:ss.fff", CultureInfo.CurrentCulture);
            } catch (FormatException ex)
            {
                Console.WriteLine(ex.Message);
                result = DateTime.ParseExact("01/01/2020", "dd/MM/yyyy", CultureInfo.InvariantCulture);
            }
            return result;
        }

        public bool IsApplicable(string str)
        {
            return key.IsMatch(str);
        }

        public abstract void Parse(string str);
    }

    public class CreateParser : BaseParser
    {
        
        public CreateParser(Dictionary<string, VksInfo> storage) : base(storage, @"CreateConference <(.*)@.*")
        {
        }


        public override void Parse(string str)
        {
            string[] parts = str.Split('|');
            Match result = key.Match(parts[4]);
            if (result.Success)
            {
                string confId = result.Groups[1].Value;
                VksInfo info = new VksInfo()
                {
                    Id = confId,
                    StartTime = Str2Time(parts[0])
                };
                infos.Add(confId, info);
            }

        }
    }

    public class TopicParser : BaseParser
    {
        public TopicParser(Dictionary<string, VksInfo> storage) : base(storage, @"Create MC <(.*)@.*topic=(.*),.*")
        {
        }

        public override void Parse(string str)
        {
            string[] parts = str.Split('|');
            Match result = key.Match(parts[4]);
            if (result.Success)
            {
                string confId = result.Groups[1].Value;
                if (infos.ContainsKey(confId))
                {
                    infos[confId].Topic = result.Groups[2].Value;
                }
            }

        }
    }

    public class ParticipantParser : BaseParser
    {
        ADConnector ad;

        public ParticipantParser(Dictionary<string, VksInfo> storage) : base(storage, @"AddParticipant <(.*)@.*<(.*)@.*")
        {
            ad = ADConnector.Get();
        }

        public override void Parse(string str)
        {
            string[] parts = str.Split('|');
            Match result = key.Match(parts[4]);
            if (result.Success)
            {
                string confId = result.Groups[2].Value;
                //Need this to clean duplicaties like artamonov and artamonov@vks.contoso.com-<%%>-john.doe
                string login = result.Groups[1].Value.Split('@')[0];
                if (login.StartsWith("#")) return;  //those are unneeded #guest:9571b950
                string name = ad.GetName(login);
                
                if (infos.ContainsKey(confId))
                {
                    if (!infos[confId].Participants.Contains(name))
                        infos[confId].Participants.Add(name);
                }
            }
        }
    }


    
    public class EndParser : BaseParser
    {

        public EndParser(Dictionary<string, VksInfo> storage) : base(storage, @"LogConferenceEnd\((.*)@.*\)")
        {
        }

        public override void Parse(string str)
        {
            string[] parts = str.Split('|');
            Match result = key.Match(parts[4]);
            if (result.Success)
            {
                string confId = result.Groups[1].Value;

                if (infos.ContainsKey(confId))
                {                    
                    infos[confId].EndTime = Str2Time(parts[0]);
                }
            }
        }
    }

}
