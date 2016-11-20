--create database CDS;
--use CDS;

--drop table UserTypes
create table UserTypes(
Id int identity(1,1) primary key,
Name varchar(10) not null,
CreatedAt datetime not null default current_timestamp);
Insert into UserTypes(name) values('ADMIN');
Insert into UserTypes(name) values('COMMON');

--drop table Groups
create table Groups(
Id int identity(1,1) primary key,
Name varchar(50) not null,
CreatedAt datetime not null default current_timestamp,
Active bit not null default 1);
Insert into Groups(name) values ('default');

--drop table Users
create table Users(
Id int identity(1,1) primary key,
IdGroup int foreign key references Groups(Id),
IdType int foreign key references UserTypes(Id),
Name varchar(50) not null,
Email varchar(75) unique not null,
Password varbinary(max) not null,
CPF varchar(12) unique not null,
BirthDate datetime,
Gender tinyint,
Phone varchar(12),
Cellphone varchar(12),
Adress varchar(max),
CEP varchar(8),
State char(2),
CreatedAt datetime not null default current_timestamp,
Active bit not null default 1);


create table ContentTypes(
Id int identity(1,1)  primary key,
Name varchar(25) not null,
CreatedAt datetime not null default current_timestamp,
Active bit not null default 1)
Insert into ContentTypes (Name) Values('Ebook');
Insert into ContentTypes (Name) Values('Article');
Insert into ContentTypes (Name) Values('Podcast');


--drop table Contents
create table Contents(
Id int identity(1,1) primary key,
Name varchar(50) not null,
Description varchar(max),
FileName varchar(50) not null,
BeginDeliveryDate datetime not null,
EndDeliveryDate datetime not null,
IsBroadcast bit not null default 0,
CreatedAt datetime not null default current_timestamp);
alter table Contents
add ISBN varchar(25);
alter table Contents
add Price float not null default 0;
alter table Contents
add Duration float not null default 0;
alter table Contents
add PublishYear int not null;
Alter table Contents
add IdType int not null foreign key references ContentTypes(Id);

--drop table GroupContents
create table GroupContents(
Id int identity(1,1) primary key,
IdGroup int not null references Groups(Id),
IdContent int not null references Contents(Id),
CreatedAt datetime not null default current_timestamp);

--drop table TokenTypes
create table TokenTypes(
Id int identity(1,1) primary key,
Name varchar(10) not null,
CreatedAt datetime not null default current_timestamp)
Insert into TokenTypes(name) values('WebAuth');
Insert into TokenTypes(name) values('ApiAuth');
Insert into TokenTypes(name) values('Content');
Insert into TokenTypes(name) values('WebContent');


--drop table Tokens
create table Tokens(
Id int identity(1,1) primary key,
IdType int not null references TokenTypes(Id),
IdUser int not null references Users(Id),
IdContent int references Contents(Id),
Code char(32) not null unique,
ExpireDate datetime not null,
CreatedAt datetime not null default current_timestamp)

--drop table Genres
create table Genres(
Id int identity(1,1) primary key,
Name varchar(25) not null,
IdParent int foreign key references Genres(Id))

--drop table GenreContents
create table GenreContents(
IdGenre int foreign key references Genres(Id),
IdContent int foreign key references Contents(Id)
Primary key(IdGenre,IdContent))