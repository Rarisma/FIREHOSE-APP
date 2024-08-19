CREATE DATABASE IF NOT EXISTS FHDB; -- FHDB = Firehose Database
USE FHDB;
CREATE TABLE IF NOT EXISTS ARTICLES (
    URL VARCHAR(512) NOT NULL PRIMARY KEY,  		-- URL to article
    TITLE VARCHAR(512) NOT NULL,            		-- Story title
    PUBLISH_DATE DATETIME NOT NULL,         		-- Date of publication
    SUMMARY LONGTEXT NOT NULL,                      -- AI generated summary of the article content
	ImageURL VARCHAR(1024) default '?',				-- A URL to the image that represents the article.
    ARTICLE_TEXT LONGTEXT NOT NULL,         		-- Full content of article
    PUBLISHER_ID INT NOT NULL, 						-- Firehose Publication ID
	AUTHOR VARCHAR(1024),							-- Name of author(s) separated by commas.
    TimesReportedAsClickbait INT default 0, 		-- Does the article have a clickbait headline
    TimesReportedAsSummaryReported INT default 0, 	-- Does the article have a bad summary headline
    Impact INT NOT NULL,                          	-- Importance Score of Article
	TimeToRead int default 0,						-- How long it takes to read the article.
    VIEWSALL int,									-- Views all time
    VIEWSDAY int									-- Views daily
);

# Clickbait Increment Stored Proc
DELIMITER //
CREATE PROCEDURE IncrementClickbaitCount(IN p_url VARCHAR(255))
BEGIN
    UPDATE Articles
    SET TimesReportedAsClickbait = TimesReportedAsClickbait + 1
    WHERE URL = p_url;	
END //
DELIMITER ;

# Summary Report Increment Stored Proc
DELIMITER //
CREATE PROCEDURE IncrementSummaryReportCount(IN p_url VARCHAR(255))
BEGIN
    UPDATE Articles
    SET TimesReportedAsSummaryReported = TimesReportedAsSummaryReported + 1
    WHERE URL = p_url;	
END //
DELIMITER ;

DELIMITER //
CREATE PROCEDURE AddView(IN article_url VARCHAR(512))
BEGIN
    UPDATE Articles
    SET ViewsDay = ViewsDay + 1,
        ViewsAll = ViewsAll + 1
    WHERE URL = article_url;
END //
DELIMITER ;

SELECT * FROM articles;