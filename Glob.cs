using MySql.Data.MySqlClient;
using REMNANT.Typing;

namespace REMNANT;
//I'VE GOT A RECKLESS TONGUE
public static class Glob
{
    //Connection to the DB
    public static MySqlConnection DB = new ("server=localhost;user=remnant;database=fhdb;port=3306;password=REMNANT235423__");
   
    //MYSQL Query to get 100 article objects
    public static string query = "SELECT URL, TITLE, RSS_SUMMARY, PUBLISH_DATE, LOW_QUALITY, PAYWALL, SUMMARY FROM ARTICLES LIMIT 100";
    
    //Used for ArticleView.xaml
    public static Article SelectedArticle;
}