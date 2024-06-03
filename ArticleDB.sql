CREATE DATABASE IF NOT EXISTS FHDB; -- FHDB = Firehose Database
USE FHDB;
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
	AUTHOR VARCHAR(1024),					-- Name of author(s) separated by commas.
	SECTORS LONGTEXT,						-- List of sectors the article is mentioned in
    Votes INT NOT NULL, 					-- Does a user think 


    WEIGHTING INT,                          -- UNIMPLEMENTED (Trustworthiness Score)
    EXECUTIVES_MENTIONED LONGTEXT,          -- List of executives
    TimesReportedAsClickbait INT NOT NULL, 	-- Does the article have a clickbait headline
    ClickbaitHeadline VARCHAR(255)
);

select * from ARTICLES;