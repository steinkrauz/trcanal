/*
 * trcanal -- TruConf log analyzer, 2022
 *
 * Active Directory adapter
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
using System.Configuration;
using System.DirectoryServices.Protocols;
using System.Net;

namespace vksanal
{
    class ADConnector
    {
        LdapConnection connector;
        private Dictionary<string, string> cache = new Dictionary<string, string>();

        private static ADConnector conn = null;

        public static ADConnector Get()
        {
            if (conn == null)
            {
                conn = new ADConnector();
            }
            return conn;
        }

        private ADConnector()
        {
            try
            {
                NetworkCredential credentials = new NetworkCredential(ConfigurationManager.AppSettings["ldapUserID"], ConfigurationManager.AppSettings["ldapPassword"]);
                connector = new LdapConnection(new LdapDirectoryIdentifier(ConfigurationManager.AppSettings["ldapServer"]), credentials);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public string GetName(string login)
        {
            string name;
            if (cache.ContainsKey(login))
            {
                name = cache[login];
            } else
            {
                name = GetNameDirect(login);
                cache.Add(login, name);
            }
            return name;
        }

        private string GetNameDirect(string login)
        {
            string Name = "";
            string attr = "sAMAccountName";


            string userStore = ConfigurationManager.AppSettings["UserStore"];
            string searchFilter = "(&(objectClass=user)(" + attr + "=" + login + "))";
            SearchRequest searchRequest = new SearchRequest(userStore, searchFilter, System.DirectoryServices.Protocols.SearchScope.Subtree,
              new string[] { "displayName" });
            SearchResponse response = (SearchResponse)connector.SendRequest(searchRequest);

            if (response.Entries.Count == 0)
                return login;

            foreach (SearchResultEntry entry in response.Entries)
            {
                DirectoryAttribute attrName = entry.Attributes["displayName"];
                Name = (string)attrName.GetValues(Type.GetType("System.String"))[0];
            }
            return Name;

        }
    }
}


