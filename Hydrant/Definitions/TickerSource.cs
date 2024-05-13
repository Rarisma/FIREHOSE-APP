namespace HYDRANT.Definitions;
//alexander the ok

/// <summary>
/// Where did the company come from?
/// </summary>
public enum TickerSource
{
	/// <summary>
	/// Company originates from FTSE 100
	/// </summary>
    FTSE_100,

	/// <summary>
	/// Company originates from FTSE 250
	/// </summary>
    FTSE_250,

	/// <summary>
	/// Company originates from FTSE 350
	/// </summary>
    FTSE_350,

	/// <summary>
	/// Company originates from Dow Jones Industrial Average
	/// </summary>
    DowJones,

	/// <summary>
	/// Company originates from Nifty 50
	/// </summary>
    Nifty50,
}