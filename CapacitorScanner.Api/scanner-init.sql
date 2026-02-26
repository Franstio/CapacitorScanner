CREATE TABLE scraptransaction(
id integer primary key autoincrement,
transaction_date text,
login_date text,
badgeno text,
container text,
bin text,
status text,
host text,
weightresult real,
activity text,
lastbadgeno text
);

CREATE TABLE login(
id integer primary key autoincrement,
username text,
password text
);


CREATE TABLE binhost(
bin TEXT primary key,
weight real,
wastetype text,
maxweight real,
lastfrombinname text,
weightsystem text,
binweight real,
hostname TEXT,
status TEXT
);