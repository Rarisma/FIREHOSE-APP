CREATE DATABASE IF NOT EXISTS FHDB; -- FHDB = Firehose Database
USE FHDB;
--drop TABLE ARTICLES;
CREATE TABLE IF NOT EXISTS ARTICLES (
    URL VARCHAR(512) NOT NULL PRIMARY KEY,  -- URL to article
    TITLE VARCHAR(512) NOT NULL,            -- Story title
	RSS_SUMMARY longtext NOT NULL,     		-- Summary provided by RSS Feed.
    PUBLISH_DATE DATETIME NOT NULL,         -- Date of publication
    HEADLINE BOOLEAN NOT NULL,              -- Is the article a major story
    PAYWALL BOOLEAN NOT NULL,               -- Is the article paywalled?
    SUMMARY LONGTEXT,                       -- AI generated summary of the article content
	ImageURL VARCHAR(1024) NOT NULL,		-- A URL to the image that represents the article.
    ARTICLE_TEXT LONGTEXT NOT NULL,         -- Full content of article
    PUBLISHER_ID INT NOT NULL, 				-- Firehose Publication ID
    BUSINESS_RELATED BOOLEAN NOT NULL,      -- Does the article talk about/relate to business
    COMPANIES_MENTIONED LONGTEXT,           -- List of companies mentioned in the article
    JAKE_FLAG BOOLEAN NOT NULL,             -- Should Jake be informed immediately?
	AUTHOR VARCHAR(1024),					-- Name of author(s) separated by commas.

    WEIGHTING INT,                          -- UNIMPLEMENTED (Trustworthiness Score)
    EXECUTIVES_MENTIONED LONGTEXT          -- List of executives
);


select * from ARTICLES;