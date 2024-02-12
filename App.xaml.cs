namespace REMNANT;
//"But my bars are like report cards in the sky, upgrades"
public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}

/*
 * REMNANT is a news interface for Firehose.
 * Firehose is a closed source news aggregator designed by yours truly.
 * Not all Firehose fields are accessible to REMNANT users however most are.
 * Most files here have song lyrics of the song I was listening to at time at the top them, feel free to ignore them.
 * Fun Fact: Firehose is named after the twitter Firehose API as its intended to provide a flood of news articles.
 */