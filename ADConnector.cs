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


