CREATE DATABASE IF NOT EXISTS FHDB; -- FHDB = Firehose Database
USE FHDB;
--drop TABLE ARTICLES;
CREATE TABLE IF NOT EXISTS ARTICLES (
    TITLE VARCHAR(512) NOT NULL,            -- Story title
    URL VARCHAR(512) NOT NULL PRIMARY KEY,  -- URL to article
    ARTICLE_TEXT LONGTEXT NOT NULL,         -- Full content of article
    PUBLISH_DATE DATETIME NOT NULL,         -- Date of publication
    RSS_SUMMARY longtext NOT NULL,     		-- Summary provided by RSS Feed.
    SUMMARY LONGTEXT,                       -- AI generated summary of the article content
    WEIGHTING INT,                          -- UNIMPLEMENTED (Trustworthiness Score)
    HEADLINE BOOLEAN NOT NULL,              -- Is the article a major story
    BUSINESS_RELATED BOOLEAN NOT NULL,      -- Does the article talk about/relate to business
    PAYWALL BOOLEAN NOT NULL,               -- Is the article paywalled?
    COMPANIES_MENTIONED LONGTEXT,           -- List of companies mentioned in the article
    EXECUTIVES_MENTIONED LONGTEXT,          -- List of executives
    JAKE_FLAG BOOLEAN NOT NULL,             -- Should Jake be informed immediately?
	ImageURL VARCHAR(1024) NOT NULL,		-- A URL to the image that represents the article.
    PUBLISHER_ID INT NOT NULL 				-- Firehose Publication ID
);

create table IF NOT EXISTS PUBLICATIONS(
	ID int NOT NULL PRIMARY KEY,
    PubName VARCHAR(512) not null,
    Favi varchar(512) not null,
    BaseWeight int not null,			
    AlwaysJakeFlag bool not null
);

select * from PUBLICATIONS;