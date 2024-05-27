# Firehose
#### A (not so) simple way to scrape article data and make AI summaries

### What is firehose?
Firehose is a project designed to scrape news articles and make AI news summaries.
As this is a large project, it has several subprojects 


#### HYDRANT
HYDRANT provides common class defintions and tooling to access the database.


#### FirehoseServer
Firehose server is ASP.NET server that provides readonly access to the database.
It also scrapes and summarises news articles from a range of sources.


#### FirehoseApp
FirehoseApp is a UNO UI news app written in C#.
