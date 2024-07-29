
namespace HYDRANT.Definitions;
internal class Account
{
    /// <summary>
    /// Account name
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Account token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Internal account ID
    /// </summary>
    public Guid UserID { get; set; }

    /// <summary>
    /// Should the user see FHN+ features
    /// (Won't be accessible if the user doesn't have a FHN+ account)
    /// </summary>
    public bool FHNPlus;

    /// <summary>
    /// Are they a part of the test group?
    /// (Won't be accessible if the user isn't in the test group)
    /// </summary>
    public bool TestGroup;
}
