CREATE DATABASE IF NOT EXISTS FHDB; -- FHDB = Firehose Database
USE FHDB;
drop TABLE ARTICLES;
CREATE TABLE IF NOT EXISTS ARTICLES (
    TITLE VARCHAR(255) NOT NULL,            -- Story title
    URL VARCHAR(255) NOT NULL PRIMARY KEY,  -- URL to article
    ARTICLE_TEXT LONGTEXT NOT NULL,         -- Full content of article
    PUBLISH_DATE DATETIME NOT NULL,         -- Date of publication
    RSS_SUMMARY VARCHAR(255) NOT NULL,      -- Summary provided by RSS Feed.
    SUMMARY LONGTEXT,                       -- AI generated summary of the article content
    WEIGHTING INT,                          -- UNIMPLEMENTED (Trustworthiness Score)
    LOW_QUALITY BOOLEAN NOT NULL,           -- Is the article low quality (i.e gossip)
    BUSINESS_RELATED BOOLEAN NOT NULL,      -- Does the article talk about/relate to business
    PAYWALL BOOLEAN NOT NULL,               -- Is the article paywalled?
    COMPANIES_MENTIONED LONGTEXT,           -- List of companies mentioned in the article
    EXECUTIVES_MENTIONED LONGTEXT,          -- List of executives
    JAKE_FLAG BOOLEAN NOT NULL,             -- Should Jake be informed immediately?
	ImageURL VARCHAR(255) NOT NULL			-- A URL to the image that represents the article.
);

DROP USER IF EXISTS 'remnant'@'%';
FLUSH PRIVILEGES;
CREATE USER 'remnant'@'%' IDENTIFIED BY 'remnant';
GRANT SELECT ON fhdb.* TO 'remnant'@'%';
FLUSH PRIVILEGES;
SELECT user, host FROM mysql.user WHERE user = 'remnant';
ALTER USER 'remnant'@'%' IDENTIFIED WITH mysql_native_password BY 'remnant';



SELECT user, host FROM mysql.user WHERE user = 'remnant';
SHOW GRANTS FOR 'remnant'@'%';


select * from Articles;
															