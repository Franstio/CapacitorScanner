
CREATE TABLE scraptransaction( id integer primary key autoincrement,
transaction_date text,
login_date text,
badgeno text,
container text,
bin text,
status text,
host text,
weightresult real,
doorstatus integer,
activity text,
scrapitem_name text,
scraptype_name text,
scrapgroup_name text,
lastbadgeno text,
);

CREATE TABLE login( id integer primary key autoincrement,
username text,
password text,
);

CREATE TABLE binhost( bin TEXT primary key,
hostname TEXT,
);