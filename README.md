# trcanal
TrueConf log analyzer

This utility extracts some basic statistics from TrueConf log file, enriching them from Active Directory. It extracts conferences start timestamp, topic for conferences, list of participatiing users and total time in minutes. Any username is assumed to be a SAMAccountName and is checked against Active Directory. If found, the username is replaced with displayName attribute from AD record. Usernames with and without hostname attached with @ are considered to be the same. Usernames with '#' as the first symbol are ignored. Conferences that were less than 5 minutes long are skipped, because in 99% they are irrelevant.

## install
This utility saves extracted data into a MS SQL database, so you'll have to created two tables to store the data. Create tables as follows:

```sql
CREATE TABLE [dbo].[VKSSESSIONS](
	[id] [nchar](16) NOT NULL,
	[topic] [nchar](256) NOT NULL,
	[start_time] [datetime] NOT NULL,
	[end_time] [datetime] NOT NULL
)

CREATE TABLE [dbo].[VKSUSERS](
	[id] [nchar](16) NOT NULL,
	[name] [nchar](128) NOT NULL
) 
```

## config
Connection credentials and other information is stored in .config file along with the executable. The meaning of keys is described below:
<dl>
<dt>Server</dt>
<dd>MS SQL Server host name</dd>
<dt>UserID</dt>
<dd>MS SQL login name</dd>
<dt>Password</dt>
<dd>MS SQL user password</dd>
<dt>Database</dt>
<dd>MS SQL database name</dd>
<dt>ldapUserID</dt>
<dd>Domain login</dd>
<dt>ldapPassword</dt>
<dd>Domain password</dd>
<dt>ldapServerv</dd>
<dd>Domain Controller hostname</dd>
<dt>UserStore</dt>
<dd>Organisation Unit where to look up users</dd>
</dl>

## usage
trcanal.exe -file=P:\ath\to\stdout.log

## license
This program is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, version 3 of the License.

This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with this program. If not, see https://www.gnu.org/licenses/.
