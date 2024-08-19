using System.Text.Json.Serialization;
using HYDRANT.Definitions;
namespace HYDRANT.Serializers;
/// <summary>
/// This is required for AOT
/// </summary>
[JsonSourceGenerationOptions(WriteIndented = true)]
[JsonSerializable(typeof(List<Publication>))]
public partial class PublicationSerialiser : JsonSerializerContext;
