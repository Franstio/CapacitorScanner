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
CREATE table station(
id integer primary key autoincrement,
property text,
datavalue text
);
CREATE table employee(
id integer primary key autoincrement,
employeename text,
badgeno text,
registerdate text
);
CREATE TABLE binhost(
bin TEXT primary key,
weight real,
wastetype text,
maxweight real,
lastfrombinname text,
lastbadgeno TEXT,
weightsystem text,
binweight real,
hostname TEXT,
status TEXT
);


CREATE TABLE IF NOT EXISTS containerbin (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT ,
    description TEXT ,
    scrapitem_name TEXT ,
    scraptype_name TEXT ,
    weight NUMERIC ,
    capacity NUMERIC ,
    weightresult NUMERIC ,
    weightsystem NUMERIC ,
    wastestation_name TEXT ,
    department_name TEXT ,
    logindate TEXT,           
    doorstatus INTEGER,       
    lastfrombinname TEXT ,
    url TEXT ,
    scrapgroup_name TEXT ,
    lastbadgeno TEXT ,
    activity TEXT 
);
