CREATE DATABASE IF NOT EXISTS FHDB; -- FHDB = Firehose Database
USE FHDB;
CREATE TABLE IF NOT EXISTS ARTICLES (
    URL VARCHAR(512) NOT NULL PRIMARY KEY,  		-- URL to article
    TITLE VARCHAR(512) NOT NULL,            		-- Story title
    PUBLISH_DATE DATETIME NOT NULL,         		-- Date of publication
    PAYWALL BOOLEAN NOT NULL,              			-- Is the article paywalled?
    SUMMARY LONGTEXT NOT NULL,                      -- AI generated summary of the article content
	ImageURL VARCHAR(1024) default '?',				-- A URL to the image that represents the article.
    ARTICLE_TEXT LONGTEXT NOT NULL,         		-- Full content of article
    PUBLISHER_ID INT NOT NULL, 						-- Firehose Publication ID
    BUSINESS_RELATED BOOLEAN NOT NULL,      		-- Does the article talk about/relate to business
    BREAKING BOOLEAN DEFAULT FALSE,      				-- Is the article major news
    COMPANIES_MENTIONED LONGTEXT,           		-- List of companies mentioned in the article
	AUTHOR VARCHAR(1024),							-- Name of author(s) separated by commas.
	SECTORS LONGTEXT,								-- List of sectors the article is mentioned in
    TimesReportedAsClickbait INT default 0, 		-- Does the article have a clickbait headline
    TimesReportedAsSummaryReported INT default 0, 	-- Does the article have a bad summary headline
    ClickbaitHeadline VARCHAR(255),					-- Headline with clickbait removed.
    Impact INT NOT NULL,                          	-- Importance Score of Article
	TimeToRead int default 0,						-- How long it takes to read the article.
	UserGeneratedArticle BOOL default false,		-- Marks articles submitted via discord bot
	pubname varchar(512) default null,				-- publication name for UGC
-- UNINMPLEMENTED
    EXECUTIVES_MENTIONED LONGTEXT           		-- List of executives
);

select * from ARTICLES;