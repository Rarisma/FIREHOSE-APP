CREATE DATABASE IF NOT EXISTS FHDB; 
USE FHDB;
CREATE TABLE IF NOT EXISTS articles_archive_new (
    URL VARCHAR(512) NOT NULL PRIMARY KEY,  		-- Article URL
    TITLE VARCHAR(512) NOT NULL,            		-- Article Title
    PUBLISHED DATETIME NOT NULL,         			-- Article Publication Date Time
    SUMMARY LONGTEXT NOT NULL,                      -- AI Summary of Article
	IMAGE VARCHAR(1024),							-- A URL to the image that represents the article.
    CONTENT LONGTEXT NOT NULL,         				-- Text of Article
    PUBLISHER INT NOT NULL, 						-- Firehose Publication ID
	AUTHOR VARCHAR(1024),							-- Name of author(s) separated by commas
    CLICKBAIT_COUNTER INT default 0, 				-- Ammount of times article has been reported as clickbait
    REPORT_COUNTER INT default 0, 					-- Ammount of times summary has been reported
    IMPACT INT NOT NULL,                          	-- Importance Score of Article (0- unimportant, 50 - important, 80+ Breaking)
	READ_TIME int default 0,						-- How long it takes to read the article.
    VIEWSALL int default 0,							-- Views all time
    VIEWSDAY int default 0,							-- Views daily (Updated every 15 minutes unless within archive.)
    CATEGORY VARCHAR(128)
);

CREATE TABLE IF NOT EXISTS ARTICLES_NEW_EXDATA(
	URL VARCHAR(512) NOT NULL PRIMARY KEY,			-- Article URL
    EMBEDDINGS BLOB,								-- Embeddings generated
    MODEL VARCHAR(32),								-- LLM used for summary
    GEN_TIME DATETIME, 								-- Actual time the summary was generated at.
    READER_MODE VARCHAR(32),						-- Reader mode used (i.e smart reader, scrape)
    HEADLINE_REWRITTEN BOOLEAN DEFAULT FALSE,		-- Has the headline been rewritten
    OLD_HEADLINE VARCHAR(512)						-- Original headline if the article has been rewritten
);

INSERT IGNORE INTO ARTICLES_NEW (URL, TITLE, PUBLISHED, SUMMARY, IMAGE, CONTENT, PUBLISHER, AUTHOR, CLICKBAIT_COUNTER, REPORT_COUNTER, IMPACT, READ_TIME, VIEWSALL, VIEWSDAY, CATEGORY)
SELECT 
    URL, 
    TITLE, 
    PUBLISH_DATE AS PUBLISHED, 
    SUMMARY, 
    ImageURL AS IMAGE, 
    ARTICLE_TEXT AS CONTENT, 
    PUBLISHER_ID AS PUBLISHER, 
    AUTHOR, 
    TimesReportedAsClickbait AS CLICKBAIT_COUNTER, 
    TimesReportedAsSummaryReported AS REPORT_COUNTER, 
    Impact, 
    TimeToRead AS READ_TIME, 
    VIEWSALL, 
    VIEWSDAY,
    TAGS AS CATEGORY
FROM ARTICLES;

select * FROM articles_archive_new;
select * FROM articles_new;
SET SESSION net_read_timeout = 600;
SET SESSION net_write_timeout = 600;
